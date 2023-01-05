using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace RegisterControls
{
    public enum BindingType
    {
        Text,
        Int,
        Real,
        Boolean
    }

    public class Device
    {
        #region CONST_REGION
        private readonly UInt16[] Bits = 
        {
            0x0002,
            0x0004,
            0x0008,
            0x0010,
            0x0020,
            0x0040,
            0x0080
        };
        const int Changes = 5;
        #endregion

        #region Varialbes Region
        List<Register> Registers;
        List<BindingSource> GeneralBindings;
        List<GridViewBinder> GridList;
        Dictionary<int, BindingSource> BindingIndex;
        InterfaceClass IO;
        private Thread UpdateFormThread;
        private Boolean Suspend = false;
        private Queue<KeyValue> KVList;
        System.Windows.Forms.Button Control;
        int Tries = 0;
        #endregion

        #region Initialization and Setup
        //----------------------------------------------------------------------
        //
        //
        public Device(ref List<Register> RegisterList)
        {
            RegisterList = Registers = new List<Register>();
            BindingIndex = new Dictionary<int, BindingSource>();
            GeneralBindings = new List<BindingSource>();
            KVList = new Queue<KeyValue>();
            Walking = false;
        }

        //----------------------------------------------------------------------
        //
        //
        public Device()
        {
            BindingIndex = new Dictionary<int, BindingSource>();
            GeneralBindings = new List<BindingSource>();
            KVList = new Queue<KeyValue>();
            Walking = false;
        }

        #endregion

        #region Device Threading
        //----------------------------------------------------------------------
        //
        //
        internal void UpdateBindings()
        {
            // Update output data to the controller first
            while(KVList.Count > 0)
            {
                UInt16 Rst;
                KeyValue Pair = (KeyValue)KVList.Dequeue();
                Tries = 0;
                //IO.WriteDevice(Pair.Address, Pair.Value);
                do
                {
                    IO.WriteDevice(Pair.Address, Pair.Value);
                    Rst = IO.Read(Pair.Address);
                } while ((Rst != Pair.Value) && (Tries++ < Changes));
                Rst = 0;
            }

            // Read everything which changed in from the controller
            if (IO.Update(ref Registers))
            {
                foreach (GridViewBinder Gvb in GridList)
                    Gvb.ResetBindings(false);
                foreach (BindingSource BindSource in GeneralBindings)
                    BindSource.ResetBindings(false);
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public static Boolean Walking { get; set; }

        //----------------------------------------------------------------------
        //
        //
        public void Start()
        {
            UpdateFormThread = new Thread(new ThreadStart(RunThread));
            UpdateFormThread.Name = "UpDater";
            UpdateFormThread.Start();
        }

        //----------------------------------------------------------------------
        //
        //
        // delegate that allows method DisplayCharacter to be called
        // in the thread that creates and maintains the GUI       
        internal delegate void MainFormDisplayDelegate();

        //----------------------------------------------------------------------
        //
        //
        // change the suspended/running state
        public void MutexThread(Boolean NewState)
        {
            Suspend = NewState;

            lock (this) // obtain lock
            {
                if (!Suspend) // if thread resumed
                    Monitor.Pulse(this);
            } // end lock
        }

        //----------------------------------------------------------------------
        //
        //
        public void RunThread()
        {
            Boolean LoopingForever = true;

            while (LoopingForever) // Never end
            {
                lock (this) // obtain lock
                {
                    while (Suspend) // loop until not suspended
                    {
                        Monitor.Wait(this); // suspend thread execution
                    } // end while
                } // end lock

                try
                {
                    MainFormDisplayDelegate UpdateGUI = new MainFormDisplayDelegate(UpdateBindings);
                    try
                    {
                        Walking = true;
                        Control.Invoke(UpdateGUI, new object[] { });
                        Walking = false;
                    }
                    catch (System.Exception excep)
                    {
                        MessageBox.Show(excep.Message);
                    }
                }
                catch (System.Exception excep)
                {
                    MessageBox.Show(excep.Message);
                }

                Thread.Sleep(200);
            } // end while
        }
        #endregion

        //----------------------------------------------------------------------
        //
        //
        public void Add(ref List<BindingSource> TheList)
        {
            GeneralBindings = TheList;
        }

        //----------------------------------------------------------------------
        //
        //
        public void Add(ref List<GridViewBinder> Gl)
        {
            GridList = Gl;
        }

        //----------------------------------------------------------------------
        //
        //
        public void OpenComPort(String Port, int Baud) 
        {
            IO = new InterfaceClass(Port, Baud);
        }

        //----------------------------------------------------------------------
        //
        //
        public void LoadRegisterSet()
        {
            System.IO.StreamReader InputSteam;
            InputSteam = System.IO.File.OpenText(MemDefs.IniFile);
            char[] delimit = new char[] { ',', '\n' };
            String LineInput;
            String Temp;
            int Index = 0;

            // put the different parameters into Mem Map
            do
            {
                LineInput = InputSteam.ReadLine();
                if (LineInput != null) // Not Null
                { // And not a comment
                    if (!LineInput.StartsWith("; "))
                    {  // Split input into separate Strings
                        String[] Columns = LineInput.Split(delimit);

                        // Allocate space for a new register
                        Register r = new Register();
                        r.Name = Columns[0].Trim();
                        r.Address = Convert.ToInt16(Columns[1].Trim());
                        r.Contents = Convert.ToUInt16(Columns[2]);

                        Temp = Columns[3].Trim();
                        r.CanChange = (Temp == "Yes");

                        Temp = Columns[4].Trim();
                        r.ROM = (Temp == "Yes");

                        BindRegister(ref r);

                        foreach (GridViewBinder Gc in GridList)
                        {
                            if (Gc.InRange(r.Address))
                                Gc.Add(ref r);
                        }

                        // Add the new register into the array of registers
                        Registers.Add(r);
                        r = null;
                        Index++;
                    }
                }
            } while (LineInput != null);
            InputSteam.Close();
        }

        //----------------------------------------------------------------------
        //
        //
        public void LoadRegisterSet(ref List<GridViewBinder> GvList)
        {
            System.IO.StreamReader InputSteam;
            InputSteam = System.IO.File.OpenText(MemDefs.IniFile);
            char[] delimit = new char[] { ',', '\n' };
            String LineInput;
            String Temp;
            int Index = 0;

            GridList = GvList;
            // put the different parameters into Mem Map
            do
            {
                LineInput = InputSteam.ReadLine();
                if (LineInput != null) // Not Null
                { // And not a comment
                    if (!LineInput.StartsWith("; "))
                    {  // Split input into separate Strings
                        String[] Columns = LineInput.Split(delimit);

                        // Allocate space for a new register
                        Register r = new Register();
                        r.Name = Columns[0].Trim();
                        r.Address = Convert.ToInt16(Columns[1].Trim());
                        r.Contents = Convert.ToUInt16(Columns[2]);

                        Temp = Columns[3].Trim();
                        r.CanChange = (Temp == "Yes");

                        Temp = Columns[4].Trim();
                        r.ROM = (Temp == "Yes");

                        BindRegister(ref r);

                        foreach (GridViewBinder Gc in GridList)
                        {
                            if (Gc.InRange(r.Address))
                                Gc.Add(ref r);
                        }

                        // Add the new register into the array of registers
                        Registers.Add(r);
                        r = null;
                        Index++;
                    }
                }
            } while (LineInput != null);
            InputSteam.Close();
        }

        //----------------------------------------------------------------------
        //
        //
        public void SetForDisplay()
        {
            foreach (GridViewBinder Gc in GridList)
            {
                Gc.SetForDisplay();
            }
        }

        #region ControlsBinder
        //----------------------------------------------------------------------
        //
        //
        public void BindRegister(ref Register r)
        {
            BindingSource Context = new BindingSource();

            Context.Add(r);
            GeneralBindings.Add(Context);
            BindingIndex.Add(r.Address, Context);
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.Control Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.Label Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.TextBox Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.ListBox Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref RegisterControls.AGauge Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref RegisterControls.IOBox8Bit Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    break;
                case BindingType.Int:
                    IOBox8Bit Box = (IOBox8Bit)Obj;
                    Box.DataBindings.Add("Value", Context, "Contents");
                    Box.SetCallBack(AddKeyValue);
                    Box.Address = (Byte)Index;
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.Button Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.ProgressBar Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObjectBinding(ref System.Windows.Forms.RadioButton Obj, int Index, BindingType Bt)
        {
            BindingSource Context = BindingIndex[Index];
            switch (Bt)
            {
                case BindingType.Text:
                    Obj.DataBindings.Add("Text", Context, "Value");
                    break;
                case BindingType.Int:
                    Obj.DataBindings.Add("Value", Context, "Contents");
                    break;
                case BindingType.Real:
                case BindingType.Boolean:
                    break;
            }
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddObject(ref System.Windows.Forms.Button Obj)
        {
            Control = Obj;
        }

        //----------------------------------------------------------------------
        //
        //
        public void AddKeyValue(KeyValue NewPair)
        {
            if(KVList == null)
            {
                KVList = new Queue<KeyValue>();
            }
            KVList.Enqueue(NewPair);
        }
        #endregion

        #region Handle Register specific reads writes
        //----------------------------------------------------------------------
        //
        //
        public Boolean LoadTable() 
        {
            return IO.Load(ref Registers);
        }

        //----------------------------------------------------------------------
        //
        //
        public Boolean Update()
        {
            return IO.Update(ref Registers);
        }

        //----------------------------------------------------------------------
        //
        //
        public List<BindingSource> GetGeneralBindings { get { return GeneralBindings; } }
        #endregion

        //----------------------------------------------------------------------
        //
        //
        public System.Collections.IEnumerator GetRegisters() { return new RegisterEnumerator(ref Registers); }

        //----------------------------------------------------------------------
        //
        //
        public System.Collections.IEnumerator GetBindingList() { return new BlEnumerator(ref GeneralBindings); }

        #region Private Register IEnumeration Class
        // Declare an inner class that implements the IEnumerator interface. 
        private class RegisterEnumerator : System.Collections.IEnumerator
        {
            private int CurrentPosition = -1;
            private List<Register> TheList;

            //----------------------------------------------------------------------
            //
            //
            public void Reset() { CurrentPosition = 0; }

            //----------------------------------------------------------------------
            //
            //
            public RegisterEnumerator(ref List<Register> RList) { this.TheList = RList; }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a MoveNext method. 
            public Boolean MoveNext()
            {
                CurrentPosition++;
                if (CurrentPosition < TheList.Count)
                    return true;
                else
                    return false;
            }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a Current method. 
            public object Current { get { return TheList[CurrentPosition]; } }

            //----------------------------------------------------------------------
            //
            //
            public int Position { get { return CurrentPosition; } }
        }
        #endregion

        #region Private Binding List IEnumeration Class
        // Declare an inner class that implements the IEnumerator interface. 
        private class BlEnumerator : System.Collections.IEnumerator
        {
            private int CurrentPosition = -1;
            private List<BindingSource> BindingList;

            //----------------------------------------------------------------------
            //
            //
            public void Reset() { CurrentPosition = 0; }

            //----------------------------------------------------------------------
            //
            //
            public BlEnumerator(ref List<BindingSource> BlList) { this.BindingList = BlList; }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a MoveNext method. 
            public Boolean MoveNext()
            {
                CurrentPosition++;
                return (CurrentPosition < BindingList.Count);
            }

            //----------------------------------------------------------------------
            //
            //
            // The IEnumerator interface requires a Current method. 
            public object Current { get { return BindingList[CurrentPosition]; } }

            //----------------------------------------------------------------------
            //
            //
            public int Position { get { return CurrentPosition; } }
        }
        #endregion
    }
}
