// Author: MyName
// Copyright:   Copyright 2018 Keysight Technologies
//              You have a royalty-free right to use, modify, reproduce and distribute
//              the sample application files (and/or any modified version) in any way
//              you find useful, provided that you agree that Keysight Technologies has no
//              warranty, obligations or liability for any sample application files.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Keysight.Tap;

namespace Tap.Wlan.Demo
{
    [Display("MinInputSensitivityTestStep", Groups: new[] { "WLAN Demo", "Receiver Measurements" }, Description: "Insert a description here")]
    [AllowAsChildIn(typeof(ReceiverStep))]
    public class MinInputSensitivityTestStep : TestStep
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        [Unit("dBm", UseEngineeringPrefix: true)]
        [Display("Amplitude Upper Limit", Group: "Limits")]
        public Double amplitudeULimit { get; set; }

        [Unit("dBm", UseEngineeringPrefix: true)]
        [Display("Amplitude Lower Limit", Group: "Limits")]
        public Double amplitudeLLimit { get; set; }

        #endregion

        public MinInputSensitivityTestStep()
        {
            // ToDo: Set default values for properties / settings.
            amplitudeULimit = -15;
            amplitudeLLimit = -85;

        }

        public override void Run()
        {
            MinInputSensitivityTestRun();
        }

        private void MinInputSensitivityTestRun()
        {
            BCM4366 chipset = GetParent<ReceiverStep>().bcm4366;
            SGInstrument signalGenerator = GetParent<ReceiverStep>().signalGenerator;
            int bandwidth = GetParent<ReceiverStep>().bandwidth;
            double amplitude = GetParent<ReceiverStep>().amplitude;
            double rms = GetParent<ReceiverStep>().rms;
            int repeat = GetParent<ReceiverStep>().repeat;
            int frames = GetParent<ReceiverStep>().frames;
            int perLimit = GetParent<ReceiverStep>().perLimit;
            string arbName = GetParent<ReceiverStep>().arbName;

            // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of Set Top Box and Analyser 
            int channel = GetParent<ReceiverStep>().channel;
            double frequency = WLanChannels.ChannelToFrequencyMHz(channel);
            Log.Info("  Running Minimum Input Sensitivity Test ");
            signalGenerator.AmplitudeLevel(amplitude);
            Log.Info("  Initial Power level : {0,0:0.00} dBm", amplitude);
            Tuple<int, int> before, after;
            double ack, perError;
            PERTest(chipset, signalGenerator, amplitude, rms, repeat, frames, arbName, frequency, out before, out after, out ack, out perError);


            // Check if PER is within limit. If yes, decrease the Signal Generator power. If no, increase the Signal Generator power. 
            if (perError < perLimit)
            {
                double pwr = amplitude;
                while (perError < perLimit & pwr > amplitudeLLimit)
                {
                    pwr = pwr - 1;
                    PERTest(chipset, signalGenerator, amplitude, rms, repeat, frames, arbName, frequency, out before, out after, out ack, out perError);

                    if (perError >= perLimit)
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        UpgradeVerdict(Verdict.Pass);
                        MinInputSensitivityResults(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                    MinInputSensitivityResults(perError, pwr, ack);
                    
                }
            }
            else
            {
                double pwr = amplitude;
                while (perError > perLimit & pwr < amplitudeULimit)
                {
                    pwr = pwr + 1;
                    PERTest(chipset, signalGenerator, amplitude, rms, repeat, frames, arbName, frequency, out before, out after, out ack, out perError);

                    if (perError <= perLimit)
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        UpgradeVerdict(Verdict.Pass);
                        MinInputSensitivityResults(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                    MinInputSensitivityResults(perError, pwr, ack);
                }
            }

        }

        private static void PERTest(BCM4366 chipset, SGInstrument signalGenerator, double amplitude, double rms, int repeat, int frames, string arbName, double frequency, out Tuple<int, int> before, out Tuple<int, int> after, out double ack, out double perError)
        {
            signalGenerator.AmplitudeLevel(amplitude);
            TestPlan.Sleep(200);
            before = chipset.RxPackagesReceived();
            TestPlan.Sleep(500);
            signalGenerator.PlayWaveform(frequency, amplitude, arbName, rms, repeat);
            TestPlan.Sleep(1000);
            after = chipset.RxPackagesReceived();
            ack = after.Item1 - before.Item1;
            perError = (1 - (double)ack / (int)frames) * 100;
        }

        private void MinInputSensitivityResults(double perError, double pwr, double ack)
        {
            var per = new double[] { perError };
            var powerLevel = new double[] { pwr };
            var ackPackets = new double[] { ack };
            Results.PublishTable("Minimum Sensitivity Result", new List<string> { "Error Rate", "Power Level", "No. of Packets Received" }, per, powerLevel, ackPackets);
        }

    }
}
