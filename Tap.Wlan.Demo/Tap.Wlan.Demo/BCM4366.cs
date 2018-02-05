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
    [Display("BCM4366", Group: "Demo", Description: "Add a description here")]
    [ShortName("MyDUT")]
    public class BCM4366 : Dut
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change.
        // Creates Text Box GUI for DUT's IP Address
        Telnet telnet = null;

        [DisplayName("Telnet\\IP Address")]
        public string IpAddress { get; set; }

        // Creates Text Box GUI for DUT's Telnet Port

        [DisplayName("Telnet\\Port")]
        public int Port { get; set; }

        #endregion

        #region BCM4366 Telnet Login
        // Initializes a new instance of this DUT class.
        public BCM4366()
        {
            // ToDo: Set default values for properties / settings.
            Name = "Broadcom 4366 WLAN device";
            Port = 23;
            IpAddress = "10.0.0.16"; //The IP of the DUT... watch out for conflicts

        }

        // Opens a connection to the DUT represented by this class
        public override void Open()
        {
            telnet = new Telnet(IpAddress, Port);
            try
            {
                // ask for current directory. Should be just "/" it might contain other things though
                var response = telnet.SendQuery("pwd");
                Log.Info("Result '{0}'", response);
                // Just for testing.

                // init4366tx(mode, antenna, pwrdB);

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
        //rxdtucastmbss 0 rxdtocast 0
        Regex searchExpression1 = new Regex(@"rxdtucastmbss (\d+)");
        Regex searchExpression2 = new Regex(@"rxdtocast (\d+)");
        internal Tuple<int, int> RxPackagesReceived()
        {
            string respons = telnet.SendQuery("wl counters");
            Match m1 = searchExpression1.Match(respons);
            Match m2 = searchExpression2.Match(respons);
            if (m1.Success && m2.Success)
            {
                int acks = int.Parse(m1.Groups[1].Value);
                int nacks = int.Parse(m2.Groups[1].Value);

                return new Tuple<int, int>(acks, nacks);
            }
            return null;
        }

        #endregion

        #region WL Commands 4366
        // WL commands for BCM 4366 Transmitter
        public void init4366tx(string mode, int antenna, string ISM_Band, Rate_Setting rate, double OFDM_rate, double BW, double channel, double pwrdB)
        {
            sendcmd("wl down");
            sendcmd("wl up");
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
            string ant = ("wl txchain" + antenna.ToString());
            sendcmd(ant);
            sendcmd("wl rxchain 1");
            sendcmd("wl " + ISM_Band.ToString() + "_rate -" + rate.ToString() + " " + OFDM_rate.ToString() + " -b " + BW.ToString());
            sendcmd("wl chanspec " + ISM_Band.ToString() + "" + channel.ToString() + "/" + BW.ToString());
            sendcmd("wl ssid \"\"");
            sendcmd("wl up");
            sendcmd("wl interference 0");
            sendcmd("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd("wl pkteng_start 00:11:22:33:44:55:66 tx 25 1500 0");
            sendcmd("wl phy_forcecal 1");

        }


        // WL commands for BCM 4366 Receiver
       public void init4366rx(string mode, double BW, int antenna, string ISM_Band, RateSettingRX rate, double OFDM_rate, double pwrdB, int channel)
        {
            sendcmd("wl down");
            sendcmd("wl cur_etheraddr 00:11:22:33:44:55:66");
            sendcmd("wl ap 0");
            sendcmd("wl mpc 0");
            sendcmd("wl phy_watchdog 0");
            sendcmd("wl tempsense_disable 1");
            sendcmd("wl legacylink 1");
            sendcmd("wl scansuppress 1");
            sendcmd("wl frameburst 1");
            sendcmd("wl interference 0");
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
            sendcmd("wl country all");
            sendcmd("wl bi 65535");
            sendcmd("wl ampdu 1");
            sendcmd("wl txchain 1");
            sendcmd("wl rxchain " + antenna.ToString());
            sendcmd("wl " + ISM_Band.ToString() + "_rate -" + rate.ToString() + " " + OFDM_rate.ToString());
            sendcmd("wl chanspec " + ISM_Band.ToString() + "" + channel.ToString() + "/" + BW.ToString());
            sendcmd("wl up");
            sendcmd("wl join aaa imode adhoc");
            sendcmd("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd("wl disassoc");
            sendcmd("wl phy_forcecal 1");
            string respons = telnet.SendQuery("wl reset_cnts");

        }

        // Sets power level in Chipsets for CHP measurement
        public void bcm4366SetPowerLevel(double pwrdB)
        {
            string pow = ("wl txpwr1 -o -q " + pwrdB.ToString());
            sendcmd(pow);
        }

        // Shuts down chipset after measurement
        public void BCM4366_ShutDown()
        {
            sendcmd("wl pkteng_stop tx");
            sendcmd("wl down");
        }

        #endregion

        // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of Set Top Box and Analyser 
        public double choosefrequency(int bw, int channel)
        {
            // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of Set Top Box and Analyser 
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


    }
}
