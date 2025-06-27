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

            Guid serviceUuid = new Guid("00001101-0000-1000-8000-00805F9B34FB");

            // Initialize BluetoothListener with service UUID
            using (BluetoothListener listener = new BluetoothListener(serviceUuid))
            {
                try
                {
                    listener.Start();
                    Console.WriteLine("Waiting for RFCOMM connection...");

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

                            // Read file data
                            byte[] buffer = new byte[4096];
                            int bytesRead, bytesRemaining = fileSize;
                            while (bytesRemaining > 0 && (bytesRead = stream.Read(buffer, 0, Math.Min(buffer.Length, bytesRemaining))) > 0)
                            {
                                fileStream.Write(buffer, 0, bytesRead);
                                bytesRemaining -= bytesRead;
                            }
                            Console.WriteLine("File received successfully.");
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
                }
            }
        }


    }
}

