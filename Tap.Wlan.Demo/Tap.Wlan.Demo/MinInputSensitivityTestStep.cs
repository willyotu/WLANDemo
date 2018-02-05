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
        [Display("Amplitude Lower Limit", Group:"Limits")]
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
            BCM4366 chipset = GetParent<TransmitterStep>().bcm4366;
            SGInstrument signalGenerator = GetParent<ReceiverStep>().signalGenerator;
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            double triggerLevel = GetParent<TransmitterStep>().triggerLevel;
            int bandwidth = GetParent<TransmitterStep>().bandwidth;
            double amplitude = GetParent<ReceiverStep>().amplitude;
            double rms = GetParent<ReceiverStep>().rms;
            int repeat = GetParent<ReceiverStep>().repeat;
            int frames = GetParent<ReceiverStep>().frames;
            int perLimit = GetParent<ReceiverStep>().perLimit;
            string arbName = GetParent<ReceiverStep>().arbName;

            // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of Set Top Box and Analyser 
            int channel = GetParent<ReceiverStep>().channel;
            double frequency = WLanChannels.ChannelToFrequencyMHz(channel);
            signalGenerator.Set_Amplitude(amplitude);

            Log.Info("  Running Minimum Input Sensitivity Test ");
            Log.Info("  Initial Power level : {0,0:0.00} dBm", amplitude);
            TestPlan.Sleep(200);
            Tuple<int, int> before = chipset.RxPackagesReceived();  //Get number of received packages (as a reference)
            TestPlan.Sleep(500);
            signalGenerator.Play_Waveform(frequency,amplitude, arbName, rms, repeat);
            TestPlan.Sleep(1000);
            Tuple<int, int> after = chipset.RxPackagesReceived(); //Get number of received packages after waveform is completed

            double ack = after.Item1 - before.Item1;

            double perError = (1 - (double)ack / (int)frames) * 100;


            // Check if PER is within limit. If yes, decrease the Signal Generator power. If no, increase the Signal Generator power. 
            if (perError <perLimit)
            {
                double pwr = amplitude;
                while (perError < perLimit & pwr > amplitudeLLimit)
                {
                    pwr = pwr - 1;
                    //PERTest(ref before, ref after, ref ack, ref perError, pwr, frequency);

                    if (perError >= perLimit)
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        Verdict = Verdict.Pass;
                       // StoreResults_TAP(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                       Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                       Log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                       Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                  //  StoreResults_TAP(perError, pwr, ack);
                }
            }
            else
            {
                double pwr = amplitude;
                while (perError >perLimit & pwr < amplitudeULimit)
                {
                    pwr = pwr + 1;
                    //PERTest(ref before, ref after, ref ack, ref perError, pwr, frequency);

                    if (perError <= perLimit)
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        Verdict = Verdict.Pass;
                       // StoreResults_TAP(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                        Log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        Log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                        Log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                    //StoreResults_TAP(perError, pwr, ack);
                }
            }

            // Initialise CHP settings  

            //MinInputSensitivityResults(xAPP);
        }

        private void MinInputSensitivityResults(SAInstrument xAPP)
        {
            // Return CHP in dBm
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            SAMeasurements.CHPResult chp = xAPP.MeasureChannelPower(average, numberOfAverages);
            var chpResult = new double[] { chp.PowerResult };
            
            var chpUnit = new string[] {
                 "dBm"
            };
            Results.PublishTable("CHP Result", new List<string> { "CHP", "CHP Unit" }, chpResult, chpUnit);
        }


        //private void PER_Test(ref Tuple<int, int> before, ref Tuple<int, int> after, ref double ack, ref double perError, double pwr, double frequency)
        //{
        //    Generator.Set_Amplitude(pwr);
        //    TestPlan.Sleep(200);
        //    before = BCM4366.RxPackagesReceived();  //Get number of received packages (as a reference)
        //    TestPlan.Sleep(500);
        //    Generator.Play_Waveform(frequency, RxParent.Amptd, RxParent.ArbName, RxParent.rms, RxParent.repeat);
        //    TestPlan.Sleep(1000);
        //    after = BCM4366.RxPackagesReceived(); //Get number of received packages after waveform is completed

        //    ack = after.Item1 - before.Item1;

        //    perError = (1 - (double)ack / (int)RxParent.frames) * 100;
        //}

        //private void StoreResults_TAP(double perError, double pwr, double ack)
        //{
        //    TestStepResultType type = new TestStepResultType();
        //    type.DimensionTitles.Add("PER");
        //    type.DimensionTitles.Add("Minimum Input Sensitivity Level");
        //    type.DimensionTitles.Add("No. of Packets Sent");
        //    type.DimensionTitles.Add("No. of Packets Received");
        //    type.DimensionTitles.Add("Pass/Fail");
        //    type.Name = "PER Results";
        //    TestStepResult res = new TestStepResult();
        //    res.Type = type;
        //    res.Doubles.Add(perError);
        //    res.Doubles.Add(pwr);
        //    res.Doubles.Add(RxParent.frames);
        //    res.Doubles.Add(ack);
        //    if (Verdict == Verdict.Pass)
        //    {
        //        res.Strings.Add("Pass");
        //    }
        //    else
        //    {
        //        res.Strings.Add("Fail");
        //    }


        //    this.Results.StoreResult(res);
        //}

    }
}
