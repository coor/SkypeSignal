using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Configuration;
using System.IO;

namespace SkypeSignal
{
    class BTSender : SenderBase
    {

        private BluetoothClient _cli;
        private bool _isConnected = false;
        private BluetoothDeviceInfo device = null;
        private BluetoothEndPoint ep = null;
        private Stream strm = null;

        public override void Initialize()
        {

            device = null;

            if (strm != null)
                strm.Close();

            strm = null;

            _isConnected = false;

            string strAddress = ConfigurationManager.AppSettings["BTAddress"] ?? "CACA52BF713C";
            byte[] bAddress = Enumerable.Range(0, strAddress.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(strAddress.Substring(x, 2), 16))
                             .ToArray();


            BluetoothAddress btAddress =  new BluetoothAddress ( bAddress ); 

            var serviceClass = BluetoothService.RFCommProtocol; //.SerialPort;
            if (_cli != null)
            {
                _cli.Close();
            }

            _cli = new BluetoothClient();
            var bluetoothDeviceInfos = _cli.DiscoverDevices();
            var deviceInfos = bluetoothDeviceInfos.ToList();
            
            foreach (var bluetoothDeviceInfo in deviceInfos)
            {
                var scannedDeviceAddress = bluetoothDeviceInfo.DeviceAddress;

                if (scannedDeviceAddress == btAddress)
                {
                    device = bluetoothDeviceInfo;
                }
            }

            if (device == null)
            {
                return;
            }

            ep = new BluetoothEndPoint(device.DeviceAddress, serviceClass);

            try
            {
                if (!device.Connected)
                {
                    _cli.Connect(ep);
                    strm = _cli.GetStream();
                }
                _isConnected = true;
            }
            catch (System.Net.Sockets.SocketException e)
            {
                System.Console.WriteLine(e.Message);
                _cli.Close();
                _isConnected = false;
                return;
            }

            SendVisualInitSequence();
        }

        
        private void WriteStringToStream(string Command)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Command);

            for (int i = 0; i < bytes.Length; i++)
            {
                strm.WriteByte((byte)bytes[0]);
            }
        }

        public override void SendSerialData(string Command)
        {

            if (_isConnected)
            {
                try
                {
                    WriteStringToStream(Command);
                }
                catch(Exception ex)
                {
                    Initialize();
                    if (_isConnected)
                        WriteStringToStream(Command);
                }
            }


        }
    }
}
