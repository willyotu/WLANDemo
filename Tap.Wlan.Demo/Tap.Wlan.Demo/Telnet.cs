// Responsible: TEAM (asgeiver)
// Copyright:   Copyright 2015 Keysight Technologies.  All rights reserved. No 
//              part of this program may be photocopied, reproduced or translated 
//              to another program language without the prior written consent of 
//              Keysight Technologies.
using Keysight.Tap;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;


namespace TapPlugin.PaceUK
{
    internal enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    internal enum Options
    {
        SGA = 3
    }

    public class Telnet
    {
        Keysight.Tap.TraceSource log = Log.CreateSource("Chipset Connection");

        public static Telnet Login(string hostname, ushort port, string Username, string password, int timeoutMs = 1000)
        {

            Telnet telnet = new Telnet(hostname, port);

            telnet.Login(Username, password, timeoutMs);
            return telnet;
        }

        public const ushort DefaultPort = 23;

        private TcpClient tcpSocket;

        private int TimeOutMs = 10;

        public Telnet(string Hostname, int Port)
        {
            tcpSocket = new TcpClient(Hostname, Port);
        }

        public void Disconnect()
        {
            tcpSocket.Close();

        }

        public string Login(string Username, string Password, int LoginTimeOutMs)
        {
            int oldTimeOutMs = TimeOutMs;
            TimeOutMs = LoginTimeOutMs;
            try
            {
                string s = Read();
                if (!s.TrimEnd().EndsWith(":"))
                    throw new Exception("Failed to connect : no login prompt");
                WriteLine(Username);

                s += Read();
                if (!s.TrimEnd().EndsWith("#"))
                    throw new Exception("Failed to connect : no password prompt");

                return s;
            }
            finally
            {
                TimeOutMs = oldTimeOutMs;
            }
        }

        public void SendCmd(string cmd)
        {
            Stopwatch timer = Stopwatch.StartNew();

            WriteLine(cmd);
            while (!Read().Contains("#"))
            {
                if (timer.ElapsedMilliseconds > 1000)
                {
                    log.Warning(timer, "Command '{0}' timed out", cmd);
                    return;
                }
                TestPlan.Sleep(5); // Make it possible to break the testplan
            }
            log.Debug(timer, "Telnet >> {0}", cmd);
        }


        public string SendQuery(string cmd)
        {
            Stopwatch timer = Stopwatch.StartNew();

            StringBuilder response = new StringBuilder();

            WriteLine(cmd);

            while (true)
            {
                response.Append(Read());
                if (response.ToString().Contains("#"))
                {
                    break;
                }
                if (timer.ElapsedMilliseconds > 1000)
                {
                    log.Warning(timer, "Command '{0}' timed out", cmd);
                    return "";
                }
                TestPlan.Sleep(5); // Make it possible to break the testplan
            }
            log.Debug(timer, "Telnet >> {0}", cmd);
            log.Debug("Telnet << {0}", response.ToString());
            return response.ToString();
        }


        private void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        private void Write(string cmd)
        {
            //log.Debug("Telnet >> {0}",cmd);
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            Write(buf);
        }

        private void Write(byte[] data)
        {
            if (!tcpSocket.Connected)
            {
                throw new IOException("TCP Socket is not connected");
            }
            tcpSocket.GetStream().Write(data, 0, data.Length);
        }

        private string Read()
        {
            if (!tcpSocket.Connected) return null;
            StringBuilder sb = new StringBuilder();
            do
            {
                var data = ParseTelnet();
                sb.Append(Encoding.ASCII.GetString(data));
                System.Threading.Thread.Sleep(TimeOutMs);
            } while (tcpSocket.Available > 0);

            //log.Debug("Telnet << {0}", sb);
            return sb.ToString();
        }

        private byte[] ReadData()
        {
            return ParseTelnet();
        }

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        private byte[] ParseTelnet()
        {
            using (var memStr = new MemoryStream(16))
             {
                while (tcpSocket.Available > 0)
                {
                    int input = tcpSocket.GetStream().ReadByte();
                    switch (input)
                    {
                        case -1:
                            break;

                        case (int)Verbs.IAC:
                            // interpret as command
                            int inputverb = tcpSocket.GetStream().ReadByte();
                            if (inputverb == -1) break;
                            switch (inputverb)
                            {
                                case (int)Verbs.IAC:
                                    //literal IAC = 255 escaped, so append char 255 to string
                                    memStr.WriteByte((byte)inputverb);
                                    //sb.Append(inputverb);
                                    break;

                                case (int)Verbs.DO:
                                case (int)Verbs.DONT:
                                case (int)Verbs.WILL:
                                case (int)Verbs.WONT:
                                    // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                    int inputoption = tcpSocket.GetStream().ReadByte();
                                    if (inputoption == -1) break;
                                    tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                    if (inputoption == (int)Options.SGA)
                                        tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                    else
                                        tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                    tcpSocket.GetStream().WriteByte((byte)inputoption);
                                    break;

                                default:
                                    break;
                            }
                            break;

                        default:
                            memStr.WriteByte((byte)input);
                            //sb.Append((char)input);
                            break;
                    }
                }
                return memStr.ToArray();
            }
        }
    }

}
