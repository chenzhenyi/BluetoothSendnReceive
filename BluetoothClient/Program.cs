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
    public class BluetoothUuidInfo
    {
        public string Uuid { get; set; }
        public string Description { get; set; }
        public string ShortUuid { get; set; }

        public BluetoothUuidInfo(string uuid, string description, string shortUuid = null)
        {
            Uuid = uuid;
            Description = description;
            ShortUuid = shortUuid;
        }
    }

    public class BluetoothUuids
    {
        // List to store commonly used Bluetooth service UUIDs with descriptions
        public static List<BluetoothUuidInfo> ServiceUuids = new List<BluetoothUuidInfo>
        {
            new BluetoothUuidInfo("00001101-0000-1000-8000-00805F9B34FB", "Serial Port Profile (SPP)", "1101"),
            new BluetoothUuidInfo("0000110A-0000-1000-8000-00805F9B34FB", "Audio Source", "110A"),
            new BluetoothUuidInfo("0000110B-0000-1000-8000-00805F9B34FB", "Audio Sink", "110B"),
            new BluetoothUuidInfo("0000110C-0000-1000-8000-00805F9B34FB", "A/V Remote Control Target", "110C"),
            new BluetoothUuidInfo("0000110E-0000-1000-8000-00805F9B34FB", "A/V Remote Control", "110E"),
            new BluetoothUuidInfo("00001103-0000-1000-8000-00805F9B34FB", "DUN (Dial-up Networking)", "1103"),
            new BluetoothUuidInfo("00001104-0000-1000-8000-00805F9B34FB", "IrMC Sync", "1104"),
            new BluetoothUuidInfo("00001105-0000-1000-8000-00805F9B34FB", "OBEX Object Push", "1105"),
            new BluetoothUuidInfo("00001106-0000-1000-8000-00805F9B34FB", "OBEX File Transfer", "1106"),
            new BluetoothUuidInfo("00001108-0000-1000-8000-00805F9B34FB", "Headset", "1108"),
            new BluetoothUuidInfo("0000111E-0000-1000-8000-00805F9B34FB", "Handsfree", "111E"),
            new BluetoothUuidInfo("0000112F-0000-1000-8000-00805F9B34FB", "Phonebook Access - PCE", "112F"),
            new BluetoothUuidInfo("00001130-0000-1000-8000-00805F9B34FB", "Phonebook Access - PSE", "1130"),
            new BluetoothUuidInfo("00001132-0000-1000-8000-00805F9B34FB", "Message Access Server", "1132"),
            new BluetoothUuidInfo("00001133-0000-1000-8000-00805F9B34FB", "Message Notification Server", "1133"),
            new BluetoothUuidInfo("00001134-0000-1000-8000-00805F9B34FB", "Message Access Profile", "1134"),
            new BluetoothUuidInfo("0000180F-0000-1000-8000-00805F9B34FB", "Battery Service", "180F"),
            new BluetoothUuidInfo("00001800-0000-1000-8000-00805F9B34FB", "Generic Access Profile", "1800"),
            new BluetoothUuidInfo("00001801-0000-1000-8000-00805F9B34FB", "Generic Attribute Profile", "1801"),
            new BluetoothUuidInfo("0000180A-0000-1000-8000-00805F9B34FB", "Device Information Service", "180A"),
            new BluetoothUuidInfo("0000180D-0000-1000-8000-00805F9B34FB", "Heart Rate Service", "180D"),
            new BluetoothUuidInfo("0000180E-0000-1000-8000-00805F9B34FB", "Phone Alert Status Service", "180E"),
            new BluetoothUuidInfo("00001802-0000-1000-8000-00805F9B34FB", "Immediate Alert Service", "1802"),
            new BluetoothUuidInfo("00001803-0000-1000-8000-00805F9B34FB", "Link Loss Service", "1803"),
            new BluetoothUuidInfo("00001804-0000-1000-8000-00805F9B34FB", "Tx Power Service", "1804"),
            new BluetoothUuidInfo("00001812-0000-1000-8000-00805F9B34FB", "Human Interface Device Service", "1812"),
        };

        // Method to find UUID by description
        public static BluetoothUuidInfo FindByDescription(string description)
        {
            return ServiceUuids.Find(u => u.Description.Contains(description));
        }

        // Method to find UUID by short UUID
        public static BluetoothUuidInfo FindByShortUuid(string shortUuid)
        {
            return ServiceUuids.Find(u => u.ShortUuid?.Equals(shortUuid, StringComparison.OrdinalIgnoreCase) == true);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            BluetoothRadio radio = BluetoothRadio.Default;
            if (radio == null)
            {
                Console.WriteLine("No Bluetooth radio hardware or unsupported software stack");
                return;
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

            BluetoothDeviceInfo[] devices = client.DiscoverDevices().ToArray();

            if (devices.Length == 0)
            {
                Console.WriteLine("No Bluetooth devices found.");
                return;
            }

            Console.WriteLine("Bluetooth Devices: ");
            for (int i = 0; i < devices.Count(); i++)
            {
                Console.WriteLine(i + " - " + devices[i].DeviceName);
            }

            Console.Write("Choose the device to connect: ");
            int deviceInput;
            if (!int.TryParse(Console.ReadLine(), out deviceInput) || deviceInput < 0 || deviceInput >= devices.Length)
            {
                Console.WriteLine("Invalid device selection.");
                return;
            }

            BluetoothDeviceInfo device = devices[deviceInput];
            Console.WriteLine("Device address: " + device.DeviceAddress.ToString());

            // Display available service UUIDs
            Console.WriteLine("\nAvailable Bluetooth Service UUIDs:");
            for (int i = 0; i < BluetoothUuids.ServiceUuids.Count; i++)
            {
                var uuidInfo = BluetoothUuids.ServiceUuids[i];
                Console.WriteLine($"{i} - {uuidInfo.Description} ({uuidInfo.ShortUuid})");
            }

            Console.Write("\nChoose the service UUID to use: ");
            int uuidInput;
            if (!int.TryParse(Console.ReadLine(), out uuidInput) || uuidInput < 0 || uuidInput >= BluetoothUuids.ServiceUuids.Count)
            {
                Console.WriteLine("Invalid UUID selection. Using default SPP UUID.");
                uuidInput = 0; // Default to SPP
            }

            BluetoothUuidInfo selectedUuid = BluetoothUuids.ServiceUuids[uuidInput];
            Console.WriteLine($"Selected: {selectedUuid.Description} - {selectedUuid.Uuid}");

            BluetoothAddress bluetoothAddress = device.DeviceAddress;
            Guid serviceUuid = new Guid(selectedUuid.Uuid);
            BluetoothEndPoint endPoint = new BluetoothEndPoint(bluetoothAddress, serviceUuid);

            try
            {
                if (!device.Authenticated)
                {
                    Console.WriteLine("The device is not authenticated");
                    try
                    {
                        BluetoothSecurity.PairRequest(device.DeviceAddress, null);
                        Console.WriteLine("Pairing request sent.");
                    }
                    catch (Exception pairEx)
                    {
                        Console.WriteLine($"Pairing failed: {pairEx.Message}");
                    }
                }
                else if (device.Authenticated)
                {
                    Console.WriteLine("The device is authenticated");
                }

                Console.WriteLine("Attempting to connect...");
                client.Connect(endPoint);
                Console.WriteLine("Bluetooth Connected to client");

                // Check if file exists
                string fileName = "example.txt";
                if (!File.Exists(fileName))
                {
                    Console.WriteLine($"File '{fileName}' not found. Creating a sample file...");
                    File.WriteAllText(fileName, "Hello from Bluetooth sender!\nThis is a test file transmission.");
                }

                byte[] fileData = File.ReadAllBytes(fileName);
                using (NetworkStream stream = client.GetStream())
                {
                    // Send file size first
                    byte[] fileSize = BitConverter.GetBytes(fileData.Length);
                    stream.Write(fileSize, 0, 4);
                    Console.WriteLine($"File size sent: {fileData.Length} bytes");

                    // Then send file
                    stream.Write(fileData, 0, fileData.Length);
                    stream.Flush();
                    Console.WriteLine("File Sent successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                Console.WriteLine("Make sure the target device supports the selected service and is accepting connections.");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Connection closed.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
