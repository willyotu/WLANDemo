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

        [Display("Trigger Source", Group: "Analyzer")]
        public TriggerSource triggerSource { get; set; }
        public enum TriggerSource
        {
            RFB,
            VID
        };
        // ToDo: Add property here for each parameter the end user should be able to change
        #endregion
        public CHPTestStep()
        {
            // ToDo: Set default values for properties / settings.
            chpLowerLimit = 5;
            chpUpperLimit = 15;
            triggerSource = TriggerSource.VID;
        }

        public override void Run()
        {
            CHPTestRun();
        }

        private void CHPTestRun()
        {
            BCM4360 chipset60 = GetParent<TransmitterStep>().bcm4360;
            BCM4366 chipset66 = GetParent<TransmitterStep>().bcm4366;
            SAInstrument xAPP = GetParent<TransmitterStep>().signalAnalyzer;
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            double triggerLevel = GetParent<TransmitterStep>().triggerLevel;
            int bandwidth = GetParent<TransmitterStep>().bandwidth;
            int channel = GetParent<TransmitterStep>().channel;
            string mode = GetParent<TransmitterStep>().mode;

            // Select frequency based on bandwidth
            if (chipset66.IsConnected)
            {
                double frequency = chipset66.choosefrequency(bandwidth, channel);
                xAPP.centerFrequency = (frequency * 1000000).ToString();
            }
            else
            {
                double frequency = chipset60.ChooseFrequency(bandwidth, channel);
                xAPP.centerFrequency = (frequency * 1000000).ToString();
            }
           
            //Set set-top power level to pwrdB value
            double chipsetPowerLevel = GetParent<TransmitterStep>().pwrdB;
            if (chipset66.IsConnected)
            {
                chipset66.SetPowerLevel(chipsetPowerLevel);
            }
            else
            {
                chipset60.SetPowerLevel(chipsetPowerLevel);
            }

            // Initialise CHP settings  
            xAPP.MeasurementMode();
            xAPP.WlanMode(bandwidth, mode.ToString());
            xAPP.ChannelPowerConfigure();
            xAPP.CHPTrigger(triggerSource,triggerLevel);
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
