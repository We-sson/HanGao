using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using FluentModbus;
using HanGao.ModbusTcp;

return await ModbusTestApplication.RunAsync(args);

internal static class ModbusTestApplication
{
    public static async Task<int> RunAsync(string[] args)
    {
        try
        {
            StressSettings settings = StressSettings.Parse(args);
            if (settings.ShowHelp)
            {
                StressSettings.PrintHelp();
                return 0;
            }

            Console.WriteLine("HanGao.ModbusTcp verification");
            Console.WriteLine($"Runtime: {Environment.Version}; OS: {Environment.OSVersion}");

            RunCodecTests();
            RunSnapshotTests();
            RunRobotInfoRegisterMapTests();
            await RunProtocolAndLifecycleTestsAsync();
            await RunConcurrentStressTestAsync(settings);

            Console.WriteLine("PASS: all correctness, protocol, lifecycle and concurrency tests completed.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("FAIL: " + exception);
            return 1;
        }
    }

    private static void RunCodecTests()
    {
        ushort[] registers = new ushort[12];

        ModbusRegisterCodec.WriteUInt32(registers, 0, 0x1122_3344);
        AssertEqual((ushort)0x1122, registers[0], "UInt32 high word");
        AssertEqual((ushort)0x3344, registers[1], "UInt32 low word");
        AssertEqual(0x1122_3344u, ModbusRegisterCodec.ReadUInt32(registers, 0), "UInt32 round trip");

        ModbusRegisterCodec.WriteUInt32(
            registers,
            2,
            0x5566_7788,
            ModbusWordOrder.LowWordFirst);
        AssertEqual((ushort)0x7788, registers[2], "Low-word-first low word");
        AssertEqual((ushort)0x5566, registers[3], "Low-word-first high word");
        AssertEqual(
            0x5566_7788u,
            ModbusRegisterCodec.ReadUInt32(registers, 2, ModbusWordOrder.LowWordFirst),
            "Low-word-first UInt32 round trip");

        const int signedValue = -123_456_789;
        ModbusRegisterCodec.WriteInt32(registers, 4, signedValue);
        AssertEqual(signedValue, ModbusRegisterCodec.ReadInt32(registers, 4), "Int32 round trip");

        const ulong wideValue = 0x1122_3344_5566_7788;
        ModbusRegisterCodec.WriteUInt64(registers, 6, wideValue);
        AssertEqual(wideValue, ModbusRegisterCodec.ReadUInt64(registers, 6), "UInt64 round trip");

        ModbusRegisterCodec.WriteUInt64(registers, 6, wideValue, ModbusWordOrder.LowWordFirst);
        AssertEqual(
            wideValue,
            ModbusRegisterCodec.ReadUInt64(registers, 6, ModbusWordOrder.LowWordFirst),
            "Low-word-first UInt64 round trip");

        AssertThrows<ArgumentOutOfRangeException>(
            () => ModbusRegisterCodec.WriteUInt32(registers, registers.Length - 1, 1),
            "Codec range validation");

        AssertThrows<ArgumentOutOfRangeException>(
            () => ModbusRegisterCodec.WriteUInt32(
                registers,
                0,
                1,
                (ModbusWordOrder)999),
            "Codec word-order validation");

        Console.WriteLine("PASS: register codec tests");
    }

    private static void RunSnapshotTests()
    {
        ushort[] source = [1, 2, 3];
        var snapshot = new ModbusRegisterSnapshot(100, source, sequence: 7);
        source[0] = 999;

        AssertEqual((ushort)1, snapshot.Registers.Span[0], "Snapshot must copy caller memory");
        AssertEqual(7L, snapshot.Sequence, "Snapshot sequence");

        AssertThrows<ArgumentException>(
            () => _ = new ModbusRegisterSnapshot(0, ReadOnlySpan<ushort>.Empty, 0),
            "Empty snapshot validation");

        AssertThrows<ArgumentOutOfRangeException>(
            () => _ = new ModbusRegisterSnapshot(ushort.MaxValue, new ushort[2], 0),
            "Snapshot address validation");

        Console.WriteLine("PASS: immutable snapshot tests");
    }

    private static void RunRobotInfoRegisterMapTests()
    {
        var updatedAt = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);
        var values = new RobotInfoRegisterValues(
            DailyRunMinutes: 1_440,
            TotalRunSeconds: 31_536_000,
            DowntimeMinutes: 525_600,
            CurrentDowntimeMinutes: 100_000,
            StatusCode: RobotInfoStatusCode.Standby,
            DailyProduction: 350,
            TotalProduction: 127_750,
            PowerOnMinutes: 525_600,
            UpdatedAtUtc: updatedAt);

        ModbusRegisterSnapshot snapshot = RobotInfoRegisterMap.CreateSnapshot(
            values,
            sequence: 0x1122_3344);
        ReadOnlySpan<ushort> registers = snapshot.Registers.Span;

        AssertEqual(RobotInfoRegisterMap.RegisterCount, registers.Length, "Robot register-map length");
        AssertEqual((ushort)0, snapshot.StartAddress, "40001 protocol address");
        AssertEqual(
            40001,
            RobotInfoRegisterMap.ToReferenceAddress(RobotInfoRegisterMap.DailyRunMinutesAddress),
            "40001 reference conversion");
        AssertEqual(
            (ushort)0,
            ModbusAddressConverter.ToProtocolAddress(40001, ModbusRegisterArea.HoldingRegisters),
            "40001 zero-based conversion");
        AssertEqual(values.DailyRunMinutes, registers[RobotInfoRegisterMap.DailyRunMinutesAddress], "Daily run minutes");
        AssertEqual(
            values.TotalRunSeconds,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.TotalRunSecondsAddress),
            "Annual run seconds must use UInt32");
        AssertEqual(
            values.DowntimeMinutes,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.DowntimeMinutesAddress),
            "Annual downtime minutes must use UInt32");
        AssertEqual(
            values.CurrentDowntimeMinutes,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.CurrentDowntimeMinutesAddress),
            "Long single downtime must use UInt32");
        AssertEqual(
            (ushort)RobotInfoStatusCode.Standby,
            registers[RobotInfoRegisterMap.StatusCodeAddress],
            "Stable SCADA status code");
        AssertEqual(
            values.TotalProduction,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.TotalProductionAddress),
            "Annual production must use UInt32");
        AssertEqual(
            values.PowerOnMinutes,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.PowerOnMinutesAddress),
            "Annual power-on minutes must use UInt32");
        AssertEqual(
            RobotInfoRegisterMap.CurrentMapVersion,
            registers[RobotInfoRegisterMap.MapVersionAddress],
            "Register-map version");
        AssertEqual(
            0x1122_3344u,
            ModbusRegisterCodec.ReadUInt32(registers, RobotInfoRegisterMap.SnapshotSequenceAddress),
            "Snapshot sequence encoding");

        Console.WriteLine("PASS: 40001 Robot_Info_Mes register-map tests");
    }

    private static async Task RunProtocolAndLifecycleTestsAsync()
    {
        int port = GetFreeTcpPort();
        var options = new ModbusTcpServerOptions
        {
            BindAddress = IPAddress.Loopback,
            Port = port,
            AcceptedUnitIdentifier = 1,
            RegisterArea = ModbusRegisterArea.HoldingRegisters,
            RegisterCount = 64,
            MaxConnections = 8,
            ConnectionTimeout = TimeSpan.FromSeconds(10),
        };

        await using var server = new FluentModbusTcpServer();
        await server.StartAsync(options);
        await server.StartAsync(options);

        ushort[] published = [0x1234, 0xABCD, 0x0001, 0xFFFF];
        server.Publish(new ModbusRegisterSnapshot(10, published, sequence: 1));

        using var client = CreateClient();
        client.Connect(new IPEndPoint(IPAddress.Loopback, port), ModbusEndianness.LittleEndian);

        ushort[] read = client.ReadHoldingRegisters<ushort>(1, 10, published.Length).ToArray();
        AssertSequenceEqual(published, read, "FC03 holding-register read");

        AssertModbusException(
            () => client.WriteSingleRegister(1, 0, (ushort)42),
            ModbusExceptionCode.IllegalFunction,
            "Write request must be rejected");

        AssertModbusException(
            () => client.ReadInputRegisters(1, 0, 1),
            ModbusExceptionCode.IllegalFunction,
            "FC04 must be rejected in 40001/FC03 mode");

        AssertModbusException(
            () => client.ReadHoldingRegisters(1, 63, 2),
            ModbusExceptionCode.IllegalDataAddress,
            "Out-of-range read must be rejected");

        ModbusTcpServerStatus status = server.GetStatus();
        AssertEqual(ModbusServerState.Running, status.State, "Running status");
        AssertTrue(status.TotalRequests >= 4, "Request counter must include accepted and rejected requests");
        AssertEqual(3L, status.RejectedRequests, "Rejected request counter");
        AssertEqual(1L, status.PublishedSnapshots, "Published snapshot counter");

        client.Disconnect();
        await server.StopAsync();
        await server.StopAsync();
        AssertEqual(ModbusServerState.Stopped, server.GetStatus().State, "Stopped status");

        await server.StartAsync(options);
        AssertEqual(ModbusServerState.Running, server.GetStatus().State, "Restart status");
        await server.StopAsync();

        ModbusTcpServerOptions inputRegisterOptions = options with
        {
            RegisterArea = ModbusRegisterArea.InputRegisters,
        };
        await server.StartAsync(inputRegisterOptions);
        server.Publish(new ModbusRegisterSnapshot(10, published, sequence: 2));
        using var inputClient = CreateClient();
        inputClient.Connect(new IPEndPoint(IPAddress.Loopback, port), ModbusEndianness.LittleEndian);
        ushort[] inputRead = inputClient
            .ReadInputRegisters<ushort>(1, 10, published.Length)
            .ToArray();
        AssertSequenceEqual(published, inputRead, "Configurable FC04 input-register read");
        await server.StopAsync();

        Console.WriteLine("PASS: FC03/40001, optional FC04, read-only and lifecycle integration tests");
    }

    private static async Task RunConcurrentStressTestAsync(StressSettings settings)
    {
        int port = GetFreeTcpPort();
        var options = new ModbusTcpServerOptions
        {
            BindAddress = IPAddress.Loopback,
            Port = port,
            AcceptedUnitIdentifier = 1,
            RegisterArea = ModbusRegisterArea.HoldingRegisters,
            RegisterCount = 256,
            MaxConnections = settings.ClientCount + 16,
            ConnectionTimeout = TimeSpan.FromSeconds(30),
            DiagnosticHistoryCapacity = 1000,
        };

        await using var server = new FluentModbusTcpServer();
        await server.StartAsync(options);

        long sequence = 1;
        server.Publish(CreateStressSnapshot(sequence, settings.RegisterCount));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(settings.TimeoutSeconds));
        var failureSamples = new ConcurrentQueue<string>();
        long failureCount = 0;
        long completedReads = 0;
        int peakConnections = 0;
        var clients = new ModbusTcpClient?[settings.ClientCount];
        var workloadStart = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        Task? workloadTask = null;
        string phase = "client connection";

        void RecordFailure(string message)
        {
            long count = Interlocked.Increment(ref failureCount);
            if (count <= 100)
            {
                failureSamples.Enqueue(message);
            }
        }

        var endpoint = new IPEndPoint(IPAddress.Loopback, port);
        var totalStopwatch = Stopwatch.StartNew();

        async Task ConnectClientAsync(int clientIndex)
        {
            var tcpClient = new TcpClient();
            ModbusTcpClient? modbusClient = null;

            try
            {
                await tcpClient.ConnectAsync(
                        endpoint.Address,
                        endpoint.Port,
                        timeout.Token)
                    .ConfigureAwait(false);

                modbusClient = CreateClient();
                modbusClient.Initialize(tcpClient, ModbusEndianness.LittleEndian);
                clients[clientIndex] = modbusClient;
            }
            catch (OperationCanceledException) when (timeout.IsCancellationRequested)
            {
                modbusClient?.Dispose();
                tcpClient.Dispose();
                throw;
            }
            catch (Exception exception)
            {
                modbusClient?.Dispose();
                tcpClient.Dispose();
                RecordFailure($"client {clientIndex} connect: {exception.GetType().Name}: {exception.Message}");
            }
        }

        async Task ReadClientAsync(ModbusTcpClient client, int clientIndex)
        {
            await workloadStart.Task.WaitAsync(timeout.Token).ConfigureAwait(false);

            for (int readIndex = 0; readIndex < settings.ReadsPerClient; readIndex++)
            {
                timeout.Token.ThrowIfCancellationRequested();
                try
                {
                    Memory<ushort> response = await client.ReadHoldingRegistersAsync<ushort>(
                            1,
                            0,
                            settings.RegisterCount,
                            timeout.Token)
                        .ConfigureAwait(false);

                    if (!ValidateStressResponse(response.Span, settings.RegisterCount, out string? error))
                    {
                        RecordFailure($"client {clientIndex}, read {readIndex}: {error}");
                    }

                    Interlocked.Increment(ref completedReads);
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    RecordFailure($"client {clientIndex}, read {readIndex}: {exception.GetType().Name}: {exception.Message}");
                    break;
                }
            }
        }

        async Task MonitorConnectionsAsync()
        {
            try
            {
                while (!timeout.IsCancellationRequested)
                {
                    int connections = server.GetStatus().ConnectedClients;
                    UpdateMaximum(ref peakConnections, connections);
                    await Task.Delay(5, timeout.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // 测试完成后主动取消连接监控，属于正常结束路径。
            }
        }

        Task monitorTask = MonitorConnectionsAsync();
        TimeSpan connectionElapsed = TimeSpan.Zero;
        var workloadStopwatch = new Stopwatch();

        try
        {
            Task[] connectionTasks = Enumerable.Range(0, settings.ClientCount)
                .Select(ConnectClientAsync)
                .ToArray();

            await Task.WhenAll(connectionTasks).WaitAsync(timeout.Token).ConfigureAwait(false);

            if (clients.Any(client => client is null))
            {
                string samples = string.Join(Environment.NewLine, failureSamples.Select(value => "  - " + value));
                throw new InvalidOperationException(
                    $"Only {clients.Count(client => client is not null)}/{settings.ClientCount} stress clients connected." +
                    Environment.NewLine + samples);
            }

            phase = "server connection acceptance";
            var acceptanceStopwatch = Stopwatch.StartNew();
            int acceptedClients;
            do
            {
                acceptedClients = server.GetStatus().ConnectedClients;
                UpdateMaximum(ref peakConnections, acceptedClients);
                if (acceptedClients >= settings.ClientCount)
                {
                    break;
                }

                await Task.Delay(5, timeout.Token).ConfigureAwait(false);
            }
            while (acceptanceStopwatch.Elapsed < TimeSpan.FromSeconds(10));

            if (acceptedClients < settings.ClientCount)
            {
                throw new InvalidOperationException(
                    $"Server accepted only {acceptedClients}/{settings.ClientCount} connected stress clients within 10 seconds.");
            }

            connectionElapsed = totalStopwatch.Elapsed;
            phase = "read/publish workload";
            Task[] readerTasks = clients
                .Select((client, clientIndex) => ReadClientAsync(client!, clientIndex))
                .ToArray();

            Task[] publisherTasks = Enumerable.Range(0, settings.PublisherCount)
                .Select(_ => Task.Run(async () =>
                {
                    await workloadStart.Task.WaitAsync(timeout.Token).ConfigureAwait(false);

                    for (int publishIndex = 0; publishIndex < settings.PublishesPerPublisher; publishIndex++)
                    {
                        timeout.Token.ThrowIfCancellationRequested();
                        long next = Interlocked.Increment(ref sequence);
                        server.Publish(CreateStressSnapshot(next, settings.RegisterCount));
                    }
                }, timeout.Token))
                .ToArray();

            workloadStopwatch.Start();
            workloadTask = Task.WhenAll(readerTasks.Concat(publisherTasks));
            workloadStart.TrySetResult(true);
            await workloadTask.WaitAsync(timeout.Token).ConfigureAwait(false);
            workloadStopwatch.Stop();
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            throw new TimeoutException(
                $"Stress test exceeded {settings.TimeoutSeconds}s during {phase}; " +
                $"completedReads={Volatile.Read(ref completedReads):N0}.");
        }
        finally
        {
            timeout.Cancel();
            workloadStart.TrySetCanceled();

            foreach (ModbusTcpClient? client in clients)
            {
                client?.Dispose();
            }

            if (workloadTask is not null && !workloadTask.IsCompleted)
            {
                try
                {
                    await workloadTask.WaitAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false);
                }
                catch
                {
                    // The original timeout or workload result is reported below.
                }
            }

            await monitorTask.ConfigureAwait(false);
            totalStopwatch.Stop();
        }

        long expectedReads = (long)settings.ClientCount * settings.ReadsPerClient;
        long expectedPublishes = 1L + (long)settings.PublisherCount * settings.PublishesPerPublisher;

        using var verificationClient = CreateClient();
        verificationClient.Connect(endpoint, ModbusEndianness.LittleEndian);
        ushort[] finalSnapshot = verificationClient
            .ReadHoldingRegisters<ushort>(1, 0, settings.RegisterCount)
            .ToArray();
        ModbusTcpServerStatus status = server.GetStatus();
        ModbusPublishInfo lastPublish = status.LastPublish ??
            throw new InvalidOperationException("The server did not retain last-publish metadata.");

        if (!ValidateStressResponse(finalSnapshot, settings.RegisterCount, out string? finalError))
        {
            throw new InvalidOperationException("Final published snapshot is invalid: " + finalError);
        }

        AssertEqual(
            unchecked((ushort)lastPublish.Sequence),
            finalSnapshot[0],
            "Last-publish metadata must identify the final register snapshot");

        Console.WriteLine(
            $"STRESS: clients={settings.ClientCount}, peakConnections={peakConnections}, " +
            $"reads={completedReads:N0}/{expectedReads:N0}, publishes={status.PublishedSnapshots:N0}/{expectedPublishes:N0}, " +
            $"connect={connectionElapsed.TotalSeconds:F2}s, workload={workloadStopwatch.Elapsed.TotalSeconds:F2}s, " +
            $"readsPerSecond={completedReads / Math.Max(workloadStopwatch.Elapsed.TotalSeconds, 0.001):N0}");

        if (failureCount != 0)
        {
            string samples = string.Join(Environment.NewLine, failureSamples.Select(value => "  - " + value));
            throw new InvalidOperationException($"Stress test recorded {failureCount:N0} failure(s):{Environment.NewLine}{samples}");
        }

        AssertEqual(expectedReads, completedReads, "Completed stress reads");
        AssertEqual(expectedPublishes, status.PublishedSnapshots, "Completed stress publishes");
        AssertEqual(0L, status.RejectedRequests, "Stress requests must not be rejected");
        AssertTrue(status.TotalRequests >= expectedReads, "Server request counter must cover all stress reads");
        AssertTrue(peakConnections >= Math.Min(settings.ClientCount, 2), "Stress test must establish concurrent connections");

        await server.StopAsync();
        Console.WriteLine("PASS: concurrent readers plus concurrent atomic publishers");
    }

    private static ModbusRegisterSnapshot CreateStressSnapshot(long sequence, int registerCount)
    {
        ushort version = unchecked((ushort)sequence);
        ushort[] values = new ushort[registerCount];
        values[0] = version;

        for (int index = 1; index < registerCount - 1; index++)
        {
            values[index] = StressPattern(version, index);
        }

        values[^1] = version;
        return new ModbusRegisterSnapshot(0, values, sequence);
    }

    private static bool ValidateStressResponse(
        ReadOnlySpan<ushort> response,
        int registerCount,
        out string? error)
    {
        if (response.Length != registerCount)
        {
            error = $"response length {response.Length}, expected {registerCount}";
            return false;
        }

        ushort version = response[0];
        ushort endingVersion = response[^1];
        if (version != endingVersion)
        {
            error = $"torn snapshot: begin={version}, end={endingVersion}";
            return false;
        }

        for (int index = 1; index < registerCount - 1; index++)
        {
            ushort actual = response[index];
            ushort expected = StressPattern(version, index);
            if (actual != expected)
            {
                error = $"torn/corrupt register {index}: version={version}, actual=0x{actual:X4}, expected=0x{expected:X4}";
                return false;
            }
        }

        error = null;
        return true;
    }

    private static ushort StressPattern(ushort version, int index) =>
        unchecked((ushort)(version ^ (index * 257) ^ 0xA55A));

    private static ModbusTcpClient CreateClient() => new()
    {
        ConnectTimeout = 10_000,
        ReadTimeout = 10_000,
        WriteTimeout = 10_000,
    };

    private static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    private static void UpdateMaximum(ref int target, int value)
    {
        int observed;
        while (value > (observed = Volatile.Read(ref target)) &&
               Interlocked.CompareExchange(ref target, value, observed) != observed)
        {
        }
    }

    private static void AssertTrue(bool condition, string description)
    {
        if (!condition)
        {
            throw new InvalidOperationException("Assertion failed: " + description);
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string description)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException(
                $"Assertion failed: {description}. Expected={expected}; Actual={actual}.");
        }
    }

    private static void AssertSequenceEqual(
        ReadOnlySpan<ushort> expected,
        ReadOnlySpan<ushort> actual,
        string description)
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new InvalidOperationException(
                $"Assertion failed: {description}. Expected=[{string.Join(',', expected.ToArray())}]; Actual=[{string.Join(',', actual.ToArray())}].");
        }
    }

    private static void AssertThrows<TException>(Action action, string description)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Assertion failed: {description}. Expected exception {typeof(TException).Name}.");
    }

    private static void AssertModbusException(
        Action action,
        ModbusExceptionCode expectedCode,
        string description)
    {
        try
        {
            action();
        }
        catch (ModbusException exception) when (exception.ExceptionCode == expectedCode)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Assertion failed: {description}. Expected Modbus exception {expectedCode}.");
    }
}

internal sealed record StressSettings(
    int ClientCount,
    int ReadsPerClient,
    int PublisherCount,
    int PublishesPerPublisher,
    int RegisterCount,
    int TimeoutSeconds,
    bool ShowHelp)
{
    public static StressSettings Parse(string[] args)
    {
        var result = new StressSettings(
            ClientCount: 64,
            ReadsPerClient: 1000,
            PublisherCount: 4,
            PublishesPerPublisher: 10_000,
            RegisterCount: 32,
            TimeoutSeconds: 180,
            ShowHelp: false);

        for (int index = 0; index < args.Length; index++)
        {
            string argument = args[index];
            if (argument is "--help" or "-h")
            {
                result = result with { ShowHelp = true };
                continue;
            }

            if (argument == "--quick")
            {
                result = result with
                {
                    ClientCount = 8,
                    ReadsPerClient = 100,
                    PublisherCount = 2,
                    PublishesPerPublisher = 500,
                };
                continue;
            }

            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Missing value for '{argument}'.");
            }

            int value = int.Parse(args[++index], System.Globalization.CultureInfo.InvariantCulture);
            result = argument switch
            {
                "--clients" => result with { ClientCount = InRange(value, 1, 512, argument) },
                "--reads" => result with { ReadsPerClient = InRange(value, 1, 10_000_000, argument) },
                "--publishers" => result with { PublisherCount = InRange(value, 1, 64, argument) },
                "--publishes" => result with { PublishesPerPublisher = InRange(value, 1, 10_000_000, argument) },
                "--registers" => result with { RegisterCount = InRange(value, 4, 125, argument) },
                "--timeout-seconds" => result with { TimeoutSeconds = InRange(value, 5, 3600, argument) },
                _ => throw new ArgumentException($"Unknown argument '{argument}'. Use --help for usage."),
            };
        }

        return result;
    }

    public static void PrintHelp()
    {
        Console.WriteLine("HanGao.ModbusTcp.Tests options:");
        Console.WriteLine("  --quick                 Run a short smoke test");
        Console.WriteLine("  --clients N             Concurrent TCP clients (1..512, default 64)");
        Console.WriteLine("  --reads N               FC03 reads per client (default 1000)");
        Console.WriteLine("  --publishers N          Concurrent snapshot publishers (default 4)");
        Console.WriteLine("  --publishes N           Publishes per publisher (default 10000)");
        Console.WriteLine("  --registers N           Registers per atomic block (4..125, default 32)");
        Console.WriteLine("  --timeout-seconds N     Whole stress-test timeout (default 180)");
    }

    private static int InRange(int value, int minimum, int maximum, string name)
    {
        if (value < minimum || value > maximum)
        {
            throw new ArgumentOutOfRangeException(name, value, $"Value must be between {minimum} and {maximum}.");
        }

        return value;
    }
}
