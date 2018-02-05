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
    [Display("Channel Power", Groups: new[] { "WLAN Demo", "Transmitter Measurements" }, Description: "Insert a description here")]
    [AllowAsChildIn(typeof(TransmitterStep))]
    public class CHPTestStep : TestStep
    {
        #region Settings
        [Unit("dBm", UseEngineeringPrefix: true)]
        [Display("Channel Power Upper Limit",Group:"Limits")]
        public double chpUpperLimit { get; set; }

        [Unit("dBm", UseEngineeringPrefix: true)]
        [Display("Channel Power Lower Limit", Group:"Limits")]
        public double chpLowerLimit { get; set; }
        // ToDo: Add property here for each parameter the end user should be able to change
        #endregion
        public CHPTestStep()
        {
            // ToDo: Set default values for properties / settings.
            chpLowerLimit = 5;
            chpUpperLimit = 15;
        }

        public override void Run()
        {
            CHPTestRun();
        }

        private void CHPTestRun()
        {
            BCM4366 chipset = GetParent<TransmitterStep>().bcm4366;
            SAInstrument xAPP = GetParent<TransmitterStep>().signalAnalyzer;
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            double rfbLevel = GetParent<TransmitterStep>().absTriggerLevel;
            int bandwidth = GetParent<TransmitterStep>().bw;
            int channel = GetParent<TransmitterStep>().channel;
            string mode = GetParent<TransmitterStep>().mode;

            //Set set-top power level to pwrdB value
            double chipsetPowerLevel = GetParent<TransmitterStep>().pwrdB;
            chipset.bcm4366SetPowerLevel(chipsetPowerLevel);
          
            // Initialise CHP settings  
            xAPP.WlanMode(bandwidth, mode.ToString());
            xAPP.ChannelPowerConfigure();
            xAPP.RFBLevel = rfbLevel.ToString();
           // xAPP.CHPTrigger();
            xAPP.OptimizePowerRange();

            CHPResults(xAPP);
        }

        private void CHPResults(SAInstrument xAPP)
        {
            // Return CHP in dBm
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            SAMeasurements.CHPResult chp = xAPP.MeasureChannelPower(average, numberOfAverages);
            var chpResult = new double[] {chp.PowerResult };
            if (chpLowerLimit < chp.PowerResult & chp.PowerResult < chpUpperLimit)
            {
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                UpgradeVerdict(Verdict.Fail);
            }
            var chpUnit = new string[] {
                 "dBm"
            };
            Results.PublishTable("CHP Result", new List<string> {"CHP","CHP Unit"}, chpResult, chpUnit);
        }

 
    }
}
