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
    [Display("Transmitter Setup", Groups: new[] { "WLAN Demo", "Transmitter Measurements"}, Description: "Insert a description here")]
    [AllowAnyChild]
    public class TransmitterStep : TestStep
    {
        #region Settings
        #region Enums for GUI Drop down menus
        // Creates variables and GUI drop down for Rate_Setting
        public enum Rate_Setting
        {
            r,
            h,
            v,
            c,
            s
        };
        #endregion
        // ToDo: Add property here for each parameter the end user should be able to change
        [Unit("Hz", UseEngineeringPrefix: true)]
        [Display("Center Frequency", Group: "Analyzer Setup", Description: " Enter carrier frequency ")]
        public double CenterFrequency { get; set; }

        [Unit("dB", UseEngineeringPrefix: true)]
        [Display("Cable Loss", Group: "Analyzer Setup", Description: " Enter cable loss value ")]
        public double loss { get; set; }

        [Display("Average State", Group: "Analyzer Setup")]
        public bool average { get; set; }

        [Display("Average Number", Group: "Analyzer Setup")]
        public int averageNumber { get; set; }

        [Unit("dB", UseEngineeringPrefix: true)]
        [Display("Trigger Level", Group: "Analyzer Setup", Description: "Sets absolute trigger level")]
        public double absTriggerLevel { get; set; }

        [Display("ISM Band", Group: "Analyzer Setup", Description:"Choose 2g or 5g depnding on wifi channel")]
        public string ismBand { get; set; }

        [Display("Channel", Group: "Chipset Setup")]
        public int channel { get; set; }

        [Display("Rate Setting", Group: "Chipset Setup", Description:"Choose rate setting for wlan mode ")]
        public Rate_Setting rate { get; set; }

        [Display("OFDM Rate(MCS Index)", Group: "Chipset Setup", Description:"Choose rate for a,b,or MCS index for n,ac wlan mode ")]
        public double ofdmRate { get; set; }

        [Unit("MHz", UseEngineeringPrefix: true)]
        [Display("Bandwidth", Group: "Chipset Setup")]
        public int bw { get; set; }

        [Display("Antenna TX Chain", Group: "Chipset Setup")]
        public int antenna { get; set; }

        [Display("Chipset WLAN mode", Group: "Chipset Setup", Description: "Set WLAN mode for DUT")]
         public string mode { get; set; }

        [Display("Start Power", Group: "Chipset Setup")]
        public double pwrdB { get; set; }

        // This property lets the instrument appear in the plugin Instrument field
        public SAInstrument signalAnalyzer { get; set; }

        public BCM4366 bcm4366 { get; set; }
        #endregion
        public TransmitterStep()
        {
            // ToDo: Set default values for properties / settings.

            // Sets averaging state for measurements
            average = false;

            // Sets number of averages for measurements
            averageNumber = 10;

            // Sets cable correction for measurements
            loss = -1.4;

            // Sets ISM Band GUI Text Box to 5g
            ismBand = "5g";

            // Sets GUI Text Box to Channel to 36 Used by WlanChannels to determine frequency if Set Top Box and Analyser
            channel = 36;

            // Sets  the ODFM rate if a, b mode or MCS Index if n, ac mode on the Set Top Box
            ofdmRate = 7;

            // Sets Rate GUI Drop Down to r which determines the modulation scheme set on Set Top Box
            rate = Rate_Setting.h;

            // Sets Bandwidth Text Box to 20 which determines the bandwidth on Set Top Box
            bw = 20;

            // Sets Antenna Tx Chain Text Box to 1 which determines antenna used (1 2 or 3) on Set Top Box
            antenna = 1;

            // Sets BCM4366 Mode GUI Drop Down to r which determines the modulation scheme set on Set Top Box
            mode = "n";

            // Sets Trig Level Text Box to -25 which determines trigger level on analyser
            absTriggerLevel = -26;

            // Sets Start Power Text Box to 30 which determines the Start Power of the Set top Box
            pwrdB = 30;
        }
        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }
        public override void Run()
        {
            // ToDo: Add test case code here
             RunChildSteps(); //If step has child steps.
         
           // UpgradeVerdict(Verdict.Pass);
        }
        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
