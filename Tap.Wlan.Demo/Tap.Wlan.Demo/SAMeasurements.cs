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

        
    }
}
