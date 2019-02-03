using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkypeSignal
{
    public class SenderBase
    {
        //public abstract void SendSerialData(string Command);

        public virtual void SendSerialData(string Command)
        {

        }

        public virtual void Initialize()
        {

        }

        public void SendVisualInitSequence()
        {
            SendSerialData("2");
            System.Threading.Thread.Sleep(500);
            SendSerialData("1");
            System.Threading.Thread.Sleep(500);
            SendSerialData("9");
        }

    }
}
