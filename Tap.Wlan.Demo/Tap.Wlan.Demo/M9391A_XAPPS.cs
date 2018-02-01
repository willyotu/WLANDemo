// Responsible: TEAM (alexfrew and Will Attoh)
// Copyright:   Copyright 2016 Keysight Technologies.  All rights reserved. No 
//              part of this program may be photocopied, reproduced or translated 
//              to another program language without the prior written consent of 
//              Keysight Technologies.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Tap;
using System.Xml.Serialization;
using System.Diagnostics;

namespace TapPlugin.PaceUK
{
    [DisplayName("M9391A_XAPPS")]
    [Description("Signal Analyser for Transmitter tests")]
    [ShortName("M9391A-XAPP")]
    public class M9391A_XAPPS : ScpiInstrument
    {
        #region Settings
    
        // measurements
        public class CHP_Results
        {
            public double power_result;
            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add("Channel Power [dBm]");
                type.Name = "CHP_Results";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(power_result);
                return res;
            }
        }
        public class EVM_Results
        {
            public double RMS_EVM_MAX_DB; //1
            public double RMS_EVM_AVG_DB; //2
            public double PEAK_EVM_MAX_DB; //3
            public double PEAK_EVM_AVG_DB; //4
            public double MAX_PEAK_EVM_INDEX; //5
            public double PEAK_EVM_INDEX; //6
            public double FREQ_ERR_MAX_HZ; //7
            public double FREQ_ERR_AVG_HZ; //8
            public double FREQ_ERR_MAX_PPM;//9
            public double FREQ_ERR_AVG_PPM; //10
            public double Symbol_Clock_ERR_MAX;//11
            public double Symbol_Clock_ERR_AVG;//12
            public double IQ_Origin_Offset_MAX;//13
            public double IQ_Origin_Offset_AVG;//14
            public double Gain_Imbalance_MAX;//15
            public double Gain_Imbalance_AVG;//16
            public double Quad_Error_MAX_Degrees;//17
            public double Quad_Error_AVG_Degrees;//18
            public double AVG_Burst_Power_MAX;//19
            public double AVG_Burst_Power_AVG; //20
            public double Peak_Burst_Power_MAX;//21
            public double Peak_Burst_Power_AVG;//22
            public double Peak_to_AVG_BurstPowerRatio_MAX;//23
            public double Peak_to_AVG_BurstPowerRatio_AVG;//24
            public double Data_MOD_Format;//25
            public double Data_Bit_Rate;//26
            public double Pilot_EVM_MAX_DB;//27
            public double Pilot_EVM_AVG_DB;//28
            public double DATA_EVM_MAX_DB;//29
            public double DATA_EVM_AVG_DB; //30
            public double IQ_Timing_Skew_MAX;//31
            public double IQ_Timing_Skew_AVG;//32
            public double RMS_EVM_MAX;//33
            public double RMS_EVM_AVG;//34
            public double PEAK_EVM_MAX;//35
            public double PEAK_EVM_AVG;//36
            public double Pilot_EVM_MAX;//37
            public double Pilot_EVM_AVG;//38
            public double DATA_EVM_MAX;//39
            public double DATA_EVM_AVG; //40
            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add("RMS_EVM_MAX_DB");
                type.DimensionTitles.Add("RMS_EVM_AVG_DB");
                type.DimensionTitles.Add("PEAK_EVM_MAX_DB");
                type.DimensionTitles.Add("PEAK_EVM_AVG_DB");
                type.DimensionTitles.Add("MAX_PEAK_EVM_INDEX");
                type.DimensionTitles.Add("PEAK_EVM_INDEX");
                type.DimensionTitles.Add("FREQ_ERR_MAX_HZ");
                type.DimensionTitles.Add("FREQ_ERR_AVG_HZ");
                type.DimensionTitles.Add("FREQ_ERR_MAX_PPM");
                type.DimensionTitles.Add("FREQ_ERR_AVG_PPM");
                type.DimensionTitles.Add("Symbol_Clock_ERR_MAX");
                type.DimensionTitles.Add("Symbol_Clock_ERR_AVG");
                type.DimensionTitles.Add(" IQ_Origin_Offset_MAX");//13
                type.DimensionTitles.Add(" IQ_Origin_Offset_AVG");//14
                type.DimensionTitles.Add(" Gain_Imbalance_MAX");//15
                type.DimensionTitles.Add(" Gain_Imbalance_AVG");//16
                type.DimensionTitles.Add(" Quad_Error_MAX_Degrees");//17
                type.DimensionTitles.Add(" Quad_Error_AVG_Degrees");//18
                type.DimensionTitles.Add(" AVG_Burst_Power_MAX");//19
                type.DimensionTitles.Add(" AVG_Burst_Power_AVG"); //20
                type.DimensionTitles.Add(" Peak_Burst_Power_MAX");//21
                type.DimensionTitles.Add(" Peak_Burst_Power_AVG");//22
                type.DimensionTitles.Add(" Peak_to_AVG_BurstPowerRatio_MAX");//23
                type.DimensionTitles.Add(" Peak_to_AVG_BurstPowerRatio_AVG");//24
                type.DimensionTitles.Add(" Data_MOD_Format");//25
                type.DimensionTitles.Add(" Data_Bit_Rate");//26
                type.DimensionTitles.Add(" Pilot_EVM_MAX_DB");//27
                type.DimensionTitles.Add(" Pilot_EVM_AVG_DB");//28
                type.DimensionTitles.Add(" DATA_EVM_MAX_DB");//29
                type.DimensionTitles.Add(" DATA_EVM_AVG_DB"); //30
                type.DimensionTitles.Add(" IQ_Timing_Skew_MAX");//31
                type.DimensionTitles.Add(" IQ_Timing_Skew_AVG");//32
                type.DimensionTitles.Add(" RMS_EVM_MAX");//33
                type.DimensionTitles.Add(" RMS_EVM_AVG");//34
                type.DimensionTitles.Add(" PEAK_EVM_MAX");//35
                type.DimensionTitles.Add(" PEAK_EVM_AVG");//36
                type.DimensionTitles.Add(" Pilot_EVM_MAX");//37
                type.DimensionTitles.Add(" Pilot_EVM_AVG");//38
                type.DimensionTitles.Add(" DATA_EVM_MAX");//39
                type.DimensionTitles.Add(" DATA_EVM_AVG"); //40
                type.Name = "EVM_Results";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(RMS_EVM_MAX_DB); //1
                res.Doubles.Add(RMS_EVM_AVG_DB); //2
                res.Doubles.Add(PEAK_EVM_MAX_DB); //3
                res.Doubles.Add(PEAK_EVM_AVG_DB); //4
                res.Doubles.Add(MAX_PEAK_EVM_INDEX); //5
                res.Doubles.Add(PEAK_EVM_INDEX); //6
                res.Doubles.Add(FREQ_ERR_MAX_HZ); //7
                res.Doubles.Add(FREQ_ERR_AVG_HZ); //8
                res.Doubles.Add(FREQ_ERR_MAX_PPM);//9
                res.Doubles.Add(FREQ_ERR_AVG_PPM); //10
                res.Doubles.Add(Symbol_Clock_ERR_MAX);//11
                res.Doubles.Add(Symbol_Clock_ERR_AVG);//12
                res.Doubles.Add(IQ_Origin_Offset_MAX);//13
                res.Doubles.Add(IQ_Origin_Offset_AVG);//14
                res.Doubles.Add(Gain_Imbalance_MAX);//15
                res.Doubles.Add(Gain_Imbalance_AVG);//16
                res.Doubles.Add(Quad_Error_MAX_Degrees);//17
                res.Doubles.Add(Quad_Error_AVG_Degrees);//18
                res.Doubles.Add(AVG_Burst_Power_MAX);//19
                res.Doubles.Add(AVG_Burst_Power_AVG); //20
                res.Doubles.Add(Peak_Burst_Power_MAX);//21
                res.Doubles.Add(Peak_Burst_Power_AVG);//22
                res.Doubles.Add(Peak_to_AVG_BurstPowerRatio_MAX);//23
                res.Doubles.Add(Peak_to_AVG_BurstPowerRatio_AVG);//24
                res.Doubles.Add(Data_MOD_Format);//25
                res.Doubles.Add(Data_Bit_Rate);//26
                res.Doubles.Add(Pilot_EVM_MAX_DB);//27
                res.Doubles.Add(Pilot_EVM_AVG_DB);//28
                res.Doubles.Add(DATA_EVM_MAX_DB);//29
                res.Doubles.Add(DATA_EVM_AVG_DB); //30
                res.Doubles.Add(IQ_Timing_Skew_MAX);//31
                res.Doubles.Add(IQ_Timing_Skew_AVG);//32
                res.Doubles.Add(RMS_EVM_MAX);//33
                res.Doubles.Add(RMS_EVM_AVG);//34
                res.Doubles.Add(PEAK_EVM_MAX);//35
                res.Doubles.Add(PEAK_EVM_AVG);//36
                res.Doubles.Add(Pilot_EVM_MAX);//37
                res.Doubles.Add(Pilot_EVM_AVG);//38
                res.Doubles.Add(DATA_EVM_MAX);//39
                res.Doubles.Add(DATA_EVM_AVG); //40
                return res;
            }

        }
        public class SEM_Results
        {
            public double NegOFFSFREQA;//4
            public double PosOFFSFREQA;//5
            public double NegOFFSFREQB;//6
            public double PosOFFSFREQB;//7
            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add(" NegOFFSFREQA");//4
                type.DimensionTitles.Add(" PosOFFSFREQA");//5
                type.DimensionTitles.Add(" NegOFFSFREQB");//6
                type.DimensionTitles.Add(" PosOFFSFREQB");//7
                type.Name = "SEM_Results";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(NegOFFSFREQA);//4
                res.Doubles.Add(PosOFFSFREQA);//5
                res.Doubles.Add(NegOFFSFREQB);//6
                res.Doubles.Add(PosOFFSFREQB);//7
                return res;
            }
        }
        public class SEM_Data
        {
            public double TotalPowerRef; //1
            public double LowerAbsPowerA;//12
            public double LowerDeltaLimitA;//70
            public double LowerFreqA;//14
            public double UpperAbsPowerA;//17
            public double UpperDeltaLimitA;//71
            public double UpperFreqA;//19
            public double LowerAbsPowerB;//22
            public double LowerDeltaLimitB;//72
            public double LowerFreqB;//24
            public double UpperAbsPowerB;//27
            public double UpperDeltaLimitB;//73
            public double UpperFreqB;//29
            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add(" Total Power Ref");//1 This only applies if measurement type selected under Meas Type is Total Power Ref
                type.DimensionTitles.Add(" LowerAbsPowerA");//12
                type.DimensionTitles.Add(" LowerDeltaLimitA");//70
                type.DimensionTitles.Add(" LowerFreqA");//14
                type.DimensionTitles.Add(" UpperAbsPowerA");//17
                type.DimensionTitles.Add(" UpperDeltaLimitA");//71
                type.DimensionTitles.Add(" UpperFreqA");//19
                type.DimensionTitles.Add(" LowerAbsPowerB");//22
                type.DimensionTitles.Add(" LowerDeltaLimitB");//72
                type.DimensionTitles.Add(" LowerFreqB");//24
                type.DimensionTitles.Add(" UpperAbsPowerB");//27
                type.DimensionTitles.Add(" UpperDeltaLimitB");//73
                type.DimensionTitles.Add(" UpperFreqB");//29
                type.Name = "SEM_Data";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(TotalPowerRef);//1 This only applies if measurement type selected under Meas Type is Total Power Ref
                res.Doubles.Add(LowerAbsPowerA);//12
                res.Doubles.Add(LowerDeltaLimitA);//70
                res.Doubles.Add(LowerFreqA);//14
                res.Doubles.Add(UpperAbsPowerA);//17
                res.Doubles.Add(UpperDeltaLimitA);//71
                res.Doubles.Add(UpperFreqA);//19
                res.Doubles.Add(LowerAbsPowerB);//22
                res.Doubles.Add(LowerDeltaLimitB);//72
                res.Doubles.Add(LowerFreqB);//24
                res.Doubles.Add(UpperAbsPowerB);//27
                res.Doubles.Add(UpperDeltaLimitB);//73
                res.Doubles.Add(UpperFreqB);//29
                return res;
            }
        }
        public class Flatness_Results
        {
            public double S1MAX_Point_UL_dB;
            public double S1MAX_Point_UL_Delta;
            public double S1MAX_Point_UL_Subcarrier;
            public double S1MIN_Point_LL_dB;
            public double S1MIN_Point_LL_Delta;
            public double S1MIN_Point_LL_Subcarrier;
            public double S2MAX_Point_UL_dB;
            public double S2MAX_Point_UL_Delta;
            public double S2MAX_Point_UL_Subcarrier;
            public double S2MIN_Point_LL_dB;
            public double S2MIN_Point_LL_Delta;
            public double S2MIN_Point_LL_Subcarrier;

            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add("S1MAX_Point_UL_dB");
                type.DimensionTitles.Add("S1MAX_Point_UL_Delta");
                type.DimensionTitles.Add("S1MAX_Point_UL_Subcarrier");
                type.DimensionTitles.Add("S1MIN_Point_LL_dB");
                type.DimensionTitles.Add("S1MIN_Point_LL_Delta");
                type.DimensionTitles.Add("S1MIN_Point_LL_Subcarrier");
                type.DimensionTitles.Add("S2MAX_Point_UL_dB");
                type.DimensionTitles.Add("S2MAX_Point_UL_Delta");
                type.DimensionTitles.Add("S2MAX_Point_UL_Subcarrier");
                type.DimensionTitles.Add("S2MIN_Point_LL_dB");
                type.DimensionTitles.Add("S2MIN_Point_LL_Delta");
                type.DimensionTitles.Add("S2MIN_Point_LL_Subcarrier");
                type.Name = "Flatness_Results";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(S1MAX_Point_UL_dB);
                res.Doubles.Add(S1MAX_Point_UL_Delta);
                res.Doubles.Add(S1MAX_Point_UL_Subcarrier);
                res.Doubles.Add(S1MIN_Point_LL_dB);
                res.Doubles.Add(S1MIN_Point_LL_Delta);
                res.Doubles.Add(S1MIN_Point_LL_Subcarrier);
                res.Doubles.Add(S2MAX_Point_UL_dB);
                res.Doubles.Add(S2MAX_Point_UL_Delta);
                res.Doubles.Add(S2MAX_Point_UL_Subcarrier);
                res.Doubles.Add(S2MIN_Point_LL_dB);
                res.Doubles.Add(S2MIN_Point_LL_Delta);
                res.Doubles.Add(S2MIN_Point_LL_Subcarrier);
                return res;
            }
        }
        public class PVT_Results
        {
            public double Power_on_Ramp_Time;
            public double Power_down_Ramp_Time;
            public double Overall_Pass_Fail;
            public double Power_on_Ramp_Pass_Fail;
            public double Power_down_Ramp_Pass_Fail;
            public double Start_Level_Ramp_up;
            public double Start_Level_Ramp_down;
            public double Stop_Level_Ramp_up;
            public double Stop_Level_Ramp_down;

            public TestStepResult ToTestStepResult()
            {
                TestStepResultType type = new TestStepResultType();
                type.DimensionTitles.Add("Power_on_Ramp_Time");
                type.DimensionTitles.Add("Power_down_Ramp_Time");
                type.DimensionTitles.Add("Overall_Pass_Fail");
                type.DimensionTitles.Add("Power_on_Ramp_Pass_Fail");
                type.DimensionTitles.Add("Power_down_Ramp_Pass_Fail");
                type.DimensionTitles.Add("Start_Level_Ramp_up");
                type.DimensionTitles.Add("Start_Level_Ramp_down");
                type.DimensionTitles.Add("Stop_Level_Ramp_up");
                type.DimensionTitles.Add("Stop_Level_Ramp_down");
                type.Name = "PVT_Results";
                TestStepResult res = new TestStepResult();
                res.Type = type;
                res.Doubles.Add(Power_on_Ramp_Time);
                res.Doubles.Add(Power_down_Ramp_Time);
                res.Doubles.Add(Overall_Pass_Fail);
                res.Doubles.Add(Power_on_Ramp_Pass_Fail);
                res.Doubles.Add(Power_down_Ramp_Pass_Fail);
                res.Doubles.Add(Start_Level_Ramp_up);
                res.Doubles.Add(Start_Level_Ramp_down);
                res.Doubles.Add(Stop_Level_Ramp_up);
                res.Doubles.Add(Stop_Level_Ramp_down);
                return res;
            }
        }
        #endregion
        public M9391A_XAPPS()
        {
           
         
        }

        // Open procedure for the instrument.
        public override void Open()
        {
            base.Open();
            //if (!IdnString.Contains("Instrument ID"))
            //{
            //    log.Error("This instrument driver does not support the connected instrument.");
            //    throw new ArgumentException("Wrong instrument type.");
            //}
        }

        // Close procedure for the instrument.
        public override void Close()
        {
            base.Close();
        }

        #region WLAN measurements
        public CHP_Results MeasureChannelPower(bool Average, int Aver_Num)
        {
            CHP_Results power_result = new CHP_Results();

            Set_CHP_AvgState(Average, Aver_Num);
           
            // Return channel power results data into variable Results_CHP
            string Results_CHP;
            Results_CHP = Fetch_ChannelPower();

            // Split Results_CHP data into individual elements in an array
            string[] iqCols = Results_CHP.Split(',');

            // Extract CHP only element from array into power_result variable
            Double.TryParse(iqCols[0], out power_result.power_result);
            return (power_result);

        }
        public EVM_Results MeasureEvm(bool Average, int Aver_Num)
        {
            EVM_Results EVM_Results = new EVM_Results();
       
            Set_EVM_AvgState (Average, Aver_Num);
           
            // Return EVM results data into variable Results
            string Results_EVM;
            Results_EVM = Fetch_EVM();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_EVM.Split(',');

            // Extract EVM result elements from array into EVM_Results variable
            Double.TryParse(iqCols[0], out EVM_Results.RMS_EVM_MAX_DB);
            Double.TryParse(iqCols[1], out EVM_Results.RMS_EVM_AVG_DB);
            Double.TryParse(iqCols[2], out EVM_Results.PEAK_EVM_MAX_DB);
            Double.TryParse(iqCols[3], out EVM_Results.PEAK_EVM_AVG_DB);
            Double.TryParse(iqCols[4], out EVM_Results.MAX_PEAK_EVM_INDEX);
            Double.TryParse(iqCols[5], out EVM_Results.PEAK_EVM_INDEX);
            Double.TryParse(iqCols[6], out EVM_Results.FREQ_ERR_MAX_HZ);
            Double.TryParse(iqCols[7], out EVM_Results.FREQ_ERR_AVG_HZ);
            Double.TryParse(iqCols[8], out EVM_Results.FREQ_ERR_MAX_PPM);
            Double.TryParse(iqCols[9], out EVM_Results.FREQ_ERR_AVG_PPM);
            Double.TryParse(iqCols[10], out EVM_Results.Symbol_Clock_ERR_MAX);
            Double.TryParse(iqCols[11], out EVM_Results.Symbol_Clock_ERR_AVG);
            Double.TryParse(iqCols[12], out EVM_Results.IQ_Origin_Offset_MAX);
            Double.TryParse(iqCols[13], out EVM_Results.IQ_Origin_Offset_AVG);
            Double.TryParse(iqCols[14], out EVM_Results.Gain_Imbalance_MAX);
            Double.TryParse(iqCols[15], out EVM_Results.Gain_Imbalance_AVG);
            Double.TryParse(iqCols[16], out EVM_Results.Quad_Error_MAX_Degrees);
            Double.TryParse(iqCols[17], out EVM_Results.Quad_Error_AVG_Degrees);
            Double.TryParse(iqCols[18], out EVM_Results.AVG_Burst_Power_MAX);
            Double.TryParse(iqCols[19], out EVM_Results.AVG_Burst_Power_AVG);
            Double.TryParse(iqCols[20], out EVM_Results.Peak_Burst_Power_MAX);
            Double.TryParse(iqCols[21], out EVM_Results.Peak_Burst_Power_AVG);
            Double.TryParse(iqCols[22], out EVM_Results.Peak_to_AVG_BurstPowerRatio_MAX);
            Double.TryParse(iqCols[23], out EVM_Results.Peak_to_AVG_BurstPowerRatio_AVG);
            Double.TryParse(iqCols[24], out EVM_Results.Data_MOD_Format);
            Double.TryParse(iqCols[25], out EVM_Results.Data_Bit_Rate);
            Double.TryParse(iqCols[26], out EVM_Results.Pilot_EVM_MAX_DB);
            Double.TryParse(iqCols[27], out EVM_Results.Pilot_EVM_AVG_DB);
            Double.TryParse(iqCols[28], out EVM_Results.DATA_EVM_MAX_DB);
            Double.TryParse(iqCols[29], out EVM_Results.DATA_EVM_AVG_DB);
            Double.TryParse(iqCols[30], out EVM_Results.IQ_Timing_Skew_MAX);
            Double.TryParse(iqCols[31], out EVM_Results.IQ_Timing_Skew_AVG);
            Double.TryParse(iqCols[32], out EVM_Results.RMS_EVM_MAX);
            Double.TryParse(iqCols[33], out EVM_Results.RMS_EVM_AVG);
            Double.TryParse(iqCols[34], out EVM_Results.PEAK_EVM_MAX);
            Double.TryParse(iqCols[35], out EVM_Results.PEAK_EVM_AVG);
            Double.TryParse(iqCols[36], out EVM_Results.Pilot_EVM_MAX);
            Double.TryParse(iqCols[37], out EVM_Results.Pilot_EVM_AVG);
            Double.TryParse(iqCols[38], out EVM_Results.DATA_EVM_MAX);
            Double.TryParse(iqCols[39], out EVM_Results.DATA_EVM_AVG);
            return (EVM_Results);

        }
        public SEM_Data MeasureSEMData()
        {
            SEM_Data SEM_Data = new SEM_Data();
         
            // Return SEM data into variable Results
            string Results_SEM_DATA;
            Results_SEM_DATA = Fetch_SEMData();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_SEM_DATA.Split(',');

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
        public SEM_Results MeasureSEM(bool Average, int Aver_Num)
        {
            SEM_Results SEM_Results = new SEM_Results();
          
            Set_SEM_AvgState(Average,Aver_Num);
            
            // Return SEM data into variable Results
            string Results_SEM_P_F;
            Results_SEM_P_F = Fetch_SEM();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_SEM_P_F.Split(',');

            // Extract SEM resuts element from array into SEM_Results variable
            Double.TryParse(iqCols[4], out SEM_Results.NegOFFSFREQA);
            Double.TryParse(iqCols[5], out SEM_Results.PosOFFSFREQA);
            Double.TryParse(iqCols[6], out SEM_Results.NegOFFSFREQB);
            Double.TryParse(iqCols[7], out SEM_Results.PosOFFSFREQB);
            return (SEM_Results);

        }
        public Flatness_Results MeasureChannelFlatness( bool Average, int Aver_Num)
        {
            Flatness_Results Flatness_results = new Flatness_Results();
        
            Set_FLAT_AvgState(Average, Aver_Num);

            // Return Flatness results data into variable Results_FLT
            string Results_FLAT;
            Results_FLAT = Fetch_ChannelFlatness();

            // Split Results_FLT data into individual elements in an array
            string[] iqCols = Results_FLAT.Split(',');

            // Extract Spectral Flatness element from array into Flatness_results variable
            Double.TryParse(iqCols[0], out Flatness_results.S1MAX_Point_UL_dB);
            Double.TryParse(iqCols[1], out Flatness_results.S1MAX_Point_UL_Delta);
            Double.TryParse(iqCols[2], out Flatness_results.S1MAX_Point_UL_Subcarrier);
            Double.TryParse(iqCols[3], out Flatness_results.S1MIN_Point_LL_dB);
            Double.TryParse(iqCols[4], out Flatness_results.S1MIN_Point_LL_Delta);
            Double.TryParse(iqCols[5], out Flatness_results.S1MIN_Point_LL_Subcarrier);
            Double.TryParse(iqCols[6], out Flatness_results.S2MAX_Point_UL_dB);
            Double.TryParse(iqCols[7], out Flatness_results.S2MAX_Point_UL_Delta);
            Double.TryParse(iqCols[8], out Flatness_results.S2MAX_Point_UL_Subcarrier);
            Double.TryParse(iqCols[9], out Flatness_results.S2MIN_Point_LL_dB);
            Double.TryParse(iqCols[10], out Flatness_results.S2MIN_Point_LL_Delta);
            Double.TryParse(iqCols[11], out Flatness_results.S2MIN_Point_LL_Subcarrier);
            return (Flatness_results);

        }
        public PVT_Results MeasurePVT(bool Average, int Aver_Num)
        {
            PVT_Results PVT_results = new PVT_Results();
           
            Set_PVT_AvgState(Average, Aver_Num);

            // Return PVT results data into variable Results_PVT
            string Results_PVT;
            Results_PVT = Fetch_PVT();

            // Split Results_PVT data into individual elements in an array
            string[] iqCols = Results_PVT.Split(',');
            // Extract PVT element from array into PVT_results variable
            Double.TryParse(iqCols[1], out PVT_results.Power_on_Ramp_Time);
            Double.TryParse(iqCols[2], out PVT_results.Power_down_Ramp_Time);
            Double.TryParse(iqCols[3], out PVT_results.Overall_Pass_Fail);
            Double.TryParse(iqCols[4], out PVT_results.Power_on_Ramp_Pass_Fail);
            Double.TryParse(iqCols[5], out PVT_results.Power_down_Ramp_Pass_Fail);
            Double.TryParse(iqCols[6], out PVT_results.Start_Level_Ramp_up);
            Double.TryParse(iqCols[7], out PVT_results.Start_Level_Ramp_down);
            Double.TryParse(iqCols[8], out PVT_results.Stop_Level_Ramp_up);
            Double.TryParse(iqCols[9], out PVT_results.Stop_Level_Ramp_down);
            return (PVT_results);
        }
        #endregion

        #region VSA state settings

        // Sets the wlan mode in VSA for measurements
        public void Set_Mode()
        {
            ScpiCommand("INST WLAN");
        }

        // Sets the wlan standard setup a,b,g,n,ac in VSA for measurements
        public void Set_WlanMode(int BW, string mode)
        {
            if (mode == "a" & BW == 20)
            {
              ScpiCommand("RAD:STAN AG");
            }
            else if(mode == "b" | mode == "g" & BW == 20)
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
               log.Debug("Unsupported Wlan Mode");
            }

        }

        public void System_Preset()
        {
            ScpiCommand(":SYST:PRES");
        }

        public void Correction(double loss)
        {
            string cable_l = ":CORR:SA:GAIN " + loss.ToString();
            ScpiCommand(cable_l);
        }

        [XmlIgnore]
        public string Set_frequency
        {
            set { ScpiCommand(":FREQ:CENT {0}", value); }
            get
            {
                return ScpiQuery<string>(":FREQ:CENT?");

            }
        }

        // Sets  RF Burst  Absolute Trigger Level
        [XmlIgnore]
        public string Set_RFB_level
        {
            set { ScpiCommand(":TRIG:RFB:LEV:ABS {0}", value); }
            get { return ScpiQuery<string>(":TRIG:RFB:LEV:ABS?"); }
        }

        // Optimizes signal level into ADC
         public void Set_Power_Range()
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
        public bool Set_CHP_AvgState (bool Average, int Aver_Num )
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
        public bool Set_EVM_AvgState (bool Average,int Aver_Num)
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
        public void Set_SEM_Conf()
        {
            ScpiCommand("CONF:SEM");
        }
        public void Set_SEM_MeasTyp_TP()
        { 
            ScpiCommand("SEM:TYPE TPRef");
            ScpiQuery("*OPC?");
        }

        public void Set_SEM_MeasTyp_SP()
        {
            ScpiCommand("SEM:TYPE SPRef");
            ScpiQuery("*OPC?");
        }
        //Sets averaging for SEM
        public bool Set_SEM_AvgState(bool Average, int Aver_Num)
        {
            ScpiCommand("SEM:AVER {0}", Average);
            string scpi =("SEM:AVER:COUN " + Aver_Num.ToString());
            ScpiCommand(scpi);    
            return Average;
        }
        public void Set_SEM_Trig()
        {
            ScpiCommand("TRIG:SEM:SOUR RFB");
        }
        public string Fetch_SEM()
        {
            return (ScpiQuery("FETCH:SEM7?"));
        }
        public string Fetch_SEMData()
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

                log.Debug("M90XA XAPPs Started");

        }
        #endregion



    }
}
