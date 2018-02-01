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
            SAInstrument xAPP = GetParent<TransmitterStep>().signalAnalyzer;
           
            double chipsetPowerLevel = GetParent<TransmitterStep>().pwrdB;
            double rfbLevel = GetParent<TransmitterStep>().absTriggerLevel;
            int bandwidth = GetParent<TransmitterStep>().bw;
            int channel = GetParent<TransmitterStep>().channel;
            string mode = GetParent<TransmitterStep>().mode;

            //Set set-top power level to pwrdB value
            // BCM4366.BCM4366_SetPowerLevel(chipsetPowerLevel);

            // Initialise SEM settings    
            //  xAPP.CenterFrequency = (frequency * 1000000).ToString();
            xAPP.MeasurementMode();
            xAPP.WlanMode(bandwidth, mode.ToString());
            xAPP.SEMConfigure();
            xAPP.RFBLevel = rfbLevel.ToString();
            //xAPP.SEMTriggerSource();
            xAPP.OptimizePowerRange();

            // Returns SEM Pass/Fail Test results
            //  M9391A_XAPPS.SEM_Results SEM_Results = xAPP.MeasureSEM(average, Aver_Num);

            // Returns SEM data results
            SAMeasurements.SEM_Data SEM_Data = xAPP.MeasureSEMData();

            // Log SEM results into TAP message window
            //Log_SEM_TAP(log, SEM_Results, SEM_Data);


            // Code to Increase Power if Channel Power measured does not exceed the SEM limit Or decrese power if measured power exceeds SEM limit
            //Inc_Dec_PWR_SEM(log, SEM_Results, BCM4366, frequency);
        }
    }
}
