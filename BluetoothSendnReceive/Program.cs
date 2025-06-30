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

namespace BluetoothSendnReceive
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
        public static List<BluetoothUuidInfo> ServiceUuids = new List<BluetoothUuidInfo>
        {
            new BluetoothUuidInfo("00001101-0000-1000-8000-00805F9B34FB", "Serial Port Profile (SPP)", "1101"),
            new BluetoothUuidInfo("0000110A-0000-1000-8000-00805F9B34FB", "Audio Source", "110A"),
            new BluetoothUuidInfo("0000110B-0000-1000-8000-00805F9B34FB", "Audio Sink", "110B"),
            new BluetoothUuidInfo("0000110C-0000-1000-8000-00805F9B34FB", "A/V Remote Control Target", "110C"),
            new BluetoothUuidInfo("0000110E-0000-1000-8000-00805F9B34FB", "A/V Remote Control", "110E"),
            new BluetoothUuidInfo("00001103-0000-1000-8000-00805F9B34FB", "Dial-up Networking", "1103"),
            new BluetoothUuidInfo("00001104-0000-1000-8000-00805F9B34FB", "IrMC Sync", "1104"),
            new BluetoothUuidInfo("00001105-0000-1000-8000-00805F9B34FB", "OBEX Object Push", "1105"),
            new BluetoothUuidInfo("00001106-0000-1000-8000-00805F9B34FB", "OBEX File Transfer", "1106"),
            new BluetoothUuidInfo("00001108-0000-1000-8000-00805F9B34FB", "Headset Profile", "1108"),
            new BluetoothUuidInfo("0000111E-0000-1000-8000-00805F9B34FB", "Hands-free Profile", "111E"),
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
            new BluetoothUuidInfo("00001816-0000-1000-8000-00805F9B34FB", "Cycling Speed and Cadence Service", "1816"),
            new BluetoothUuidInfo("00001818-0000-1000-8000-00805F9B34FB", "Cycling Power Service", "1818"),
            new BluetoothUuidInfo("00001819-0000-1000-8000-00805F9B34FB", "Location and Navigation Service", "1819"),
        };

        public static BluetoothUuidInfo FindByIndex(int index)
        {
            if (index >= 0 && index < ServiceUuids.Count)
                return ServiceUuids[index];
            return null;
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

            // Display available UUIDs for user selection
            Console.WriteLine("\nAvailable Bluetooth Service UUIDs:");
            Console.WriteLine("=====================================");
            for (int i = 0; i < BluetoothUuids.ServiceUuids.Count; i++)
            {
                var uuidInfo = BluetoothUuids.ServiceUuids[i];
                Console.WriteLine($"{i + 1}. {uuidInfo.Description}");
                Console.WriteLine($"   UUID: {uuidInfo.Uuid}");
                Console.WriteLine($"   Short: {uuidInfo.ShortUuid}");
                Console.WriteLine();
            }

            // Get user selection
            Console.Write($"Please select a service UUID (1-{BluetoothUuids.ServiceUuids.Count}): ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int selection) || selection < 1 || selection > BluetoothUuids.ServiceUuids.Count)
            {
                Console.WriteLine("Invalid selection. Using default Serial Port Profile (SPP).");
                selection = 1; // Default to SPP
            }

            BluetoothUuidInfo selectedUuid = BluetoothUuids.FindByIndex(selection - 1);
            Guid serviceUuid = new Guid(selectedUuid.Uuid);

            Console.WriteLine($"\nSelected Service: {selectedUuid.Description}");
            Console.WriteLine($"Using UUID: {selectedUuid.Uuid}");
            Console.WriteLine();

            // Initialize BluetoothListener with selected service UUID
            using (BluetoothListener listener = new BluetoothListener(serviceUuid))
            {
                try
                {
                    listener.Start();
                    Console.WriteLine($"Waiting for RFCOMM connection on {selectedUuid.Description}...");

                    using (BluetoothClient client = listener.AcceptBluetoothClient())
                    {
                        Console.WriteLine($"Connected to {client.RemoteMachineName}");

                        using (Stream stream = client.GetStream())
                        using (var fileStream = File.Create("received_file.txt"))
                        {
                            // Read file size (4 bytes)
                            byte[] sizeBuffer = new byte[4];
                            int totalRead = 0;
                            while (totalRead < 4)
                            {
                                int read = stream.Read(sizeBuffer, totalRead, 4 - totalRead);
                                if (read == 0) throw new IOException("Connection closed before file size received.");
                                totalRead += read;
                            }
                            int fileSize = BitConverter.ToInt32(sizeBuffer, 0);

                            Console.WriteLine($"Receiving file of size: {fileSize} bytes");

                            // Read file data
                            byte[] buffer = new byte[4096];
                            int bytesRead, bytesRemaining = fileSize;
                            int totalBytesReceived = 0;

                            while (bytesRemaining > 0 && (bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, bytesRemaining))) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                                bytesRemaining -= bytesRead;
                                totalBytesReceived += bytesRead;

                                // Show progress
                                double progress = (double)totalBytesReceived / fileSize * 100;
                                Console.Write($"\rProgress: {progress:F1}% ({totalBytesReceived}/{fileSize} bytes)");
                            }

                            Console.WriteLine("\nFile received successfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    listener.Stop(); // Ensure proper cleanup
                    Console.WriteLine("Bluetooth listener stopped.");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
