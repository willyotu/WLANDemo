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
    [Display("Receiver Setup", Groups: new[] { "WLAN Demo", "Receiver Measurements"}, Description: "Insert a description here")]
    [AllowAnyChild]
    public class ReceiverStep : TestStep
    {
        #region Settings
        #region Enums for GUI Drop down menus
        // Creates variables and GUI drop down for Rate_Setting
        public enum RateSettingRX
        {
            r,
            h,
            v,
            c,
            s
        };
        
        #endregion
        // ToDo: Add property here for each parameter the end user should be able to change
        [Unit("dB", UseEngineeringPrefix: true)]
        [Display("Cable Loss", Group: "Chipset", Description: " Enter cable loss value ", Order: 1.1)]
        public double loss { get; set; }

        [Display("Channel", Group: "Chipset", Order: 1.2)]
        public int channel { get; set; }

        [Display("Antenna", Group: "Chipset", Order: 1.3)]
        public int antenna { get; set; }

        [Display("Rate Setting", Description:"Choose rate setting for wlan mode ", Group: "Chipset", Order: 1.4)]
        public RateSettingRX rate { get; set; }

        [Display("Index", Group: "Chipset", Order: 1.5)]
        public double ofdmRate { get; set; }

        [Display("Chipset WLAN mode", Description:"Set WLAN mode for DUT", Group: "Chipset", Order: 1.6)]
        public string mode { get; set; }

        [Display("ISM Band", Description:"Choose 2g or 5g depnding on wifi channel ", Group: "Chipset", Order: 1.7)]
        public string ismBand { get; set; }

        [Display("Start Power", Group: "Chipset", Order: 1.8)]
        public Double pwrdB { get; set; }

        [Unit("MHz", UseEngineeringPrefix: true)]
        [Display("Bandwidth", Group: "Chipset", Order: 1.9)]
        public Double bandwidth { get; set; }

        [Unit("dBm", UseEngineeringPrefix: true)]
        [Display("Amplitude", Group: "Chipset", Order: 1.10)]
        public Double amplitude { get; set; }

        [Display("RMS Power", Group: "Chipset", Order: 1.10)]
        public Double rms { get; set; }

        [Display("Repetition", Group: "Generator", Order: 2.1)]
        public int repeat { get; set; }

        [Display("Number of Frames", Group: "Generator", Order: 2.2)]
        public int frames { get; set; }

        [FilePath]
        [Display("Waveform", Group: "Generator", Order: 2.3)]
        public string arbName { get; set; }

        [Unit("%", UseEngineeringPrefix: true)]
        [Display("Max PER", Group: "Generator", Order: 2.4,Description:"If measured PER is greater than the specified number the teststep will fail.")]
        public int perLimit { get; set; }

        // This property lets the instrument appear in the plugin Instrument field
        public SGInstrument signalGenerator { get; set; }

        public BCM4366 bcm4366 { get; set; }
        #endregion
        public ReceiverStep()
        {
            // ToDo: Set default values for properties / settings.
            rate = RateSettingRX.h;
            ofdmRate = 0;
            mode = "n";
            ismBand = "5g";
            pwrdB = 60;
            channel = 36;
            amplitude = -20;
            arbName = "C:\\802.11n_Pace_100frames_MCS0.wfm";
            perLimit = 10;
            rms = 12.0;
            repeat = 1;
            frames = 100;
            bandwidth = 20;
            antenna = 1;
        }
        public override void PrePlanRun()
        {
            base.PrePlanRun();
           
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }
        public override void Run()
        {
            // ToDo: Add test case code here
            bcm4366.init4366rx(mode, bandwidth, antenna, ismBand, rate, ofdmRate, pwrdB, channel);
            signalGenerator.InitializeGenerator();
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
