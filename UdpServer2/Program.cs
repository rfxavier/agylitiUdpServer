/* Project  : Simple Multi-threaded TCP/UDP Server v2
 * Author   : Patrick Lam
 * Date     : 09/19/2001
 * Brief    : The simple multi-threaded TCP/UDP Server v2 does the same thing as v1.  What
 *            it intends to demonstrate is the amount of code you can save by using TcpListener
 *            instead of the traditional raw socket implementation (The UDP part is still
 *            the same.  When you compare the following code with v1, you will see the 
 *            difference.
 * Usage    : sampleTcpUdpServer2
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sampleTcpUdpServer2
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class SampleTcpUdpServer2
    {
        private const int sampleTcpPort = 4567;
        private const int sampleUdpPort = 4568;
        public Thread sampleTcpThread, sampleUdpThread;

        public SampleTcpUdpServer2()
        {
            //try
            //{
            //    //Starting the TCP Listener thread.
            //    sampleTcpThread = new Thread(new ThreadStart(StartListen2));
            //    sampleTcpThread.Start();
            //    Console.WriteLine("Started SampleTcpUdpServer's TCP Listener Thread!\n");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("An TCP Exception has occurred!" + e.ToString());
            //    sampleTcpThread.Abort();
            //}

            try
            {
                //Starting the UDP Server thread.
                sampleUdpThread = new Thread(new ThreadStart(StartReceiveFromEscapeException));
                sampleUdpThread.Start();
                Console.WriteLine("Started SampleTcpUdpServer's UDP Receiver Thread!\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("An UDP Exception has occurred!" + e.ToString());
                sampleUdpThread.Abort();
            }
        }

        public static void Main(String[] argv)
        {
            SampleTcpUdpServer2 sts = new SampleTcpUdpServer2();
        }

        public void StartListen2()
        {
            //Create an instance of TcpListener to listen for TCP connection.
            TcpListener tcpListener = new TcpListener(sampleTcpPort);

            try
            {
                while (true)
                {
                    tcpListener.Start();

                    //Program blocks on Accept() until a client connects.
                    Socket soTcp = tcpListener.AcceptSocket();

                    Console.WriteLine("SampleClient is connected through TCP.");
                    Byte[] received = new Byte[512];
                    int bytesReceived = soTcp.Receive(received, received.Length, 0);

                    String dataReceived = System.Text.Encoding.ASCII.GetString(received);
                    Console.WriteLine(dataReceived);

                    String returningString = "The Server got your message through TCP: " +
                        dataReceived;

                    Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(returningString.ToCharArray());

                    //Returning a confirmation string back to the client.				
                    soTcp.Send(returningByte, returningByte.Length, 0);

                    tcpListener.Stop();
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
            }
        }

        public void StartReceiveFrom2()
        {

            IPHostEntry localHostEntry;

            try
            {
                //Create a UDP socket.
                Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    localHostEntry = Dns.GetHostByName(Dns.GetHostName());
                }
                catch (Exception)
                {
                    Console.WriteLine("Local Host not found"); // fail
                    return;
                }


                IPEndPoint localIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);

                soUdp.Bind(localIpEndPoint);

                while (true)
                {
                    Byte[] received = new Byte[256];
                    IPEndPoint tmpIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);

                    EndPoint remoteEP = (tmpIpEndPoint);

                    Console.WriteLine(System.Text.Encoding.ASCII.GetString(received).Length);

                    int bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);

                    Console.WriteLine(System.Text.Encoding.ASCII.GetString(received).Length);

                    String dataReceived = System.Text.Encoding.ASCII.GetString(received);

                    //Console.WriteLine("SampleClient is connected through UDP.");

                    //Console.WriteLine(dataReceived);

                    //String returningString = "The Server got your message through UDP:" + dataReceived;

                    var parsedReturnArray = dataReceived.Split(';');

                    var parsedReturnArrayFirst6 = parsedReturnArray.Take(6).ToList();

                    if (parsedReturnArrayFirst6.Count() > 2)
                    {
                        parsedReturnArrayFirst6[1] = 'A'.ToString();
                    }

                    var rawReturn = string.Join(";", parsedReturnArrayFirst6) + ";6;FIM;";

                    Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(rawReturn.ToCharArray());
                    //Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(returningString.ToCharArray());

                    Console.WriteLine("s:" + rawReturn);

                    Console.WriteLine("r:" + dataReceived);

                    soUdp.SendTo(returningByte, remoteEP);
                }

            }
            catch (SocketException se)
            {
                Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
            }

        }

        public static List<string> headerList = new List<string>()
        {
            "h01_preamble",
            "h02_status",
            "h03_modelo",
            "h04_versao",
            "h05_serial",
            "h06_sequencia",
            "h07_tipo"
        };

        public static List<string> trackingType1List = new List<string>()
        {
            "t01_ts_evento",
            "t02_ts_gps",
            "t03_lat_gps",
            "t04_lon_gps",
            "t05_vel_gps",
            "t06_dir_gps",
            "t07_alt_gps",
            "t08_val_gps",
            "t09_numsat_gps",
            "t10_hdop_gps",
            "t11_stat_sr1",
            "t12_stat_sr2",
            "t13_stat_st",
            "t14_stat_sat1",
            "t15_stat_sat2",
            "t16_tbe",
            "t17_tbi",
            "t18_hodo_gps",
            "t19_sinal_gsm",
            "t20_sensor_temp",
            "t21_lacre_sensor",
            "t22_stat1_rast",
            "t23_stat2_rast"
        };

        public static List<string> additionalTempList = new List<string>()
        {
            "a01_bits",
            "a02_tempdata01",
            "a02_tempdata02",
            "a02_tempdata03",
            "a02_tempdata04",
            "a02_tempdata05",
            "a02_tempdata06",
            "a02_tempdata07",
            "a02_tempdata08",
            "a02_tempdata09",
            "a02_tempdata10"
        };

        public static List<string> temperatureSensorHeaderList = new List<string>()
        {
            "a01_bits",
            "ts01_hasdata",
            "ts01_id",
            "ts01_temperature",
            "ts02_hasdata",
            "ts02_id",
            "ts02_temperature",
            "ts03_hasdata",
            "ts03_id",
            "ts03_temperature",
            "ts04_hasdata",
            "ts04_id",
            "ts04_temperature",
            "ts05_hasdata",
            "ts05_id",
            "ts05_temperature",
            "ts06_hasdata",
            "ts06_id",
            "ts06_temperature",
            "ts07_hasdata",
            "ts07_id",
            "ts07_temperature",
            "ts08_hasdata",
            "ts08_id",
            "ts08_temperature",
            "ts09_hasdata",
            "ts09_id",
            "ts09_temperature",
            "ts10_hasdata",
            "ts10_id",
            "ts10_temperature"
        };

        public static List<string> sensorHeaderList = new List<string> {
            "s01_sensortype",
            "s02_sensornumber",
            "s03_sensoreventtype"
        };

        public static List<string> embeddedHeaderList = new List<string>
        {
            "e01_embeddedeventtype",
            "e02_embeddedeventcontent"
        };

        public static int SecondsIntervalToFlush = 100;

        public static string IncomingFilesFolder = @"C:\pentaho\repo\projects\rstRastreador\fileHandling\incoming\";

        public enum MessagePayloadType
        {
            NotToBeHandled = 0,
            TrackingOrTrackingSleep = 1, //messageType 1 or 2
            IgnitionOnOff = 2,           //messageType 3 or 4
            SensorEvent = 3,             //messageType 9
            EmbeddedEvent = 4            //messageType 47
        }
        public void StartReceiveFromEscapeException()
        {

            IPHostEntry localHostEntry;

            try
            {
                //Create a UDP socket.
                Socket soUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                try
                {
                    localHostEntry = Dns.GetHostByName(Dns.GetHostName());
                    Console.WriteLine("Hostname:" + localHostEntry.AddressList[0] + " Port:" + sampleUdpPort);
                }
                catch (Exception)
                {
                    Console.WriteLine("Local Host not found"); // fail
                    return;
                }


                IPEndPoint localIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);

                soUdp.Bind(localIpEndPoint);

                var streamTrack = new List<string>{};
                var streamSensor = new List<string>{};
                var streamEmbedded = new List<string>{};
                var streamIgnitionOnOff = new List<string>{};

                Int32 baseUnixTimestampTrack;
                Int32 baseUnixTimestampSensor;
                Int32 baseUnixTimestampEmbedded;
                Int32 baseUnixTimestampIgnitionOnOff;
                int baseTimeReferenceForMemoryRetentionTrack;
                int baseTimeReferenceForMemoryRetentionSensor;
                int baseTimeReferenceForMemoryRetentionEmbedded;
                int baseTimeReferenceForMemoryRetentionIgnitionOnOff;

                baseUnixTimestampTrack = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                baseTimeReferenceForMemoryRetentionTrack = (baseUnixTimestampTrack - baseUnixTimestampTrack % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                baseUnixTimestampSensor = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                baseTimeReferenceForMemoryRetentionSensor = (baseUnixTimestampSensor - baseUnixTimestampSensor % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                baseUnixTimestampEmbedded = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                baseTimeReferenceForMemoryRetentionEmbedded = (baseUnixTimestampEmbedded - baseUnixTimestampEmbedded % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                baseUnixTimestampIgnitionOnOff = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                baseTimeReferenceForMemoryRetentionIgnitionOnOff = (baseUnixTimestampIgnitionOnOff - baseUnixTimestampIgnitionOnOff % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                while (true)
                {
                    try
                    {
                        Byte[] received = new Byte[1024];
                        IPEndPoint tmpIpEndPoint = new IPEndPoint(localHostEntry.AddressList[0], sampleUdpPort);

                        EndPoint remoteEP = (tmpIpEndPoint);

                        int bytesReceived = soUdp.ReceiveFrom(received, ref remoteEP);

                        var remote = (IPEndPoint)remoteEP;
                        //Console.WriteLine($"Received {bytesReceived} bytes from {remote.Address}:{remote.Port}");

                        String dataReceived = System.Text.Encoding.ASCII.GetString(received, 0, bytesReceived);
                        //Console.WriteLine($"Received from {remote.Address}:{remote.Port}: {dataReceived}");

                        var parsedDataReceivedArray = dataReceived.Split(';');

                        var parsedDataReceivedArrayStripLastPos = parsedDataReceivedArray.Take(parsedDataReceivedArray.Count() - 1).ToArray();

                        var dataReceivedTrimmed = string.Join(";", parsedDataReceivedArrayStripLastPos);

                        var parsedDataReceivedArrayFirst7 = parsedDataReceivedArray.Take(7).ToList();
                        var parsedDataReceivedArrayFirst7Raw = parsedDataReceivedArray.Take(7).ToList();

                        if (parsedDataReceivedArrayFirst7.Count() < 7)
                        {
                            continue;
                        }

                        parsedDataReceivedArrayFirst7[1] = 'A'.ToString();

                        var messageType = parsedDataReceivedArrayFirst7[6];

                        string clientAckResponse = "";
                        MessagePayloadType messagePayloadType = MessagePayloadType.NotToBeHandled;

                        if (!(messageType == "1" || messageType == "2" || messageType == "8" || messageType == "9" || messageType == "47" || messageType == "3" || messageType == "4"))
                        {
                        // msgType not interesting - Respond whatever
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";
                        }
                        else if (messageType == "8")
                        // msgType 8 - Respond with timestamp
                        {
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";

                            if (parsedDataReceivedArrayFirst7[4] == "008800696")
                            {
                                //clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";177;59;18;rst.agyliti.com.br;FIM;";
                                clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";129;3;3.82.138.151;4568;FIM;";
                            }
                        }
                        else if (messageType == "1" || messageType == "2")
                        // msgType 1 or 2
                        {
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";

                            if (parsedDataReceivedArrayFirst7[4] == "008800696")
                            {
                                //clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";177;59;18;rst.agyliti.com.br;FIM;";
                                clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";129;3;3.82.138.151;4568;FIM;";
                            }

                            if (parsedDataReceivedArrayFirst7Raw[1] == "A")
                            {
                                messagePayloadType = MessagePayloadType.TrackingOrTrackingSleep;
                            }
                        }
                        else if (messageType == "9")
                        // msgType 9
                        {
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";

                            if (parsedDataReceivedArrayFirst7Raw[1] == "A")
                            {
                                messagePayloadType = MessagePayloadType.SensorEvent;
                            }
                        }
                        else if (messageType == "47")
                        // msgType 47
                        {
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";

                            if (parsedDataReceivedArrayFirst7Raw[1] == "A")
                            {
                                messagePayloadType = MessagePayloadType.EmbeddedEvent;
                            }
                        }
                        else if (messageType == "3" || messageType == "4")
                        // msgType 3 or 4
                        {
                            clientAckResponse = string.Join(";", parsedDataReceivedArrayFirst7.Take(6).ToList()) + ";6;FIM;";

                            if (parsedDataReceivedArrayFirst7Raw[1] == "A")
                            {
                                messagePayloadType = MessagePayloadType.IgnitionOnOff;
                            }
                        };
                        
                        Byte[] returningByte = System.Text.Encoding.ASCII.GetBytes(clientAckResponse.ToCharArray());

                        Console.WriteLine("s:" + clientAckResponse);

                        Console.WriteLine("r:" + dataReceivedTrimmed);


                        soUdp.SendTo(returningByte, remoteEP);

                        if (messagePayloadType == MessagePayloadType.TrackingOrTrackingSleep) // messageType = 1 or 2
                        {
                            Console.WriteLine(baseTimeReferenceForMemoryRetentionTrack);

                            Int32 currentUnixTimestampTrack = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            int currentTimeReferenceForMemoryRetentionTrack = (currentUnixTimestampTrack - currentUnixTimestampTrack % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                            Console.WriteLine(currentTimeReferenceForMemoryRetentionTrack);

                            streamTrack.Add(dataReceivedTrimmed);

                            if (currentTimeReferenceForMemoryRetentionTrack != baseTimeReferenceForMemoryRetentionTrack)
                            {
                                Console.WriteLine("FLUSH TRACK");
                                
                                using (var fs = new FileStream($@"{IncomingFilesFolder}track{baseTimeReferenceForMemoryRetentionTrack}.csv", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    TextWriter tw = new StreamWriter(fs);

                                    var fileHeader = string.Join(";", headerList.Concat(trackingType1List).Concat(temperatureSensorHeaderList));

                                    tw.WriteLine(fileHeader);

                                    foreach (var element in streamTrack)
                                    {
                                        var parsedElement = ParseForTemperatureAdditionalData(element);
                                        Console.WriteLine(parsedElement);
                                        tw.WriteLine(parsedElement);
                                    }

                                    tw.Flush();
                                }

                                baseUnixTimestampTrack = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                baseTimeReferenceForMemoryRetentionTrack = (baseUnixTimestampTrack - baseUnixTimestampTrack % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                                streamTrack.Clear();
                            }
                        }

                        if (messagePayloadType == MessagePayloadType.SensorEvent) // messageType = 9
                        {
                            Console.WriteLine(baseTimeReferenceForMemoryRetentionSensor);

                            Int32 currentUnixTimestampSensor = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            int currentTimeReferenceForMemoryRetentionSensor = (currentUnixTimestampSensor - currentUnixTimestampSensor % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                            Console.WriteLine(currentTimeReferenceForMemoryRetentionSensor);

                            streamSensor.Add(dataReceivedTrimmed);

                            if (currentTimeReferenceForMemoryRetentionSensor != baseTimeReferenceForMemoryRetentionSensor)
                            {
                                Console.WriteLine("FLUSH SENSOR");

                                using (var fs = new FileStream($@"{IncomingFilesFolder}sensor{baseTimeReferenceForMemoryRetentionSensor}.csv", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    TextWriter tw = new StreamWriter(fs);

                                    var fileHeader = string.Join(";", headerList.Concat(trackingType1List).Concat(sensorHeaderList));

                                    tw.WriteLine(fileHeader);

                                    foreach (var element in streamSensor)
                                    {
                                        Console.WriteLine(element);
                                        tw.WriteLine(element);
                                    }

                                    tw.Flush();
                                }

                                baseUnixTimestampSensor = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                baseTimeReferenceForMemoryRetentionSensor = (baseUnixTimestampSensor - baseUnixTimestampSensor % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                                streamSensor.Clear();
                            }
                        }

                        if (messagePayloadType == MessagePayloadType.EmbeddedEvent) // messageType = 47
                        {
                            Console.WriteLine(baseTimeReferenceForMemoryRetentionEmbedded);

                            Int32 currentUnixTimestampEmbedded = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            int currentTimeReferenceForMemoryRetentionEmbedded = (currentUnixTimestampEmbedded - currentUnixTimestampEmbedded % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                            Console.WriteLine(currentTimeReferenceForMemoryRetentionEmbedded);

                            streamEmbedded.Add(dataReceivedTrimmed);

                            if (currentTimeReferenceForMemoryRetentionEmbedded != baseTimeReferenceForMemoryRetentionEmbedded)
                            {
                                Console.WriteLine("FLUSH EMBEDDED");

                                using (var fs = new FileStream($@"{IncomingFilesFolder}embedded{baseTimeReferenceForMemoryRetentionEmbedded}.csv", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    TextWriter tw = new StreamWriter(fs);

                                    var fileHeader = string.Join(";", headerList.Concat(trackingType1List).Concat(embeddedHeaderList));

                                    tw.WriteLine(fileHeader);

                                    foreach (var element in streamEmbedded)
                                    {
                                        Console.WriteLine(element);
                                        tw.WriteLine(element);
                                    }

                                    tw.Flush();
                                }

                                baseUnixTimestampEmbedded = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                baseTimeReferenceForMemoryRetentionEmbedded = (baseUnixTimestampEmbedded - baseUnixTimestampEmbedded % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                                streamEmbedded.Clear();
                            }
                        }

                        if (messagePayloadType == MessagePayloadType.IgnitionOnOff) // messageType = 3 or 4
                        {
                            Console.WriteLine(baseTimeReferenceForMemoryRetentionIgnitionOnOff);

                            Int32 currentUnixTimestampIgnitionOnOff = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            int currentTimeReferenceForMemoryRetentionIgnitionOnOff = (currentUnixTimestampIgnitionOnOff - currentUnixTimestampIgnitionOnOff % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                            Console.WriteLine(currentTimeReferenceForMemoryRetentionIgnitionOnOff);

                            streamIgnitionOnOff.Add(dataReceivedTrimmed);

                            if (currentTimeReferenceForMemoryRetentionIgnitionOnOff != baseTimeReferenceForMemoryRetentionIgnitionOnOff)
                            {
                                Console.WriteLine("FLUSH IGNITION ON OFF");

                                using (var fs = new FileStream($@"{IncomingFilesFolder}ignition{baseTimeReferenceForMemoryRetentionIgnitionOnOff}.csv", FileMode.Create, FileAccess.ReadWrite))
                                {
                                    TextWriter tw = new StreamWriter(fs);

                                    var fileHeader = string.Join(";", headerList.Concat(trackingType1List));

                                    tw.WriteLine(fileHeader);

                                    foreach (var element in streamIgnitionOnOff)
                                    {
                                        Console.WriteLine(element);
                                        tw.WriteLine(element);
                                    }

                                    tw.Flush();
                                }

                                baseUnixTimestampIgnitionOnOff = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                baseTimeReferenceForMemoryRetentionIgnitionOnOff = (baseUnixTimestampIgnitionOnOff - baseUnixTimestampIgnitionOnOff % SecondsIntervalToFlush) / SecondsIntervalToFlush;

                                streamIgnitionOnOff.Clear();
                            }
                        }

                    }
                    catch (SocketException se)
                    {
                        continue;
                    }
                }

            }
            catch (SocketException se)
            {
                Console.WriteLine("A Socket Exception has occurred!" + se.ToString());
            }

        } 

        public string ParseForTemperatureAdditionalData(string streamData)
        {
            var parsedStreamDataArray = streamData.Trim().Split(';');

            var returnData = streamData.Trim();

            var tempDataCount = 0;

            var baseStreamDataArray = parsedStreamDataArray.Take(31).ToList();

            if (parsedStreamDataArray[30] != "FIM")
            {
                for (int i = 31; i < parsedStreamDataArray.Length - 1; i++)
                {
                    var splitForEqualSignArray = parsedStreamDataArray[i].Split('=');

                    if (splitForEqualSignArray.Length > 1)
                    {
                        baseStreamDataArray.Add("1"); //tsNN_hasdata
                        baseStreamDataArray.Add(splitForEqualSignArray[0]); //tsNN_id
                        baseStreamDataArray.Add(splitForEqualSignArray[1]); //tsNN_temperature

                        tempDataCount++;
                    }
                }

            }
            else
            {
                baseStreamDataArray.Add("noa01_bits");
            }

            for (int j = 0; j <= 9 - tempDataCount; j++)
            {
                baseStreamDataArray.Add("0"); //tsNN_hasdata
                baseStreamDataArray.Add("NODATA"); //tsNN_id
                baseStreamDataArray.Add("0"); //tsNN_temperature
            }

            baseStreamDataArray.Remove("FIM");

            returnData = string.Join(";", baseStreamDataArray);

            return returnData;
        }

        public string ParseForSensorEventData(string streamData)
        {
            var parsedStreamDataArray = streamData.Trim().Split(';');

            var returnData = streamData.Trim();

            var tempDataCount = 0;

            var baseStreamDataArray = parsedStreamDataArray.Take(31).ToList();

            if (parsedStreamDataArray[30] != "FIM")
            {
                for (int i = 31; i < parsedStreamDataArray.Length - 1; i++)
                {
                    var splitForEqualSignArray = parsedStreamDataArray[i].Split('=');

                    if (splitForEqualSignArray.Length > 1)
                    {
                        baseStreamDataArray.Add("1"); //tsNN_hasdata
                        baseStreamDataArray.Add(splitForEqualSignArray[0]); //tsNN_id
                        baseStreamDataArray.Add(splitForEqualSignArray[1]); //tsNN_temperature

                        tempDataCount++;
                    }
                }

            }
            else
            {
                baseStreamDataArray.Add("noa01_bits");
            }

            for (int j = 0; j <= 9 - tempDataCount; j++)
            {
                baseStreamDataArray.Add("0"); //tsNN_hasdata
                baseStreamDataArray.Add("NODATA"); //tsNN_id
                baseStreamDataArray.Add("0"); //tsNN_temperature
            }

            baseStreamDataArray.Remove("FIM");

            returnData = string.Join(";", baseStreamDataArray);

            return returnData;
        }
    }
}
