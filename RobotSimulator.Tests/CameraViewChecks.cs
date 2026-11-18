using RobotSimulator.ViewModels;

/// <summary>检查切换标准视角后页面环绕参数被清零。</summary>
internal static class CameraViewChecks
{
    public static void Run(MainViewModel viewModel)
    {
        var originalSpeed = viewModel.OrbitSpeed;
        try
        {
            foreach (var command in new[] { viewModel.FrontCommand, viewModel.TopCommand, viewModel.IsometricCommand })
            {
                viewModel.OrbitSpeed = 15;
                command.Execute(null);
                Check(viewModel.OrbitSpeed == 0, "切换标准视角后，页面环绕速度仍不为 0。");
            }
        }
        finally { viewModel.OrbitSpeed = originalSpeed; }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
