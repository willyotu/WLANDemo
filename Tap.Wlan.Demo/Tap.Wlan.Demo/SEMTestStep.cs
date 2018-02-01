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
            // Select frequency based on bandwidth
            double frequency = chipset.choose_freq(bandwidth, channel);
            xAPP.centerFrequency = (frequency * 1000000).ToString();
            xAPP.MeasurementMode();
            xAPP.WlanMode(bandwidth, mode.ToString());
            xAPP.SEMConfigure();
            xAPP.RFBLevel = rfbLevel.ToString();
            //xAPP.SEMTriggerSource();
            xAPP.OptimizePowerRange();

            // Returns SEM Pass/Fail Test results
            //  M9391A_XAPPS.SEM_Results SEM_Results = xAPP.MeasureSEM(average, Aver_Num);

            // Returns SEM data results
            SEMDataResults(xAPP);
        }

        private void SEMDataResults(SAInstrument xAPP)
        {
            SAMeasurements.SEM_Data SEM_Data = xAPP.MeasureSEMData();
            var SEMDataSettings = new string[] {
                "LowerAbsPowerA          : {0,12:0.00} dB",
                 "LowerDeltaLimitA        : {0,70:0.00} dB",
                 "LowerFreqA              : {0,14:0.00} MHz",
                 "UpperAbsPowerA          : {0,17:0.00} dB",
                 "UpperDeltaLimitA        : {0,71:0.00} dB",
                 "UpperFreqA              : {0,19:0.00} MHz",
                 "LowerAbsPowerB          : {0,22:0.00} dB",
                 "LowerDeltaLimitB        : {0,72:0.00} dB",
                 "LowerFreqB              : {0,24:0.00} MHz",
                 "UpperAbsPowerB          : {0,27:0.00} dB",
                 "UpperDeltaLimitB        : {0,73:0.00} dB",
                 "UpperFreqB              : {0,29:0.00} MHz "

            };

            var SEMData = new double[] {
                 SEM_Data.LowerAbsPowerA,
                 SEM_Data.LowerDeltaLimitA,
                 SEM_Data.LowerFreqA,
                 SEM_Data.UpperAbsPowerA,
                 SEM_Data.UpperDeltaLimitA,
                 SEM_Data.UpperFreqA,
                 SEM_Data.LowerAbsPowerB,
                 SEM_Data.LowerDeltaLimitB,
                 SEM_Data.LowerFreqB,
                 SEM_Data.UpperAbsPowerB,
                 SEM_Data.UpperDeltaLimitB,
                 SEM_Data.UpperFreqB
            };
            Results.PublishTable("SEM Data", new List<string> { "SEM Data" }, SEMData);
        }
    }
 }
      