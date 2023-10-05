using NModbus;
using PropertyChanged;
using System;
using System.Net.Sockets;
using System.Text;
using static TCP_Modbus.Modbus_Data;

namespace TCP_Modbus
{
    [AddINotifyPropertyChangedInterface]
    public class Main_Modbus
    {
        public Main_Modbus()
        {



        }


        private ModbusFactory modbusFactory;
        private IModbusMaster master;
        private TcpClient tcpClient = new TcpClient();


        /// <summary>
        /// Modbus_设备IP号
        /// </summary>
        public string IPAdress { set; get; }
        /// <summary>
        /// Modbus_端口号
        /// </summary>
        public int Port { set; get; }

        /// <summary>
        /// Modbus读写类型
        /// </summary>
        public Modbus_Val_Type_Enum VariableType { set; get; }

        /// <summary>
        /// Modbus写入值
        /// </summary>
        public string WriteValue { set; get; }


        /// <summary>
        /// Modbus写入地址
        /// </summary>
        public ushort WriteAddress { set; get; }

        /// <summary>
        /// Modbus读取值
        /// </summary>
        public string ReadValue { set; get; }

        /// <summary>
        /// Modbus读取地址
        /// </summary>
        public ushort ReadAddress { set; get; }

        /// <summary>
        /// Modbus设备号数
        /// </summary>
        public byte SlaveID { set; get; }

        /// <summary>
        /// Modbus设备连接状态
        /// </summary>
        public bool Connected
        {
            get => tcpClient.Connected;
        }

        /// <summary>
        /// Modbus连接方法
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Ini_ModBusTCP()
        {


            modbusFactory = new ModbusFactory();
            tcpClient = new TcpClient(IPAdress, Port);
            master = modbusFactory.CreateMaster(tcpClient);
            master.Transport.ReadTimeout = 2000;
            master.Transport.Retries = 10;

        }


        /// <summary>
        /// Modbus读取方法
        /// </summary>
        public void ReadExecute()
        {
            try
            {


                ushort[] buff;
                bool[] Coils;
                float value;
                switch (VariableType)
                {
                    case Modbus_Val_Type_Enum.Real:
                        buff = ReadHoldingRegisters(SlaveID, ReadAddress, 2);
                        value = GetReal(buff, 0);
                        ReadValue = value.ToString();

                        break;
                    case Modbus_Val_Type_Enum.String:

                        buff = ReadHoldingRegisters(SlaveID, ReadAddress, 10);
                        ReadValue = GetString(buff, 0, 10);

                        break;
                    case Modbus_Val_Type_Enum.Int16:

                        buff = ReadHoldingRegisters(SlaveID, ReadAddress, 1);
                        value = GetShort(buff, 0);
                        ReadValue = value.ToString();

                        break;
                    case Modbus_Val_Type_Enum.Bool:

                        Coils = ReadCoils(SlaveID, ReadAddress, 1);
                        ReadValue = Coils[0].ToString();
                        break;


                }

            }
            catch (Exception )
            {



            }



        }


        /// <summary>
        /// Modbus写入方法
        /// </summary>
        public void WriteExecute()
        {

            try
            {

                switch (VariableType)
                {
                    case Modbus_Val_Type_Enum.Real:
                        {
                            ushort[] buff = new ushort[2];
                            float value = float.Parse(WriteValue);
                            SetReal(buff, 0, value);
                            WriteMultipleRegisters(SlaveID, WriteAddress, buff);
                        }
                        break;
                    case Modbus_Val_Type_Enum.String:
                        {

                        ushort[] buff = new ushort[10];
                        SetString(buff, 0, WriteValue);
                        WriteMultipleRegisters(SlaveID, WriteAddress, buff);
                        }
                        break;
                    case Modbus_Val_Type_Enum.Int16:
                        {

                        ushort[] buff = new ushort[1];
                        short value = short.Parse(WriteValue);
                        Modbus_Data.SetShort(buff, 0, value);
                        WriteMultipleRegisters(SlaveID, WriteAddress, buff);
                        }
                        break;
                    case Modbus_Val_Type_Enum.Bool:

                        {
                            bool[] buff = new bool[1];
                            buff[0] = bool.Parse(WriteValue);
                            //short value = short.Parse(WriteValue);
                            //Modbus_Data.GetBools();
                            WriteMultipleCoils(SlaveID, WriteAddress, buff);


                        }

                        break;

                }




            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 读取线圈
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool[] ReadCoils(byte slaveAddress, ushort startAddress, ushort num)
        {
            return master.ReadCoils(slaveAddress, startAddress, num);
        }

        /// <summary>
        /// 读取输入
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool[] ReadInputs(byte slaveAddress, ushort startAddress, ushort num)
        {
            return master.ReadInputs(slaveAddress, startAddress, num);
        }

        /// <summary>
        /// 读取保持寄存器
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort startAddress, ushort num)
        {
            return master.ReadHoldingRegisters(slaveAddress, startAddress, num);
        }

        /// <summary>
        /// 读取输入寄存器
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort startAddress, ushort num)
        {
            return master.ReadInputRegisters(slaveAddress, startAddress, num);
        }

        /// <summary>
        /// 写入单线圈
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="value"></param>
        public void WriteSingleCoil(byte slaveAddress, ushort startAddress, bool value)
        {
            master.WriteSingleCoil(slaveAddress, startAddress, value);
        }

        /// <summary>
        /// 写入单寄存器
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="value"></param>
        public void WriteSingleRegister(byte slaveAddress, ushort startAddress, ushort value)
        {
            master.WriteSingleRegister(slaveAddress, startAddress, value);
        }

        /// <summary>
        /// 写入多个线圈
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="value"></param>
        public void WriteMultipleCoils(byte slaveAddress, ushort startAddress, bool[] value)
        {
            master.WriteMultipleCoils(slaveAddress, startAddress, value);
        }

        /// <summary>
        /// 写入多个寄存器
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="startAddress"></param>
        /// <param name="value"></param>
        public void WriteMultipleRegisters(byte slaveAddress, ushort startAddress, ushort[] value)
        {
            master.WriteMultipleRegisters(slaveAddress, startAddress, value);
        }

    }

    /// <summary>
    /// Modbus值类型
    /// </summary>
    public enum Modbus_Val_Type_Enum
    {
        Real,
        String,
        Int16,
        Bool,
    }

    [AddINotifyPropertyChangedInterface]
    public class Modbus_Data
    {





        /// <summary>
        /// 赋值string
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetString(ushort[] src, int start, string value)
        {
            byte[] bytesTemp = Encoding.UTF8.GetBytes(value);
            ushort[] dest = Bytes2Ushorts(bytesTemp);
            dest.CopyTo(src, start);
        }

        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetString(ushort[] src, int start, int len)
        {
            ushort[] temp = new ushort[len];
            for (int i = 0; i < len; i++)
            {
                temp[i] = src[i + start];
            }
            byte[] bytesTemp = Ushorts2Bytes(temp);
            string res = Encoding.UTF8.GetString(bytesTemp).Trim(new char[] { '\0' });
            return res;
        }

        /// <summary>
        /// 赋值Real类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetReal(ushort[] src, int start, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            ushort[] dest = Bytes2Ushorts(bytes);

            dest.CopyTo(src, start);
        }

        /// <summary>
        /// 获取float类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static float GetReal(ushort[] src, int start)
        {
            ushort[] temp = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                temp[i] = src[i + start];
            }
            byte[] bytesTemp = Ushorts2Bytes(temp);
            float res = BitConverter.ToSingle(bytesTemp, 0);
            return res;
        }

        /// <summary>
        /// 赋值Short类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetShort(ushort[] src, int start, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            ushort[] dest = Bytes2Ushorts(bytes);

            dest.CopyTo(src, start);
        }

        /// <summary>
        /// 获取short类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static short GetShort(ushort[] src, int start)
        {
            ushort[] temp = new ushort[1];
            temp[0] = src[start];
            byte[] bytesTemp = Ushorts2Bytes(temp);
            short res = BitConverter.ToInt16(bytesTemp, 0);
            return res;
        }

        /// <summary>
        ///  获取Bool类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool[] GetBools(ushort[] src, int start, int num)
        {
            ushort[] temp = new ushort[num];
            for (int i = start; i < start + num; i++)
            {
                temp[i] = src[i + start];
            }
            byte[] bytes = Ushorts2Bytes(temp);

            bool[] res = Bytes2Bools(bytes);

            return res;
        }



        public static bool[] Bytes2Bools(byte[] b)
        {
            bool[] array = new bool[8 * b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    array[i * 8 + j] = (b[i] & 1) == 1;//判定byte的最后一位是否为1，若为1，则是true；否则是false
                    b[i] = (byte)(b[i] >> 1);//将byte右移一位
                }
            }
            return array;
        }

        public static byte Bools2Byte(bool[] array)
        {
            if (array != null && array.Length > 0)
            {
                byte b = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (array[i])
                    {
                        byte nn = (byte)(1 << i);//左移一位，相当于×2
                        b += nn;
                    }
                }
                return b;
            }
            return 0;
        }

        public static ushort[] Bytes2Ushorts(byte[] src, bool reverse = false)
        {
            int len = src.Length;

            byte[] srcPlus = new byte[len + 1];
            src.CopyTo(srcPlus, 0);
            int count = len >> 1;

            if (len % 2 != 0)
            {
                count += 1;
            }

            ushort[] dest = new ushort[count];
            if (reverse)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = (ushort)(srcPlus[i * 2] << 8 | srcPlus[2 * i + 1] & 0xff);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = (ushort)(srcPlus[i * 2] & 0xff | srcPlus[2 * i + 1] << 8);
                }
            }

            return dest;
        }

        public static byte[] Ushorts2Bytes(ushort[] src, bool reverse = false)
        {

            int count = src.Length;
            byte[] dest = new byte[count << 1];
            if (reverse)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i * 2] = (byte)(src[i] >> 8);
                    dest[i * 2 + 1] = (byte)(src[i] >> 0);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i * 2] = (byte)(src[i] >> 0);
                    dest[i * 2 + 1] = (byte)(src[i] >> 8);
                }
            }
            return dest;
        }




    }
}