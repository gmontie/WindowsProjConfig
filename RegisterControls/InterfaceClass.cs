using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace RegisterControls
{
    public class InterfaceClass
    {
        #region Constants Region
        const int TimeSlice = 5;
        const int TimeOut = 2;

        private const int COMM_BUFFER_SIZE = 256;
        const String SendAll = "#\r";
        const String SendDiff = "!\r";
        const String LoadReg = "L\r";
        const String Store = "S\r";
        const String Sync = "Y\r";

        const int MaxThreshold = 16;
        const int MinThreshold = 2;
        const int MaxTries = 63;
        const int MaxReads = 8;

        const int MaxFailCount = 10;
        const int MinFailCount = 0;

        const int MaxAttempts = 23;
        const Byte LINE_FEED = 0x0A;
        const Byte BACK_SPACE = 0x08;
        const Byte VT = 0x0B;
        const Byte FF = 0x0C;
        const Byte CR = 0x0D;
        const Byte ETB = 0x17;
        const Byte DELETE = 0x7F;
        const Byte LD = 0x4C; // 'L'
        const Byte ST = 0x53; // 'S'
        const Byte VL = 0x56; // 'V'
        const Byte EBT = 0x40;
        #endregion

        #region Type and Enumeration Region
        enum EntryStates : short
        {
            Start = 1,
            PromptChar = 2,
            Address = 3,
            HiNibble = 4,
            HiByte = 5,
            LoByte = 6,
            EndByte = 7,
            EndCap = 8,
            Accept = 9,
            Reject = 10
        }

        public enum ConnectionStates
        {
            Unconnected,
            Good,
            Poor,
            Bad,
        }

        enum IOCTLStates : short
        {
            Initial,
            Next,
            Accept,
            Reject
        }
        #endregion

        #region Variables Region
        private SerialPort CommPort;
        private int FailCount;
        #endregion

        #region Public Code Region
        //----------------------------------------------------------------------
        //
        //
        public InterfaceClass(String Port, int Baud)
        {   // Setup Communications port.
            CommPort = new SerialPort(Port, Baud);
            CommPort.Parity = Parity.None;
            CommPort.StopBits = StopBits.One;
            CommPort.DataBits = 8;
            CommPort.Handshake = Handshake.None;

            // Actually make contact
            try
            {
                CommPort.Open();
            }
            catch (System.UnauthorizedAccessException)
            {
                throw;
            }
            catch (System.IO.IOException)
            {
                throw;
            }
            CommPort.DiscardInBuffer();
        }

        //----------------------------------------------------------------------
        //
        //
        public void CloseInterface()
        {
            CommPort.Close();
        }

        //----------------------------------------------------------------------
        //
        //
        internal Boolean Load(ref List<Register> Registers)
        {
            int Errors = 0;
            Byte[] InBuf;
            Boolean Success = false;

            do
            {
                Errors += SendCommand(SendAll);
                do
                {
                    InBuf = ReadBuffer();
                    if (InBuf != null)
                    {
                        if (ReadTable(ref InBuf, ref Registers))
                            Success = true;
                        else
                        {
                            Errors++;
                            System.Threading.Thread.Sleep(360);
                        }
                    }
                } while (InBuf != null);
            } while ((!Success) && (Errors < MaxTries));
            UpdateState(Errors);
            return (Errors == 0);
        }

        //----------------------------------------------------------------------
        //
        //
        internal Boolean Update(ref List<Register> Registers)
        {
            int Errors = 0;
            Byte[] InBuf;
            Boolean Success = false;

            do
            {
                Errors += SendCommand(SendDiff);
                do
                {
                    InBuf = ReadBuffer();
                    if (InBuf != null)
                    {
                        if (ParseDiff(ref InBuf, ref Registers))
                            Success = true;
                        else
                        {
                            Errors++;
                            System.Threading.Thread.Sleep(360);
                        }
                    }
                } while (InBuf != null);
            } while ((!Success) && (Errors < MaxTries));
            UpdateState(Errors);
            return Success;
        }

        //----------------------------------------------------------------------
        //
        //
        public UInt16 Transaction(String CmdStr,ref List<Register> Registers)
        {
            int Errors = 0;
            Byte[] InBuf;
            UInt16 Results = 0;

            do
            {
                Errors += SendCommand(CmdStr);
                do
                {
                    InBuf = ReadBuffer();
                    if (InBuf != null)
                    {
                        if (ParseDiff(ref InBuf, ref Registers))
                            Results = 1;
                        else
                        {
                            Errors++;
                            System.Threading.Thread.Sleep(360);
                        }
                    }
                } while (InBuf != null);
            } while ((Results == 0) && (Errors < MaxTries));
            UpdateState(Errors);
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        private Boolean ReadTable(ref Byte[] InputBuffer, ref List<Register> Registers)
        {
            Boolean Results = true;
            List<Byte> ByteArray = new List<Byte>();
            int Place = 0;
            Byte Address = 0;
            UInt16 Value;

            foreach (Byte Bt in InputBuffer)
            {
                if (Results)
                {
                    switch (Place)
                    {
                        case 0: // 'V'
                            if ((InputBuffer[Place] == (Byte)0x56) || (InputBuffer[Place] == (Byte)0x76))
                            {
                                ByteArray.Add(Address);
                                Address++;
                            }
                            break;
                        case 1: // Value's Most Significant Byte
                        case 2: // Value's Least Sigificant Byte                            
                            ByteArray.Add(Bt);
                            break;
                        case 3: // Carrage return 
                            if (Bt == '.')
                            {
                                //Address = ;
                                Value = (UInt16)((ByteArray[1] << 8) & 0xFF00);
                                Value += ByteArray[2];
                                Registers[ByteArray[0]].Contents = Value;
                                System.Console.WriteLine("R[{0}]<={1}", ByteArray[0], Value);
                                ByteArray = new List<Byte>();
                                Place = -1; // Reset Place 
                            }
                            else
                                Results = false;
                            break;
                        case 4: // End of Transmission
                            if (Bt != EBT) // If a character was dropped
                                Results = false;
                            break;
                        default:
                            Place = 0; // Reset Place 
                            break;
                    }
                    Place++;
                }
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        private Boolean ParseDiff(ref Byte[] InputBuffer,ref List<Register> Registers)
        {
            Boolean Results = true;
            List<Byte> ByteArray = new List<Byte>();
            UInt16 Value;
            int Place = 0;

            foreach (Byte Bt in InputBuffer)
            {
                if (Results)
                {
                    switch (Place)
                    {
                        case 0: // 'A'
                            if ((Bt == 0x41) || (Bt == 0x61)) // 'A' or 'a'
                                ByteArray.Add(Bt);
                            else
                                if (Bt != EBT) // Should be the end of transmission 0x17
                                { 
                                    Results = false; // Else it's an error
                                    Place = -1;
                                }                                    
                            break;
                        case 1: // Address Byte
                            ByteArray.Add(Bt);
                            break;
                        case 2: // 'V'
                            if ((Bt == 0x56) || (Bt == 0x76)) // 'A' or 'a'
                                ByteArray.Add(Bt);
                            else
                                Results = false;
                            break;
                        case 3: // Value's Most Significant Byte
                        case 4: // Value's Least Sigificant Byte
                            ByteArray.Add(Bt);
                            break;
                        case 5: // Carrage return 
                            if (Bt == '.') 
                            {
                                Value = (UInt16)((ByteArray[3] << 8) & 0xFF00);
                                Value += ByteArray[4];
                                Registers[ByteArray[1]].Contents = Value;
                                ByteArray = new List<Byte>();
                                Place = -1; // Reset Place 
                            }
                            else
                                Results = false;
                            break;
                        case 6: // End of Transmission
                            if (Bt != EBT) // If a character was dropped
                                Results = false;
                            break;
                        default:
                            Place = -1; // Reset Place 
                            break;
                    }
                    Place++;
                }
            }
            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean Load(ref List<Register> Registerss, Byte Address)
        {   
            Byte[] Byts = { LD, Address, CR }; // 'L' + Address + '\r'
            String CommandString = System.Text.Encoding.UTF8.GetString(Byts);
            SendCommand(CommandString);
            int Errors = Transaction(CommandString, ref Registerss);
            UpdateState(Errors);
            return (Errors == 0);
        }

        //----------------------------------------------------------------------
        //
        //
        private Boolean Load(ref List<Register> Registerss, int Index)
        {   
            int Errors = 0;
            if (Index < MemDefs.MEMORY_SIZE)
            {
                Byte Address = (Byte)Index;
                Byte[] Byts = { LD, Address, CR }; // 'L' + (Byte)Index + '\r'
                String CommandString = System.Text.Encoding.UTF8.GetString(Byts);
                Errors = Transaction(CommandString, ref Registerss);
                UpdateState(Errors);
            }
            return (Errors == 0);
        }

        //----------------------------------------------------------------------
        //
        //
        public UInt16 Read(Byte Address)
        {
            int Errors = 0;
            UInt16 Results = 0;
            Boolean Continue = true;
            int Index = 0;
            List<Byte> ByteList = new List<Byte>();
            Byte[] Byts = { LD, Address, CR }; // 'L' + Address + '\r'
            String CommandString = System.Text.Encoding.UTF8.GetString(Byts);

            do
            {
                Errors += SendCommand(CommandString, 4);
                ByteList = ReadBufferList();
                if (ByteList.Count >= 4)
                {
                    while ((ByteList[Index] == EBT) && (Index < ByteList.Count)) Index++;
                    if (Index < ByteList.Count)
                    {
                        Results = (UInt16)((ByteList[1 + Index] << 8) & 0xFF00);
                        Results += ByteList[2 + Index];
                        Continue = false;
                    }
                }
                else
                {
                    ASCIIEncoding Encoding = new ASCIIEncoding();
                    String Str = Encoding.GetString(ByteList.ToArray());
                    String ErrorStr = "Error >" + Str + "<";
                    MessageBox.Show(ErrorStr, ErrorStr, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } while ((Continue) && (Errors < MaxTries));
            UpdateState(Errors);

            return Results;
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean WriteDevice(Byte Address, UInt16 Value)
        {
            Byte[] Tmp = BitConverter.GetBytes(Value);
            Byte[] Byts = { ST, Address, VL, Tmp[1], Tmp[0], CR }; // 'S' + Address + '\r'
            //String CommandString = System.Text.Encoding.UTF8.GetString(Byts);
            CommPort.DiscardInBuffer();
            CommPort.DiscardOutBuffer();
            CommPort.Write(Byts, 0, Byts.Length);

            return true;
        }
       
        //----------------------------------------------------------------------
        //
        //
        private void UpdateState(int Retries)
        {
            switch (ConnectionState)
            {
                case ConnectionStates.Unconnected:
                    break;
                case ConnectionStates.Good:
                    if (Retries > MaxThreshold)
                        FailCount += 2;
                    if (FailCount > 4)
                        ConnectionState = ConnectionStates.Poor;
                    break;
                case ConnectionStates.Poor:
                    if (Retries > MaxThreshold)
                    {
                        FailCount++;
                        if (FailCount > 8)
                            ConnectionState = ConnectionStates.Bad;
                    }
                    else
                    {
                        if (Retries < MinThreshold)
                            FailCount -= 2;
                        if (FailCount <= MinFailCount)
                            ConnectionState = ConnectionStates.Good;
                    }
                    break;
                case ConnectionStates.Bad:
                    if (Retries < MinThreshold)
                        FailCount -= 2;
                    if (FailCount <= MinFailCount)
                        ConnectionState = ConnectionStates.Poor;
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        private List<Byte> ReadBufferList()
        {
            List<Byte> ListBuff = new List<byte>(); 
            if (CommPort.BytesToRead > 0)
            {
                while (CommPort.BytesToRead > 0)
                    ListBuff.Add((Byte)CommPort.ReadByte());
            }
            return ListBuff;
        }

        //----------------------------------------------------------------------
        //
        //
        private Byte[] ReadBuffer()
        {
            Byte[] ByteArray = null;

            if (CommPort.BytesToRead > 0)
            {
                List<Byte> ListBuff = new List<byte>();
                while (CommPort.BytesToRead > 0)
                    ListBuff.Add((Byte)CommPort.ReadByte());

                if (ListBuff.Count > 0)
                    ByteArray = ListBuff.ToArray();
            }
            return ByteArray;
        }

        //----------------------------------------------------------------------
        //
        //
        private int SendCommand(String Command, int ExpectedBytes)
        {
            int Retries = 0;

            do
            {
                CommPort.DiscardInBuffer();
                CommPort.DiscardOutBuffer();
                CommPort.WriteLine(Command);
                System.Threading.Thread.Sleep(100);
                if (CommPort.BytesToRead < ExpectedBytes)
                    Retries++;
            } while ((Retries < MaxReads) && (CommPort.BytesToRead < ExpectedBytes));

            return Retries;
        }

        //----------------------------------------------------------------------
        //
        //
        private int SendCommand(String Command)
        {
            int Retries = 0;

            do
            {
                CommPort.DiscardInBuffer();
                CommPort.DiscardOutBuffer();
                CommPort.WriteLine(Command);
                System.Threading.Thread.Sleep(85);
                if (CommPort.BytesToRead < 6)
                    Retries++;
            } while ((Retries < MaxReads) && (CommPort.BytesToRead < 6));

            return Retries;
        }

        //----------------------------------------------------------------------
        //
        //
        public ConnectionStates ConnectionState { get; private set; }
        #endregion
    }
}
