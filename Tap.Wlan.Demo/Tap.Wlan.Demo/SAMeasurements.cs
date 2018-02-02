using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tap.Wlan.Demo
{
    public class SAMeasurements
    {

        public SAMeasurements()
        {

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
        }

        public class SEMLimitTest
        {
            public double NegOFFSFREQA;//4
            public double PosOFFSFREQA;//5
            public double NegOFFSFREQB;//6
            public double PosOFFSFREQB;//7
        }

        public class EVMResults
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
        }

      
    }
}
