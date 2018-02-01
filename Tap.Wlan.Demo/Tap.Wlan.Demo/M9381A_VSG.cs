// Responsible: TEAM (Will Attoh)
// Copyright:   Copyright 2015 Keysight Technologies.  All rights reserved. No 
//              part of this program may be photocopied, reproduced or translated 
//              to another program language without the prior written consent of 
//              Keysight Technologies.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Tap;
using Agilent.CommandExpert.ScpiNet.AgSCPI99_1_0;
using System.Xml.Serialization;
using System.Diagnostics;

namespace TapPlugin.PaceUK
{
    [DisplayName("M9381A_VSG")]
    [Description("RF Generator for Receiver tests")]
    [ShortName("M9381A")]
    public class M9381A_VSG : ScpiInstrument
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        #endregion

        public M9381A_VSG()
        {
            //shortName="MyInst"; //Insert your shortname.
            // ToDo: Set default values for properties / settings.
        }

        /// <summary>
        /// Open procedure for the instrument.
        /// </summary>
        public override void Open()
        {
            base.Open();
            //if (!IdnString.Contains("Instrument ID"))
            //{
            //    log.Error("This instrument driver does not support the connected instrument.");
            //    throw new ArgumentException("Wrong instrument type.");
            //}
        }

        /// <summary>
        /// Close procedure for the instrument.
        /// </summary>
        public override void Close()
        {
            base.Close();
        }
        public void InitializeGenerator()
        {
            ScpiCommand("*WAI");
            ScpiCommand("*RST");
            ScpiQuery("*IDN?");
            ScpiCommand("*WAI");

        }
        public void Play_Waveform(double frequency, double Amptd, string ArbName,  double rms, int repeat)
        {

            // Marker Trig / Pulse Blanking"	
            ScpiCommand(":RADio:ARB:MDEStination:PULSe M2");
            ScpiCommand(":RADio:ARB:MPOL:MARK2 POS");


            // Set Arb/Seq Trigger
            ScpiCommand("SOURCe:RADio:ARB:TRIGger:SOURce:EXTernal TRIG1");
            ScpiCommand("SOURce:RADio:ARB:TRIGger:TYPE SINGle");
            ScpiCommand("SOURce:RADio:ARB:TRIGger:SOURce:EXTernal:SLOPE POSitive");

            ScpiCommand(":OUTPut:STATe OFF");
            ScpiCommand(":RAD:ARB OFF");
            ScpiCommand(":SOURce:POWer:LEVel:IMMediate:AMPLitude -100");
          
            Set_frequency = (frequency * 1000000).ToString();

            ScpiCommand(":MEMory:DELete:ALL");
            Load_Waveform(ArbName);
            ScpiCommand(":SOURce:POWer:LEVel:IMMediate:AMPLitude " + Amptd.ToString() + "dBm");
            ScpiCommand("*WAI");

            // Set RMS power for Sequence - must be before command to create sequence!!!
            ScpiCommand(":RAD:DMOD:ARB:RMS " + rms.ToString());

            // Play Sequence userdefined number of times. Default is just once.
            string scpi = (":RADio:ARB:SEQuence \"SEQ:MySequence\",\"arb\"," + repeat.ToString() + ",M2");
            ScpiCommand(scpi);
           
            ScpiCommand(":OUTPut:MODulation:STATe ON");

            // Setup Trigger output for M9300A
            ScpiCommand(":MODule:REFerence:OUTPut:TRIGger:STATe ON");
            ScpiCommand(":MODule:REFerence:OUTPut:TRIGger:DESTination TRIG2");
            ScpiCommand(":MODule:REFerence:OUTPut:TRIGger:POLarity POSitive");
            ScpiQuery("*OPC?");

            // Play Sequence
            ScpiCommand(":RAD:ARB:WAV \"SEQ:MySequence\"");

            // Must have ARB turned on before RF Output to enable RF 
            ScpiCommand(":OUTPut:STATe ON");
            ScpiCommand(":RAD:ARB ON");

            // Do Power Search sinc ALC is off
            ScpiCommand(":SOURce:POWer:ALC:SEARch:IMMediate");

            // Generate Trigger on Trig2 output of M9300A
            ScpiCommand(":MODule:REFerence:OUTPut:TRIGger:PULSE");
            ScpiQuery("*OPC?");
            System.Threading.Thread.Sleep(3001);

            // Turn VSG off
            ScpiCommand(":OUTPut:MODulation:STATe OFF");
            ScpiCommand(":OUTPut:STATe OFF");
            ScpiCommand(":RAD:ARB OFF");
            ScpiQuery("SYST:ERR?");


        }
        [XmlIgnore]
        public string Set_frequency
        {
            set { ScpiCommand(":SOURce:Frequency:CENTer {0}", value); }
            get
            {
                return ScpiQuery<string>(":FREQ:CENT?");

            }
        }
        public void Set_Amplitude(double Amptd)
        {
            string ampl = ":SOURce:POWer:LEVel:IMMediate:AMPLitude " + Amptd.ToString() + "dBm";
            ScpiCommand(ampl);
        }
        public void Load_Waveform(string ArbName)
        {
            string FileName = string.Format(":MEM:COPY \"{0}\" ,\"{1}\"", ArbName.Trim(), "arb");
            ScpiCommand(FileName);
        }
        public void M9381Server_Startup()
        {
            Process[] pname = Process.GetProcessesByName("AgM938xScpi");
            if (pname.Length == 0)

                Process.Start("C:\\Program Files (x86)\\Agilent\\M938x\\bin\\AgM938xScpi.exe");

            else

                log.Debug("M938X Server Started");

        }
    }
}
