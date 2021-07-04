// Responsible: TEAM (asgeiver)
// Copyright:   Copyright 2015 Keysight Technologies.  All rights reserved. No 
//              part of this program may be photocopied, reproduced or translated 
//              to another program language without the prior written consent of 
//              Keysight Technologies.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tap.Wlan.Demo

{
    public enum WLanStandard
    {
        A, B, G,
        N20, N40,
        Ac20, Ac40, Ac80, Ac160
    }

    public enum FiveGigChannels
    {
        // Commented out Japan channels because the channel to freq converter does not recognise them.   
        //ch183 = 183, ch184 = 184, ch185 = 185, ch187 = 187, ch188 = 188, ch189 = 189, 
        //ch192 = 192, ch196 = 196, ch7 = 7, ch8 = 8, ch9 = 9, ch11 = 11, ch12 = 12, 
        //ch16 = 16, ch34 = 34,


        ch36 = 36, ch38 = 38, ch40 = 40, ch42 = 42,
        ch44 = 44, ch46 = 46, ch48 = 48, ch52 = 52, ch56 = 56, ch60 = 60, ch64 = 64,
        ch100 = 100, ch104 = 104, ch108 = 108, ch112 = 112, ch116 = 116, ch120 = 120,
        ch124 = 124, ch128 = 128, ch132 = 132, ch136 = 136, ch140 = 140, ch149 = 149,
        ch153 = 153, ch157 = 157, ch161 = 161, ch165 = 165

    }

    public class WLanChannels
    {



        public struct StandardInfo
        {
            public string StandardName;
            public int BandwidthMHz;
        }

        static public StandardInfo GetStdInfo(WLanStandard std)
        {
            return new StandardInfo
            {
                StandardName = WLanStdToString(std),
                BandwidthMHz = WLanStdToBandwidthMHz(std)

            };
        }

        public static int WLanStdToBandwidthMHz(WLanStandard std)
        {
            switch (std)
            {
                case WLanStandard.A:
                case WLanStandard.Ac20:
                case WLanStandard.N20:
                    return 20;
                case WLanStandard.B:
                    return 22;
                case WLanStandard.N40:
                case WLanStandard.Ac40:
                    return 40;
                case WLanStandard.Ac80:
                    return 80;
                case WLanStandard.Ac160:
                    return 160;
            }
            return -1;
        }

        public static string WLanStdToString(WLanStandard std)
        {
            switch (std)
            {

                case WLanStandard.A:
                    return "AG";
                case WLanStandard.B:
                    return "BG";
                case WLanStandard.N20:
                    return "N20";
                case WLanStandard.N40:
                    return "N40";
                case WLanStandard.Ac20:
                    return "AC20";
                case WLanStandard.Ac40:
                    return "AC40";
                case WLanStandard.Ac80:
                    return "AC80";
                case WLanStandard.Ac160:
                    return "AC160";

            }
            throw new Exception("Unknown WLAN standard: " + std);
        }

        public static double ChannelToFrequency(int channel)
        {
            double frequency = 1e6 * ChannelToFrequencyMHz(channel);
            return frequency;
        }

        public static int ChannelToFrequencyMHz(int channel)
        {
            int fMHz = 2412;
            if (IsChannel11bETSI(channel))
                fMHz = 2407 + (5 * channel);
            else if (channel == 14)
                fMHz = 2484;
            else if (IsChannel11a(channel))
                fMHz = 5000 + (5 * channel);
            else
            {
                throw new Exception("Invalid channel: " + channel);
            }
            return fMHz;
        }

        public static int FrequencyToChannel(double frequency)
        {
            int channel = FrequencyMHzToChannel((int)(0.49 + frequency / 1e6));
            return channel;
        }

        public static int FrequencyMHzToChannel(int frequency_MHz)
        {
            int channel = 1;
            if (IsFrequency11bETSI(frequency_MHz))
                channel = (frequency_MHz - 2407) / 5;
            else if (frequency_MHz == 2484)
                channel = 14;
            else if (IsFrequency11a(frequency_MHz))
                channel = (frequency_MHz - 5000) / 5;
            else
            {
                throw new Exception("Invalid frequency: " + frequency_MHz);
            }
            return channel;
        }

        public static bool IsChannel11bETSI(int channel)
        {
            bool result = (1 <= channel) && (channel <= 13);
            return result;
        }

        public static bool IsChannel11a(int channel)
        {
            bool result =
                IsChannel11a(channel, 36, 64, 0) ||  // lower and middle bands
                IsChannel11a(channel, 100, 144, 0) ||  // H band 144 added for 11ac
                IsChannel11a(channel, 149, 161, 1) ||  // upper band
                IsChannel11a(channel, 165, 165, 1);    // ISM band
            return result;
        }

        public static bool IsChannel11a(int channel, int lower, int upper, int remainder)
        {
            bool result =
                (lower <= channel) && (channel <= upper) && (channel % 2 == remainder);
            return result;
        }

        public static bool IsFrequency11a(int frequency_MHz)
        {
            bool result =
                IsFrequency11a(frequency_MHz, 5180, 5320, 0) ||  // lower and middle bands
                IsFrequency11a(frequency_MHz, 5500, 5700, 0) ||  // H band
                IsFrequency11a(frequency_MHz, 5745, 5805, 5) ||  // upper band
                IsFrequency11a(frequency_MHz, 5825, 5825, 5);    // ISM band
            return result;
        }

        public static bool IsFrequency11a(int frequency_MHz, int lower, int upper, int remainder)
        {
            bool result =
                (lower <= frequency_MHz) && (frequency_MHz <= upper) && (frequency_MHz % 20 == remainder);
            return result;
        }

        public static bool IsFrequency11bETSI(int frequency_MHz)
        {
            bool result = (2412 <= frequency_MHz) && (frequency_MHz <= 2472) && (frequency_MHz % 5 == 2);
            return result;
        }

        /// <summary>
        /// Get the 40 MHz BW sideband indicator value to pass to the WLM API function wlmSetChannelSpecSet().
        /// 
        /// When in 802.11ac mode with 40 MHz bandwidth, the WLM API only accepts a channel value which is either the corresponding upper 20 MHz BW channel value
        /// or the lower 20 MHz BW channel value, not a "center" channel value. But depending on the channel choosen, the sideband indicator must be provided with
        /// the proper value. For example, in the 5170 to 5330 MHz frequency range, for the first 40 MHz "chunk", we could use either channel number 40 with sideband value 1 (upper)
        /// or channel number 36 with sideband value -1 (lower). 
        /// </summary>
        /// <param name="channel">20 MHz BW channel number</param>
        /// <param name="bandwidth_MHz">Bandwidth in MHz: 20 (20 MHz) or 40 (40 MHz)</param>
        /// <returns>sideband indicator: -1 = lower, 0 = none, 1 = upper</returns>
        public static int GetSideband11a(int channel, int bandwidth_MHz)
        {
            bool is20mBw = (20 == bandwidth_MHz);
            if (IsChannel11a(channel, 36, 64, 0) || IsChannel11a(channel, 100, 144, 0))
            {
                return is20mBw ? 0 : (channel % 8 == 0) ? 1 : -1;
            }
            else if (IsChannel11a(channel, 149, 161, 1))
            {
                return is20mBw ? 0 : (channel % 8 == 1) ? 1 : -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the measurement frequency from the channel number and the channel bandwidth.
        /// </summary>
        /// <param name="wlan_std">WLAN Standard</param>
        /// <param name="channel">Channel number</param>
        /// <returns></returns>
        public static double ChannelToMeasurementFrequency(WLanStandard wlan_std, int channel)
        {
            int bandwidth_MHz = GetStdInfo(wlan_std).BandwidthMHz;
            if (IsChannel11a(channel))
            {
                switch (bandwidth_MHz)
                {
                    case 40:
                        channel = channel - 2 * GetSideband11a(channel, 40);
                        break;
                    case 80:
                        // TODO:
                        break;
                    case 160:
                        // TODO:
                        break;
                    case 20:
                    default:
                        break;
                }
            }
            double frequency = 1e6 * ChannelToFrequencyMHz(channel);
            return frequency;
        }
    }
}
