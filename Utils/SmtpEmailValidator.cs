using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Utils
{
    public class SmtpEmailValidator
    {
        /// <summary>
        /// validates email address
        /// </summary>
        /// <param name="address">address to validation</param>
        /// <param name="hostname">SMTP server name</param>
        /// <param name="emailFromAddress">address of sender</param>
        /// <returns></returns>
        public static bool IsValidAddress(string address, string hostname, string emailFromAddress)
        {
            if (string.IsNullOrEmpty(hostname))
                return true; 
            
            if (string.IsNullOrEmpty(emailFromAddress))
                return true; 

            try
            {
                IPHostEntry IPhst = Dns.GetHostEntry(hostname);
                IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], 25);
                using (Socket s = new Socket(endPt.AddressFamily,
                                             SocketType.Stream, ProtocolType.Tcp))
                {
                    s.Connect(endPt);

                    //Attempting to connect
                    if (!CheckResponse(s, SMTPResponse.CONNECT_SUCCESS))
                    {
                        // Make sure it would work fine with SMTP being not-accessible
                        return true;
                    }

                    //HELO server
                    SendData(s, string.Format("HELO {0}\r\n", Dns.GetHostName()));
                    if (!CheckResponse(s, SMTPResponse.GENERIC_SUCCESS))
                    {
                        // Make sure it would work fine with SMTP being not-accessible
                        return true;
                    }

                    //Identify yourself
                    //Servers may resolve your domain and check whether 
                    //you are listed in BlackLists etc.
                    SendData(s, string.Format("MAIL From: {0}\r\n", emailFromAddress));
                    if (!CheckResponse(s, SMTPResponse.GENERIC_SUCCESS))
                    {
                        // Make sure it would work fine with SMTP being not-accessible
                        return true;
                    }

                    //Attempt Delivery (I can use VRFY, but most 
                    //SMTP servers only disable it for security reasons)
                    SendData(s, string.Format("rcpt to: {0}\r\n", address));
                    bool result = CheckResponse(s, SMTPResponse.GENERIC_SUCCESS);

                    SendData(s, "quit\r\n");

                    return result;
                }
            }
            catch (Exception)
            {
                // Make sure it would work fine with SMTP being not-accessible
                return true;
            }
        }

         

        /// <summary>
        /// validates SMTP server
        /// </summary>
        /// <param name="hostname">SMTP server name</param>
        /// <returns>true if server is valid</returns>
        public static bool IsValidHost(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return false;
            try
            {
                IPHostEntry IPhst = Dns.GetHostEntry(hostname);
                IPEndPoint endPt = new IPEndPoint(IPhst.AddressList[0], 25);
                using (Socket s = new Socket(endPt.AddressFamily,
                                             SocketType.Stream, ProtocolType.Tcp))
                {
                    s.Connect(endPt);

                    //Attempting to connect
                    if (!CheckResponse(s, SMTPResponse.CONNECT_SUCCESS))
                    {
                        // Make sure it would work fine with SMTP being not-accessible
                        return false;
                    }

                    //HELO server
                    SendData(s, string.Format("HELO {0}\r\n", Dns.GetHostName()));
                    if (!CheckResponse(s, SMTPResponse.GENERIC_SUCCESS))
                    {
                        // Make sure it would work fine with SMTP being not-accessible
                        return false;
                    }

                    SendData(s, "quit\r\n");

                    return true;
                }
            }
            catch (Exception)
            {
                // Make sure it would work fine with SMTP being not-accessible
                return false;
            }
        }

        private static void SendData(Socket s, string msg)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(msg);
            s.Send(bytes, 0, bytes.Length, SocketFlags.None);
        }

        private static bool CheckResponse(Socket s, SMTPResponse responseExpected)
        {
            byte[] bytes = new byte[1024];
            int count = 0;
            while (s.Available == 0)
            {
                // Make sure it would work fine with SMTP being not-accessible
                if (++count >= 50)
                    return true;
                Thread.Sleep(100);
            }

            s.Receive(bytes, 0, s.Available, SocketFlags.None);
            string sResponse = Encoding.ASCII.GetString(bytes);
            int response = Convert.ToInt32(sResponse.Substring(0, 3));
            return response == (int) responseExpected;
        }

        #region Nested type: SMTPResponse

        private enum SMTPResponse
        {
            CONNECT_SUCCESS = 220,
            GENERIC_SUCCESS = 250,
            DATA_SUCCESS = 354,
            QUIT_SUCCESS = 221
        }

        #endregion
    }
}