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
        public void CHPTrigger()
        {
            ScpiCommand("TRIG:CHP:SOUR RFB");
        }
        public void ChannelPowerConfigure()
        {
            ScpiCommand("CONF:CHPower");
        }
        //Sets averaging for CHP
        public bool CHPAveragingState(bool Average, int Aver_Num)
        {
            ScpiCommand("CHP:AVER {0}", Average);
            string scpi = ("CHP:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);
            return Average;
        }
        public string FetchChannelPower()
        {
            return (ScpiQuery("FETCH:CHPower?"));
        }

        #endregion

        #region Modulation Analysis settings
        public void EVMConfigure()
        {
            ScpiCommand("CONF:EVM");
        }
        //Sets averaging for Modulation Analysis
        public bool EVMAveragingState(bool average, int numberOfAverages)
        {
            ScpiCommand("EVM:AVER {0}", average);
            string scpi = ("EVM:AVER:COUN " + numberOfAverages.ToString());
            return average;
        }
        public void EVMLimits(double freqError, double symbolClkError, double cFLeak)
        {
            string scpi = (":CALCulate:EVM:LIMit:FERRor " + freqError.ToString());
            ScpiCommand(scpi);

            string scpi2 = (":CALCulate:EVM:LIMit:CLKerror " + symbolClkError.ToString());
            ScpiCommand(scpi2);

            string scpi3 = (":CALCulate:EVM:LIMit:CFLeakage " + cFLeak.ToString());
            ScpiCommand(scpi3);
        }
        public void EVMTrigger()
        {
            ScpiCommand("TRIG:EVM:SOUR RFB");
        }
        public void EVMTracking()
        {
            ScpiCommand("CALC:EVM:PIL:TRAC:AMPL 1");
            ScpiCommand("CALC:EVM:PIL:TRAC:PHAS 1");
            ScpiCommand("CALC:EVM:PIL:TRAC:TIM 1");
        }
        public void NumberOfSymbols(int symbols)
        {
            string syms = "EVM:TIME:RES:LENG " + symbols.ToString();
            ScpiCommand(syms);
        }
        //Sets the length of time for the burst search 
        public void BurstSearchLength(double searchLength)
        {
            string search_len = "EVM:TIME:SLEN " + searchLength.ToString();
            ScpiCommand(search_len);
        }
        public string FetchEVM()
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
        public bool SEMAveragingState(bool average, int numberOfAverages)
        {
            if (average)
            {
                ScpiCommand("SEM:AVER ON");
                ScpiCommand("SEM:AVER:COUN " + numberOfAverages.ToString());
            }
            else
            {
                ScpiCommand("SEM:AVER OFF");
            }
            return average;
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

        #region CHP Measurement
        public SAMeasurements.CHPResult MeasureChannelPower(bool average, int numberOfAverages)
        {
            SAMeasurements.CHPResult powerResult = new SAMeasurements.CHPResult();

           CHPAveragingState(average, numberOfAverages);

            // Return channel power results data into variable resultsCHP
            string resultsCHP;
            resultsCHP = FetchChannelPower();

            // Split Results_CHP data into individual elements in an array
            string[] iqCols = resultsCHP.Split(',');

            // Extract CHP only element from array into power_result variable
            Double.TryParse(iqCols[0], out  powerResult.PowerResult);
            return (powerResult);

        }
        #endregion

        #region SEM Measurements
        public SAMeasurements.SEMLimitTest MeasureSEMLimit(bool average, int NumberOfAverages)
        {
            SAMeasurements.SEMLimitTest SEMLimitResults = new SAMeasurements.SEMLimitTest();

            SEMAveragingState(average, NumberOfAverages);

            // Return SEM data into variable Results
            string ResultsSEMPassFail;
            ResultsSEMPassFail = FetchSEM();

            // Split Results data into individual elements in an array
            string[] iqCols = ResultsSEMPassFail.Split(',');

            // Extract SEM resuts element from array into SEM_Results variable
            Double.TryParse(iqCols[4], out SEMLimitResults.NegOFFSFREQA);
            Double.TryParse(iqCols[5], out SEMLimitResults.PosOFFSFREQA);
            Double.TryParse(iqCols[6], out SEMLimitResults.NegOFFSFREQB);
            Double.TryParse(iqCols[7], out SEMLimitResults.PosOFFSFREQB);
            return (SEMLimitResults);

        }
        public SAMeasurements.SEMData MeasureSEMData()
        {
            AnalyzerModels config = new AnalyzerModels();

            SAMeasurements.SEMData SEM_Data = new SAMeasurements.SEMData();
            // Return SEM data into variable Results
            string Results_SEM_DATA;
            Results_SEM_DATA = FetchSEMData();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_SEM_DATA.Split(',');

            // Extract SEM data element from array into SEM_Data variable
            //config.SEMMeasurementData();
            // Extract SEM data element from array into SEM_Data variable
            Double.TryParse(iqCols[1], out SEM_Data.TotalPowerRef);
            Double.TryParse(iqCols[12], out SEM_Data.LowerAbsPowerA);
            Double.TryParse(iqCols[70], out SEM_Data.LowerDeltaLimitA);
            Double.TryParse(iqCols[14], out SEM_Data.LowerFreqA);
            Double.TryParse(iqCols[17], out SEM_Data.UpperAbsPowerA);
            Double.TryParse(iqCols[71], out SEM_Data.UpperDeltaLimitA);
            Double.TryParse(iqCols[19], out SEM_Data.UpperFreqA);
            Double.TryParse(iqCols[22], out SEM_Data.LowerAbsPowerB);
            Double.TryParse(iqCols[72], out SEM_Data.LowerDeltaLimitB);
            Double.TryParse(iqCols[24], out SEM_Data.LowerFreqB);
            Double.TryParse(iqCols[27], out SEM_Data.UpperAbsPowerB);
            Double.TryParse(iqCols[73], out SEM_Data.UpperDeltaLimitB);
            Double.TryParse(iqCols[29], out SEM_Data.UpperFreqB);
            return (SEM_Data);
        }
        #endregion

        #region EVM Measurements
        public SAMeasurements.EVMResults MeasureEvm(bool average, int numberOfAverages)
        {
            SAMeasurements.EVMResults EVMResults = new SAMeasurements.EVMResults();

            EVMAveragingState(average, numberOfAverages);

            // Return EVM results data into variable Results
            string resultsEVM;
            resultsEVM = FetchEVM();

            // Split Results data into individual elements in an array
            string[] iqCols = resultsEVM.Split(',');

            // Extract EVM result elements from array into EVMResult variable
            Double.TryParse(iqCols[0], out EVMResults.RMS_EVM_MAX_DB);
            Double.TryParse(iqCols[1], out EVMResults.RMS_EVM_AVG_DB);
            Double.TryParse(iqCols[2], out EVMResults.PEAK_EVM_MAX_DB);
            Double.TryParse(iqCols[3], out EVMResults.PEAK_EVM_AVG_DB);
            Double.TryParse(iqCols[4], out EVMResults.MAX_PEAK_EVM_INDEX);
            Double.TryParse(iqCols[5], out EVMResults.PEAK_EVM_INDEX);
            Double.TryParse(iqCols[6], out EVMResults.FREQ_ERR_MAX_HZ);
            Double.TryParse(iqCols[7], out EVMResults.FREQ_ERR_AVG_HZ);
            Double.TryParse(iqCols[8], out EVMResults.FREQ_ERR_MAX_PPM);
            Double.TryParse(iqCols[9], out EVMResults.FREQ_ERR_AVG_PPM);
            Double.TryParse(iqCols[10], out EVMResults.Symbol_Clock_ERR_MAX);
            Double.TryParse(iqCols[11], out EVMResults.Symbol_Clock_ERR_AVG);
            Double.TryParse(iqCols[12], out EVMResults.IQ_Origin_Offset_MAX);
            Double.TryParse(iqCols[13], out EVMResults.IQ_Origin_Offset_AVG);
            Double.TryParse(iqCols[14], out EVMResults.Gain_Imbalance_MAX);
            Double.TryParse(iqCols[15], out EVMResults.Gain_Imbalance_AVG);
            Double.TryParse(iqCols[16], out EVMResults.Quad_Error_MAX_Degrees);
            Double.TryParse(iqCols[17], out EVMResults.Quad_Error_AVG_Degrees);
            Double.TryParse(iqCols[18], out EVMResults.AVG_Burst_Power_MAX);
            Double.TryParse(iqCols[19], out EVMResults.AVG_Burst_Power_AVG);
            Double.TryParse(iqCols[20], out EVMResults.Peak_Burst_Power_MAX);
            Double.TryParse(iqCols[21], out EVMResults.Peak_Burst_Power_AVG);
            Double.TryParse(iqCols[22], out EVMResults.Peak_to_AVG_BurstPowerRatio_MAX);
            Double.TryParse(iqCols[23], out EVMResults.Peak_to_AVG_BurstPowerRatio_AVG);
            Double.TryParse(iqCols[24], out EVMResults.Data_MOD_Format);
            Double.TryParse(iqCols[25], out EVMResults.Data_Bit_Rate);
            Double.TryParse(iqCols[26], out EVMResults.Pilot_EVM_MAX_DB);
            Double.TryParse(iqCols[27], out EVMResults.Pilot_EVM_AVG_DB);
            Double.TryParse(iqCols[28], out EVMResults.DATA_EVM_MAX_DB);
            Double.TryParse(iqCols[29], out EVMResults.DATA_EVM_AVG_DB);
            Double.TryParse(iqCols[30], out EVMResults.IQ_Timing_Skew_MAX);
            Double.TryParse(iqCols[31], out EVMResults.IQ_Timing_Skew_AVG);
            Double.TryParse(iqCols[32], out EVMResults.RMS_EVM_MAX);
            Double.TryParse(iqCols[33], out EVMResults.RMS_EVM_AVG);
            Double.TryParse(iqCols[34], out EVMResults.PEAK_EVM_MAX);
            Double.TryParse(iqCols[35], out EVMResults.PEAK_EVM_AVG);
            Double.TryParse(iqCols[36], out EVMResults.Pilot_EVM_MAX);
            Double.TryParse(iqCols[37], out EVMResults.Pilot_EVM_AVG);
            Double.TryParse(iqCols[38], out EVMResults.DATA_EVM_MAX);
            Double.TryParse(iqCols[39], out EVMResults.DATA_EVM_AVG);
            return (EVMResults);

        }

        #endregion
    }
}
