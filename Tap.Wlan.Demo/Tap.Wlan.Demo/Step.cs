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

using Keysight.Tap;  // Use Platform infrastructure/core components (log,TestStep definition, etc)

namespace Tap.Wlan.Demo
{
    [Display("Measure CW", Group: "Tutorial", Description: "Carrier Waveform SA using MXA")]
    public class Step : TestStep
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change.
        [Unit("Hz", UseEngineeringPrefix: true)]
        public double CenterFrequency { get; set; }
        public MxaInstrument Mxa { get; set; }
        #endregion

        public Step()
        {
            // ToDo: Set default values for properties / settings.
            CenterFrequency = 2e9;
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        public override void Run()
        {
            Mxa.Configure(CenterFrequency);
            var measurementResults = Mxa.SweepMeasurement();
            var frequency = measurementResults.Where((item, index) => index % 2 == 0).ToArray();
            var amplitude = measurementResults.Where((item, index) => index % 2 != 0).ToArray();
            Results.PublishTable("Keysight Demo", new List<string> { "Frequency", "Amplitude" }, frequency, amplitude);

            Mxa.ConfigureMeasurements(CenterFrequency);
            var chpMeasurementResults = Mxa.ChannelPowerMeasurement();
            var channel = Mxa.CHPCenterFrequency();
            var power = chpMeasurementResults.Where((item, index) => index % 2 == 0).ToArray();
            Results.PublishTable("Wlan Demo", new List<string> { "Frequency", "Amplitude" },channel, power);
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
