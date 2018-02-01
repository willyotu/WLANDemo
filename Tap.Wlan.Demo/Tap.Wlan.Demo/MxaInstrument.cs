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

//Note this template assumes that you have a SCPI based instrument, and accordingly
//extends the ScpiInstrument base class.

//If you do NOT have a SCPI based instrument, you should modify this instance to extend
//the (less powerful) Instrument base class.

namespace Tap.Wlan.Demo
{
    [Display("MxaInstrument", Group: "Demo", Description: "Insert a description here")]
    [ShortName("MyINST")]
    public class MxaInstrument : ScpiInstrument
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        #endregion
        public MxaInstrument()
        {
            // ToDo: Set default values for properties / settings.
        }

        /// <summary>
        /// Open procedure for the instrument.
        /// </summary>
        public override void Open()
        {

            base.Open();
            // TODO:  Open the connection to the instrument here

            if (!IdnString.Contains("N90"))
            {
                Log.Error("This instrument driver does not support the connected instrument.");
                throw new ArgumentException("Wrong instrument type.");
            }
            ScpiCommand("INST:SEL SA");
        }

        /// <summary>
        /// Close procedure for the instrument.
        /// </summary>
        public override void Close()
        {
            // TODO:  Shut down the connection to the instrument here.
            base.Close();
        }

        public double[] SweepMeasurement()
        {
            ScpiCommand("INIT:SAN");
            ScpiCommand("FORM:TRAC:DATA ASCii,0");
            return ScpiQuery<double[]>("READ:SAN1?");
        }
        public void Configure(double centerFrequency)
        {
            ScpiCommand("SENSE:FREQ:CENT " + centerFrequency);
            ScpiCommand("SENSE:FREQ:SPAN " + 1e6);
            ScpiCommand("SENSE:SWE:POIN " + 1001);
        }

        public void ConfigureMeasurements(double centerFrequency)
        {
            ScpiCommand("INST:SEL WLAN");
            ScpiCommand("SENSE:FREQ:CENT " + centerFrequency);
            ScpiCommand("CONF:CHP"); 
        }

        public double[] ChannelPowerMeasurement()
        {
            return ScpiQuery<double[]>("READ:CHP?");
        }

        public double[] CHPCenterFrequency()
        {
            return ScpiQuery<double[]>("SENSE:FREQ:CENT?");
        }

    }
}
