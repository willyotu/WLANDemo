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
    [Display("Transmitter_Measurements", Groups: new[] { "Demo", "Transmitter Measurements"}, Description: "Insert a description here")]
    [AllowAnyChild]
    public class Transmitter_Measurements : TestStep
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
        public double CenterFrequency { get; set; }

        [Display("VSA\\Cable Loss [dB]", Group: "Demo", Description: " Enter cable loss value ")]
        public double loss { get; set; }

        [Display("VSA\\Average State", Group: "Demo")]
        public bool average { get; set; }

        [Display("VSA\\Average Number", Group: "Demo")]
        public int Aver_Num { get; set; }

        [Display("VSA\\Trigger Level [dBm]", Group: "Demo", Description: "Sets absolute trigger level")]
        public double ABS_Trig_Level { get; set; }

        [Display("SetTop\\ISM Band", Group: "Demo", Description:"Choose 2g or 5g depnding on wifi channel")]
        public string ISM_Band { get; set; }

        [Display("SetTop\\\t\t\tChannel", Group: "Demo")]
        public int Channel { get; set; }

        [Display("SetTop\\\t\tRate Setting", Group: "Demo", Description:"Choose rate setting for wlan mode ")]
        public Rate_Setting rate { get; set; }

        [Display("SetTop\\\tOFDM Rate(MCS Index)", Group: "Demo",Description:"Choose rate for a,b,or MCS index for n,ac wlan mode ")]
        public double OFDM_rate { get; set; }

        [Display("SetTop\\Bandwidth [MHz]", Group: "Demo")]
        public int BW { get; set; }

        [Display("SetTop\\Antenna TX Chain", Group: "Demo")]
        public int antenna { get; set; }

        [Display("SetTop\\Chipset WLAN mode", Group: "Demo", Description: "Set WLAN mode for DUT")]
         public string mode { get; set; }

        [Display("SetTop\\Start Power", Group: "Demo")]
        public double pwrdB { get; set; }

        // This property lets the instrument appear in the plugin Instrument field
        public SAInstrument signalAnalyzer { get; set; }
        #endregion
        public Transmitter_Measurements()
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
            // RunChildSteps(); //If step has child steps.
            signalAnalyzer.Set_Mode();
            signalAnalyzer.Set_SEM_Conf();
            UpgradeVerdict(Verdict.Pass);
        }
        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
