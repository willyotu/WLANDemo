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
    [Display("Spectrual Emission Mask", Groups: new[] { "WLAN Demo", "Transmitter Measurements" }, Description: "Insert a description here")]
    [AllowAsChildIn(typeof(TransmitterStep))]
    public class SEMTestStep : TestStep
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        // public Instrument xAPP;
        #endregion
        public SEMTestStep()
        {
            // ToDo: Set default values for properties / settings.
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        public override void Run()
        {
            // ToDo: Add test case code here
            SEMTestRun();
            UpgradeVerdict(Verdict.Pass);
        }

        private void SEMTestRun()//System.Diagnostics.TraceSource log,  double frequency
        {
            SAMeasurements sem = new SAMeasurements();
            BCM4366 chipset = GetParent<TransmitterStep>().bcm4366;
            SAInstrument xAPP = GetParent<TransmitterStep>().signalAnalyzer;
            double rfbLevel = GetParent<TransmitterStep>().absTriggerLevel;
            int bandwidth = GetParent<TransmitterStep>().bw;
            int channel = GetParent<TransmitterStep>().channel;
            string mode = GetParent<TransmitterStep>().mode;

            //Set set-top power level to pwrdB value
            double chipsetPowerLevel = GetParent<TransmitterStep>().pwrdB;
            chipset.bcm4366SetPowerLevel(chipsetPowerLevel);

            // Initialise SEM settings  
            // Select frequency based on channel
            double frequency = chipset.choose_freq(bandwidth, channel);
            xAPP.centerFrequency = (frequency * 1000000).ToString();
            xAPP.MeasurementMode();
            xAPP.WlanMode(bandwidth, mode.ToString());
            xAPP.SEMConfigure();
            xAPP.RFBLevel = rfbLevel.ToString();
            //xAPP.SEMTriggerSource();
            xAPP.OptimizePowerRange();

            // Returns SEM Pass/Fail Test results
            SEMLimitTestResults(xAPP);

            // Returns SEM data results
            SEMDataResults(xAPP);
        }

        private void SEMLimitTestResults(SAInstrument xAPP)
        {
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            SAMeasurements.SEMLimitTest semLimitTest = xAPP.MeasureSEMLimit(average,numberOfAverages);
            var SEMLimit = new string[] {
                "Negative Offset Frequency (A) ",
                "Positive Offset Frequency (A) ",
                "Negative Offset Frequency (B) ",
                "Positive Offset Frequency (B) "
                };


            var SEMResult = new bool[] {
                semLimitTest.NegOFFSFREQA,
                semLimitTest.PosOFFSFREQA,
                semLimitTest.NegOFFSFREQB,
                semLimitTest.PosOFFSFREQB
                };
            Results.PublishTable("SEM Limit Test", new List<string> { "SEM Offset Limit", "SEMResult"}, SEMLimit, SEMResult);
        }
        private void SEMDataResults(SAInstrument xAPP)
        {
            SAMeasurements.SEM_Data SEM_Data = xAPP.MeasureSEMData();
            var SEMDataSettings = new string[] {
                 "LowerAbsPowerA  ",
                 "LowerDeltaLimitA",
                 "LowerFreqA      ",
                 "UpperAbsPowerA  ",
                 "UpperDeltaLimitA",
                 "UpperFreqA      ",
                 "LowerAbsPowerB  ",
                 "LowerDeltaLimitB",
                 "LowerFreqB      ",
                 "UpperAbsPowerB  ",
                 "UpperDeltaLimitB",
                 "UpperFreqB      "
            };

            var SEMData = new double[] {
                 Math.Round(SEM_Data.LowerAbsPowerA,2),
                 Math.Round(SEM_Data.LowerDeltaLimitA,2),
                 SEM_Data.LowerFreqA,
                 Math.Round(SEM_Data.UpperAbsPowerA,2),
                 Math.Round(SEM_Data.UpperDeltaLimitA,2),
                 SEM_Data.UpperFreqA,
                 Math.Round(SEM_Data.LowerAbsPowerB,2),
                 Math.Round(SEM_Data.LowerDeltaLimitB,2),
                 SEM_Data.LowerFreqB,
                 Math.Round(SEM_Data.UpperAbsPowerB,2),
                 Math.Round(SEM_Data.UpperDeltaLimitB,2),
                 SEM_Data.UpperFreqB,
            };

            var SEMDataUnit = new string[] {
                 "dB","dB","Hz","dB","dB","Hz","dB","dB","Hz","dB","dB","Hz"
            };
            Results.PublishTable("SEM Data", new List<string> {"SEMDataSettings", "SEM Data", "SEM Data Unit"}, SEMDataSettings, SEMData, SEMDataUnit);
        }
    }
 }
      