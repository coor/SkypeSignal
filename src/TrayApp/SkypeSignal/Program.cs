using System;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;

namespace SkypeSignal
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        public enum ConnectionType
        {
            None = 0,
            COM,
            BT
        }

        public static ConnectionType getDeviceType()
        {
            return (ConnectionType)int.Parse(ConfigurationManager.AppSettings["ConnectionType"] ?? "0");
        }

        public static SenderBase getSender()
        {
            if (getDeviceType() == ConnectionType.COM) // 1
                return new SerialSender();

            if (getDeviceType() == ConnectionType.BT) // 2
                return new BTSender();
            
            return new SenderBase(); // 0
        }

        [STAThread]
        static void Main()
        {      

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //I've found that my Arduino hangs on the very first connection to work around that
            //kick-off connection to Light before we start and send a non-event code:

            SenderBase senderBase = getSender();

            senderBase.Initialize(); 
            
            var skypeStatus = new SkypeStatusInfo();

            var skypeStatusMonitor = new Thread(skypeStatus.StatusSetup);
            
            skypeStatusMonitor.Start();  
            
            //Show the System Tray Icon
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.DisplayIcon();
                
                //Make Sure the Application Runs
                Application.Run();
            }           
        }
    }
}
