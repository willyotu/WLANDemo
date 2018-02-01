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
using System.Xml.Serialization;
using System.Diagnostics;

//Note this template assumes that you have a SCPI based instrument, and accordingly
//extends the ScpiInstrument base class.

//If you do NOT have a SCPI based instrument, you should modify this instance to extend
//the (less powerful) Instrument base class.

namespace Tap.Wlan.Demo
{
    [Display("SAInstrument", Group: "Demo", Description: "Insert a description here")]
    [ShortName("xSA")]
    public class SAInstrument : ScpiInstrument
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
       
        #endregion
        public SAInstrument()
        {
            // ToDo: Set default values for properties / settings.
       
        }

        /// <summary>
        /// Open procedure for the instrument.
        /// </summary>
        public override void Open()
        {
            AnalyzerModels model = new AnalyzerModels();
            base.Open();
            // TODO:  Open the connection to the instrument here
     
            string[] modelID = IdnString.Split(',');
            
            string[] analyzerList = model.AnalyzerModel().ToArray();

            if (!modelID.Intersect(analyzerList).Any())
            {
                Log.Error("This instrument driver does not support the connected instrument.");
                throw new ArgumentException("Wrong instrument type.");
            }

        }

        /// <summary>
        /// Close procedure for the instrument.
        /// </summary>
        public override void Close()
        {
            // TODO:  Shut down the connection to the instrument here.
            base.Close();
        }

        #region VSA state settings

        // Sets the wlan mode in VSA for measurements
        public void MeasurementMode()
        {
            ScpiCommand("INST WLAN");
        }

        // Sets the wlan standard setup a,b,g,n,ac in VSA for measurements
        public void WlanMode(int BW, string mode)
        {
            if (mode == "a" & BW == 20)
            {
                ScpiCommand("RAD:STAN AG");
            }
            else if (mode == "b" | mode == "g" & BW == 20)
            {
                ScpiCommand("RAD:STAN BG");
            }

            else if (mode == "n" & BW == 20)
            {
                ScpiCommand("RAD:STAN N20");
            }
            else if (mode == "n" & BW == 40)
            {
                ScpiCommand("RAD:STAN N40");
            }

            else if (mode == "ac" & BW == 20)
            {
                ScpiCommand("RAD:STAN AC20");
            }
            else if (mode == "ac" & BW == 40)
            {
                ScpiCommand("RAD:STAN AC40");
            }
            else if (mode == "ac" & BW == 80)
            {
                ScpiCommand("RAD:STAN AC80");
            }
            else if (mode == "ac" & BW == 160)
            {
                ScpiCommand("RAD:STAN AC160"); ;
            }
            else
            {
                Log.Debug("Unsupported Wlan Mode");
            }

        }

        public void SystemPreset()
        {
            ScpiCommand(":SYST:PRES");
        }

        public void Correction(double loss)
        {
            string cable_l = ":CORR:SA:GAIN " + loss.ToString();
            ScpiCommand(cable_l);
        }

        [XmlIgnore]
        public string centerFrequency
        {
            set { ScpiCommand(":FREQ:CENT {0}", value); }
            get
            {
                return ScpiQuery<string>(":FREQ:CENT?");

            }
        }

        // Sets  RF Burst  Absolute Trigger Level
        [XmlIgnore]
        public string RFBLevel
        {
            set { ScpiCommand(":TRIG:RFB:LEV:ABS {0}", value); }
            get { return ScpiQuery<string>(":TRIG:RFB:LEV:ABS?"); }
        }

        // Optimizes signal level into ADC
        public void OptimizePowerRange()
        {
            ScpiCommand(":POWER:RANGE:OPTimize IMMediate");
        }
        #endregion

        #region CHP settings
        public void Set_CHP_Trig()
        {
            ScpiCommand("TRIG:CHP:SOUR RFB");
        }
        public void Set_ChannelPower_Conf()
        {
            ScpiCommand("CONF:CHPower");
        }
        //Sets averaging for CHP
        public bool Set_CHP_AvgState(bool Average, int Aver_Num)
        {
            ScpiCommand("CHP:AVER {0}", Average);
            string scpi = ("CHP:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);
            return Average;
        }
        public string Fetch_ChannelPower()
        {
            return (ScpiQuery("FETCH:CHPower?"));
        }

        #endregion

        #region Modulation Analysis settings
        public void Set_EVM_Conf()
        {
            ScpiCommand("CONF:EVM");
        }
        //Sets averaging for Modulation Analysis
        public bool Set_EVM_AvgState(bool Average, int Aver_Num)
        {
            ScpiCommand("EVM:AVER {0}", Average);
            string scpi = ("EVM:AVER:COUN " + Aver_Num.ToString());
            return Average;
        }
        public void Set_EVM_Limits(double Freq_Error, double SymClk_Error, double CFLeak)
        {
            string scpi = (":CALCulate:EVM:LIMit:FERRor " + Freq_Error.ToString());
            ScpiCommand(scpi);

            string scpi2 = (":CALCulate:EVM:LIMit:CLKerror " + SymClk_Error.ToString());
            ScpiCommand(scpi2);

            string scpi3 = (":CALCulate:EVM:LIMit:CFLeakage " + CFLeak.ToString());
            ScpiCommand(scpi3);
        }
        public void Set_EVM_Trig()
        {
            ScpiCommand("TRIG:EVM:SOUR RFB");
        }
        public void Set_EVM_Tracking()
        {
            ScpiCommand("CALC:EVM:PIL:TRAC:AMPL 1");
            ScpiCommand("CALC:EVM:PIL:TRAC:PHAS 1");
            ScpiCommand("CALC:EVM:PIL:TRAC:TIM 1");
        }
        public void Num_Syms(int Symbols)
        {
            string syms = "EVM:TIME:RES:LENG " + Symbols.ToString();
            ScpiCommand(syms);
        }
        //Sets the length of time for the burst search 
        public void Set_Meas_Time(string Search_Len)
        {
            string search_len = "EVM:TIME:SLEN " + Search_Len.ToString();
            ScpiCommand(search_len);
        }
        public string Fetch_EVM()
        {
            return (ScpiQuery("FETCH:EVM1?"));
        }
        #endregion

        #region SEM settings
        public void SEMConfigure()
        {
            ScpiCommand("CONF:SEM");
        }
        public void SEMMeasurementTypeTP()
        {
            ScpiCommand("SEM:TYPE TPRef");
            ScpiQuery("*OPC?");
        }

        //Sets averaging for SEM
        public bool SEMAveragingState(bool Average, int Aver_Num)
        {
            ScpiCommand("SEM:AVER {0}", Average);
            string scpi = ("SEM:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);
            return Average;
        }
        public void SEMTriggerSource()
        {
            ScpiCommand("TRIG:SEM:SOUR RFB");
        }
        public string FetchSEM()
        {
            return (ScpiQuery("FETCH:SEM7?"));
        }
        public string FetchSEMData()
        {
            return (ScpiQuery("FETCH:SEM?"));
        }
        #endregion

        #region Channel Flatness settings
        public void Set_ChannelFlatness_Conf()
        {
            ScpiCommand("CONF:Flat");
        }
        //Sets averaging For Spectral Flatness 
        public bool Set_FLAT_AvgState(bool Average, int Aver_Num)
        {
            ScpiCommand("FLAT:AVER {0}", Average);
            string scpi = ("FLAT:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);
            return Average;
        }
        public void Set_FLAT_Limits(double S1_UL, double S1_LL, double S2_UL, double S2_LL)
        {
            string scpi = (":CALCulate:FLATness:LIMit:UPPer:SECTion1 " + S1_UL.ToString());
            ScpiCommand(scpi);

            string scpi2 = (":CALCulate:FLATness:LIMit:LOWer:SECTion1 " + S1_LL.ToString());
            ScpiCommand(scpi2);

            string scpi3 = (":CALCulate:FLATness:LIMit:UPPer:SECTion2 " + S2_UL.ToString());
            ScpiCommand(scpi3);

            string scpi4 = (":CALCulate:FLATness:LIMit:LOWer:SECTion2 " + S2_LL.ToString());
            ScpiCommand(scpi4);
        }
        public void Set_Flat_Trig()
        {
            ScpiCommand("TRIG:FLAT:SOUR RFB");
        }
        public string Fetch_ChannelFlatness()
        {
            return (ScpiQuery("FETCH:FLAT3?"));
        }
        #endregion

        #region PVT Settings
        public void Set_PVT_Conf()
        {
            ScpiCommand("CONF:PVT");
        }
        public bool Set_PVT_AvgState(bool Average, int Aver_Num)
        {
            ScpiCommand("PVT:AVER {0}", Average);
            string scpi = ("PVT:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);
            return Average;
        }
        public void Set_PVT_Limits(double RDTime, double RUTime)
        {
            string scpi = (":CALCulate:PVTime:LIMit:RDTime " + RDTime.ToString());
            ScpiCommand(scpi);
            string scpi2 = (":CALCulate:PVTime:LIMit:RUTime " + RUTime.ToString());
            ScpiCommand(scpi2);
        }
        public void Set_PVT_Trig()
        {
            ScpiCommand("TRIG:PVT:SOUR RFB");
        }
        public void Set_PVT_BurstTime(double PVT_Burst)
        {
            string scpi = ("PVT:BURS:TIME " + PVT_Burst.ToString());
            ScpiCommand(scpi);
        }
        public string Fetch_PVT()
        {
            return (ScpiQuery("FETCH:PVT1?"));
        }
        #endregion

        #region XAPPS Startup
        public void M9OXA_Startup()
        {
            Process[] pname = Process.GetProcessesByName("StartM90XA");
            if (pname.Length == 0)

                Process.Start("C:\\Program Files\\Agilent\\M90XA\\3.0\\StartM90XA.exe", "//ivi:M9391");

            else
                Log.Debug("M90XA XAPPs Started");

        }
        #endregion

        public SAMeasurements.SEM_Data MeasureSEMData()
        {
            AnalyzerModels config = new AnalyzerModels();
           
            SAMeasurements.SEM_Data SEM_Data = new SAMeasurements.SEM_Data();
            // Return SEM data into variable Results
            string Results_SEM_DATA;
            Results_SEM_DATA = FetchSEMData();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_SEM_DATA.Split(',');

            // Extract SEM data element from array into SEM_Data variable
            config.SEMMeasurementData();
            return (SEM_Data);
        }
    }
}
