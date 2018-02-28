// Author: MyName
// Copyright:   Copyright 2018 Keysight Technologies
//              You have a royalty-free right to use, modify, reproduce and distribute
//              the sample application files (and/or any modified version) in any way
//              you find useful, provided that you agree that Keysight Technologies has no
//              warranty, obligations or liability for any sample application files..

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Keysight.Tap;
using System.Text.RegularExpressions;
using static Tap.Wlan.Demo.TransmitterStep;
using static Tap.Wlan.Demo.ReceiverStep;

namespace Tap.Wlan.Demo
{
    [Display("BCM4360", Group: "Demo", Description: "Add a description here")]
    [ShortName("MyDUT")]
    public class BCM4360 : Dut
    {
        private Telnet telnet;
        public Telnet Telnet
        {
            get
            {
                return telnet;
            }
        }

        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change.
        // Creates Text Box GUI for DUT's IP Address
        [DisplayName("Telnet\\IP Address")]
        public string IpAddress { get; set; }

        // Creates Text Box GUI for DUT's Telnet Port
        [DisplayName("Telnet\\Port")]
        public int PortTelnet { get; set; }

        // Creates Text Box GUI for DUT's Telnet Login
        [DisplayName("Telnet\\Login")]
        public string Login { get; set; }

        // Creates Text Box GUI for DUT's Telnet Password
        [DisplayName("Telnet\\Password")]
        public string Password { get; set; }
        #endregion

        #region BCM4360 Telnet Login
        // Initializes a new instance of this DUT class.
        public BCM4360()
        {
            Name = "Broadcom 4360 WLAN device";
            PortTelnet = 23;
            Login = "root";
            Password = "";
            IpAddress = "192.168.1.99"; //The IP of the scope... watch out for conflicts

        }

        // Opens a connection to the DUT represented by this class
        public override void Open()
        {
            // TODO: establish connection to DUT here

            try
            {
                // Uses IpAddress Variable and PortTelnet from DUT GUI to create connection to Set Top Box
                // IpAddress Set to 192.168.1.97 and Telnet Port 23 Ipaddress may be changed at DUT GUI

                telnet = new Telnet(IpAddress, PortTelnet);

                // response returnes  IpAddress  Password  which is ""  means no password required and a timeout of 150 msecs

                string response = telnet.Login(Login, Password, 150);

                // # at the end of the response string variable determine connection to Set Top Box is made.

                if (!response.Trim().EndsWith("#"))
                    throw new Exception("Connection to DUT [" + IpAddress + ":" + PortTelnet + "] failed!");
                //Telnet.SendCmd("sh");

                // logs connection is made at TAP GUI Debug Window

                Log.Debug("Connected to DUT on IP: {0}", IpAddress);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect: " + ex.ToString());
            }
            this.IsConnected = true;
        }

        // Closes the connection made to the DUT represented by this class
        public override void Close()
        {
            // TODO: close connection to DUT
            telnet.Disconnect();
            this.IsConnected = false;
        }

        // Sending Command to telnet
        public void sendcmd(string cmd)
        {
            telnet.SendCmd(cmd);
        }

        // Read back from counter number of packets received.
        Regex searchExpression = new Regex(@"rxdfrmucastmbss (\d+) rxmfrmucastmbss (\d+)");
        internal Tuple<int, int> RxPackagesReceived()
        {
            string respons = Telnet.SendQuery("wl counters");
            Match m = searchExpression.Match(respons);
            if (m.Success)
            {
                int acks = int.Parse(m.Groups[1].Value);
                int nacks = int.Parse(m.Groups[2].Value);

                return new Tuple<int, int>(acks, nacks);
            }
            return null;
        }

        #endregion

        #region WL Commands 4360
        // WL commands for BCM 4360 Transmitter
        public void Initialize4360tx(string mode, int antenna, string ISM_Band, Rate_Setting rate, double OFDM_rate, double BW, double channel, double pwrdB)
        {
            sendcmd("wl pkteng_stop tx");
            sendcmd("wl down");
            if (mode == "a" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "b" | mode == "g" & BW == 20)
            {
                sendcmd("wl band b");
            }
            else if (mode == "n" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "n" & BW == 40)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 40)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 80)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 160)
            {
                sendcmd("wl band a");
            }
            sendcmd("wl bw_cap 5g 7");
            sendcmd("wl mpc 0");
            sendcmd("wl tempsense_disable 1");
            sendcmd("wl phy_watchdog 0");
            sendcmd("wl frameburst 1");
            sendcmd("wl scansuppress 1");
            sendcmd("wl country ALL");
            sendcmd("wl bi 65535");
            sendcmd("wl ampdu 1");
            string ant = ("wl txchain" + antenna.ToString());
            sendcmd(ant);
            string OFDM = ("wl " + ISM_Band.ToString() + "_rate -" + rate.ToString() + " " + OFDM_rate.ToString() + " -b " + BW.ToString());
            sendcmd(OFDM);
            string chan = ("wl chanspec " + channel.ToString());
            sendcmd(chan);
            sendcmd("wl ssid \"\"");
            sendcmd("wl up");
            sendcmd("wl interference 0");
            string pow = ("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd(pow);
            sendcmd("wl pkteng_start 00:22:33:44:55:66 tx 100 1500 0");
            sendcmd("wl phy_forcecal");

        }

        // WL commands for BCM 4366 Receiver
        public void Initialize4360rx(string mode, double BW, int antenna, string ISM_Band, RateSettingRX rate, double OFDM_rate, double pwrdB, int channel)
        {
            sendcmd("wl down");
            sendcmd("wl cur_etheraddr 00:11:22:33:44:55");
            sendcmd("wl ap 0");
            sendcmd("wl mpc 0");
            sendcmd("wl phy_watchdog 0");
            sendcmd("wl tempsense_disable 1");
            sendcmd("wl legacylink 1");
            sendcmd("wl scansuppress 1");
            sendcmd("wl interference 0");
            if (mode == "a" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "b" | mode == "g" & BW == 20)
            {
                sendcmd("wl band b");
            }
            else if (mode == "n" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "n" & BW == 40)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 20)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 40)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 80)
            {
                sendcmd("wl band a");
            }
            else if (mode == "ac" & BW == 160)
            {
                sendcmd("wl band a");
            }
            sendcmd("wl country all");
            sendcmd("wl bi 65535");
            sendcmd("wl ampdu 1");
            string ant = ("wl txchain 1");
            sendcmd(ant);
            sendcmd("wl rxchain " + antenna.ToString());
            string OFDM = ("wl " + ISM_Band.ToString() + "_rate -" + rate.ToString() + " " + OFDM_rate.ToString());
            sendcmd(OFDM);
            string chan = ("wl chanspec " + channel.ToString());
            sendcmd(chan);
            sendcmd("wl up");
            sendcmd("wl join aaa imode adhoc");
            string pow = ("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd(pow);
            sendcmd("wl disassoc");
            sendcmd("wl phy_forcecal 1");
            string respons = Telnet.SendQuery("wl reset_cnts");
        }

        // Sets power level in Chipsets for CHP measurement
        public void SetPowerLevel(double pwrdB)
        {
            string pow = ("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd(pow);
        }

        public double ChooseFrequency(int bw, int channel)
        {
            // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of chipset and Analyser 
            double freq;
            if (bw == 20)
            {
                freq = WLanChannels.ChannelToFrequencyMHz(channel);
            }
            else if (bw == 40)
            {
                freq = WLanChannels.ChannelToFrequencyMHz(channel) + 10;
            }
            else
            {
                freq = WLanChannels.ChannelToFrequencyMHz(channel) + 30;
            }
            return freq;
        }

        // Shuts down chipset after measurement
        public void BCM4360ShutDown()
        {
            sendcmd("wl pkteng_stop tx");
            sendcmd("wl down");
        }
        #endregion

    }
}

