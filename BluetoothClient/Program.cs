using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.IO;
using System.Xml.Linq;
using System.Net.Sockets;


namespace BluetoothClientSender
{

    internal class Program
    {
        static void Main(string[] args)
        {


            BluetoothRadio radio = BluetoothRadio.Default;
            if (radio == null)
            {
                Console.WriteLine("No Bluetooth radio hardware or unsupported software stack");
            }
            else
            {
                Console.WriteLine("Local Bluetooth MAC Address: " + radio.LocalAddress);
            }



            // Client-side (Sender)
            BluetoothClient client;
            try
            {
                client = new BluetoothClient();
            }
            catch (Exception e)
            {
                Console.WriteLine("Turn your bluetooth on and try again.");
                return;
            }

            //BluetoothDeviceInfo[] devices = client.DiscoverDevices().ToArray;
            BluetoothDeviceInfo[] devices = client.DiscoverDevices().ToArray();


            Console.WriteLine("Bluetooth Devices: ");
            for (int i = 0; i < devices.Count(); i++)
            {
                Console.WriteLine(i + " - " + devices[i].DeviceName);
            }

            Console.Write("Choose the device to connect: ");
            int input = Convert.ToInt32(Console.ReadLine());
            BluetoothDeviceInfo device = devices[input];
            Console.WriteLine("Device adress: " + device.DeviceAddress.ToString());
            BluetoothAddress bluetoothAddress = device.DeviceAddress;
            Guid serviceUuid = new Guid("00001101-0000-1000-8000-00805F9B34FB");
            BluetoothEndPoint endPoint = new BluetoothEndPoint(bluetoothAddress, serviceUuid);

            try
            {
                if (!device.Authenticated)
                {
                    Console.WriteLine("The device is not authenticated");
                    BluetoothSecurity.PairRequest(device.DeviceAddress, null);
                }
                else if (device.Authenticated)
                {
                    Console.WriteLine("The device is authenticated");
                }
                client.Connect(endPoint);
                Console.WriteLine("Bluetooth Connected to client");

                byte[] fileData = File.ReadAllBytes("example.txt");
                using (NetworkStream stream = client.GetStream())
                {
                    // Send file size first
                    byte[] fileSize = BitConverter.GetBytes(fileData.Length);
                    stream.Write(fileSize, 0, 4);
                    Console.WriteLine("File read");

                    // Then send file
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Flush();
                    Console.WriteLine("File Sent");
                }
/*
                Stream stream = client.GetStream();
                byte[] fileData = File.ReadAllBytes("example.txt");
                stream.Write(fileData, 0, fileData.Length);
                Console.WriteLine("File Sent");
                stream.Close();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
            finally
            {
                client.Close();
            }

        }
    }
}
