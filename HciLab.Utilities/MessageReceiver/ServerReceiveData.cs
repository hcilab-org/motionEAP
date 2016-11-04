// <copyright file=ServerReceiveData.cs
// <copyright>
//  Copyright (c) 2016, University of Stuttgart
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the Software),
//  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
//  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <license>MIT License</license>
// <main contributors>
//  Markus Funk, Thomas Kosch, Sven Mayer
// </main contributors>
// <co-contributors>
//  Paul Brombosch, Mai El-Komy, Juana Heusler, 
//  Matthias Hoppe, Robert Konrad, Alexander Martin
// </co-contributors>
// <patent information>
//  We are aware that this software implements patterns and ideas,
//  which might be protected by patents in your country.
//  Example patents in Germany are:
//      Patent reference number: DE 103 20 557.8
//      Patent reference number: DE 10 2013 220 107.9
//  Please make sure when using this software not to violate any existing patents in your country.
// </patent information>
// <date> 11/2/2016 12:25:58 PM</date>

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HciLab.Utilities.MessageReceiver
{
    public class ServerReceiveData
    {
        
        private int m_Port = Int32.MinValue;
        private byte[] m_Bytes;

        // Incoming data from the client.
        public static string m_Data = null;


        private IPHostEntry m_IpHostInfo = null;
        private IPAddress m_IpAddress = null;
        private IPEndPoint m_LocalEndPoint = null;
        private Socket m_Listener = null;
        private Thread m_ListenerThread = null;


        public ServerReceiveData (int pPort) {
            m_Port = pPort;
            m_Bytes = new Byte[UtilitiesNetwork.BufferSize];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            m_IpHostInfo = Dns.Resolve(Dns.GetHostName());
            m_IpAddress = m_IpHostInfo.AddressList[0];
            m_LocalEndPoint = new IPEndPoint(m_IpAddress, pPort);

            // Create a TCP/IP socket.
            m_Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                      

            /*Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();*/
        }

        public void Start()
        {
            m_ListenerThread = new Thread(StartListener);
            m_ListenerThread.Start();
        }

        public void Stop()
        {
            m_ListenerThread.Abort();
        }

        private void StartListener()
        {
            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                m_Listener.Bind(m_LocalEndPoint);
                m_Listener.Listen(10);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            // Start listening for connections.
            while (true)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.
                Socket handler = m_Listener.Accept();
                m_Data = null;

                // An incoming connection needs to be processed.
                while (true)
                {
                    m_Bytes = new byte[UtilitiesNetwork.BufferSize];
                    int bytesRec = handler.Receive(m_Bytes);
                    m_Data += Encoding.UTF8.GetString(m_Bytes, 0, bytesRec);
                    if (m_Data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                // Show the data on the console.
                Console.WriteLine("Text received : {0}", m_Data);
                if (m_Data.Contains("<EOF>"))
                {
                    string m = (m_Data.Split(new string[] { "<EOF>" }, StringSplitOptions.RemoveEmptyEntries))[0];
                    OnResivedMessage(this, m);
                } 
                else
                    OnResivedMessage(this, m_Data);

                // Echo the data back to the client.
                byte[] msg = Encoding.UTF8.GetBytes(m_Data);

                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        public delegate void resivedMessageHandler(object pSource, String pMessage);

        public event resivedMessageHandler resivedMessage;

        public void OnResivedMessage(object pSource, String pMessage)
        {
            if (this.resivedMessage != null)
                resivedMessage(pSource, pMessage);
        }
    }
}
