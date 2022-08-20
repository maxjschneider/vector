using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace charmap
{
    class UpdateInfo
    {
        public string weapon = "none";
        public string scope = "none";
        public string barrel = "none";

        public bool focused = true;

        public bool menu = true;
        public bool watermark = true;
        public bool crosshair = false;

        public string crosshaircolor = "rgb(199,21,133)";
        public string crosshairtype = "normal";
    }

    class Server
    {
        Socket socket;
        Socket client = null;

        static private string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public Server(IPAddress ip, int port)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(ip, port);

            socket = new Socket(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.IP);

            socket.Bind(iPEndPoint);

            socket.Listen(1);

            Console.WriteLine("Waiting for connection...");

            socket.BeginAccept(null, 0, OnAccept, null);
        }

        public void SendMessage(string message)
        {
            if (client != null && socket.IsBound)
            {
                try
                {
                    client.Send(GetFrameFromString(message));
                } catch
                {

                }
            }
        }

        private void OnAccept(IAsyncResult result)
        {
            byte[] buffer = new byte[1024];
            try
            {
                string headerResponse = "";
                if (socket != null && socket.IsBound)
                {
                    client = socket.EndAccept(result);
                    var i = client.Receive(buffer);
                    headerResponse = (System.Text.Encoding.UTF8.GetString(buffer)).Substring(0, i);

                    Console.WriteLine(headerResponse);
                    Console.WriteLine("=====================");
                }
                if (client != null)
                {
                    var key = headerResponse.Replace("ey:", "`")
                              .Split('`')[1]
                              .Replace("\r", "").Split('\n')[0]
                              .Trim();

                    var test1 = AcceptKey(ref key);

                    var newLine = "\r\n";

                    var response = "HTTP/1.1 101 Switching Protocols" + newLine
                         + "Upgrade: websocket" + newLine
                         + "Connection: Upgrade" + newLine
                         + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
                         ;

                    client.Send(System.Text.Encoding.UTF8.GetBytes(response));
                    var i = client.Receive(buffer);

                    while (i < 0)
                    {
                        i = client.Receive(buffer);
                    }

                    string browserSent = GetDecodedData(buffer, i);
                    Console.WriteLine("BrowserSent: " + browserSent);

                    Console.WriteLine("=====================");
                }
            }
            catch (SocketException exception)
            {
                throw exception;
            }
        }

        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        private static string AcceptKey(ref string key)
        {
            string longKey = key + guid;
            byte[] hashBytes = ComputeHash(longKey);
            return Convert.ToBase64String(hashBytes);
        }

        static SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        private static byte[] ComputeHash(string str)
        {
            return sha1.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
        }

        //Needed to decode frame
        public static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return Encoding.ASCII.GetString(buffer, dataIndex, dataLength);
        }

        //function to create  frames to send to client 
        /// <summary>
        /// Enum for opcode types
        /// </summary>
        public enum EOpcodeType
        {
            /* Denotes a continuation code */
            Fragment = 0,

            /* Denotes a text code */
            Text = 1,

            /* Denotes a binary code */
            Binary = 2,

            /* Denotes a closed connection */
            ClosedConnection = 8,

            /* Denotes a ping*/
            Ping = 9,

            /* Denotes a pong */
            Pong = 10
        }

        /// <summary>Gets an encoded websocket frame to send to a client from a string</summary>
        /// <param name="Message">The message to encode into the frame</param>
        /// <param name="Opcode">The opcode of the frame</param>
        /// <returns>Byte array in form of a websocket frame</returns>
        public static byte[] GetFrameFromString(string Message, EOpcodeType Opcode = EOpcodeType.Text)
        {
            byte[] response;
            byte[] bytesRaw = Encoding.Default.GetBytes(Message);
            byte[] frame = new byte[10];

            int indexStartRawData = -1;
            int length = bytesRaw.Length;

            frame[0] = (byte)(128 + (int)Opcode);
            if (length <= 125)
            {
                frame[1] = (byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (byte)126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (byte)127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new byte[indexStartRawData + length];

            int i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
        }

    }
}