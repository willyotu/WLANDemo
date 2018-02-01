using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tap.Wlan.Demo
{
    public class SAMeasurements
    {
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

        public SEM_Data MeasureSEMData()
        {
            AnalyzerModels config = new AnalyzerModels();
            SAInstrument benchSA = new SAInstrument();
            SEM_Data SEM_Data = new SEM_Data();

            // Return SEM data into variable Results
            string Results_SEM_DATA;
            Results_SEM_DATA = benchSA.Fetch_SEMData();

            // Split Results data into individual elements in an array
            string[] iqCols = Results_SEM_DATA.Split(',');

            // Extract SEM data element from array into SEM_Data variable
            config.SEMMeasurementData();
            return (SEM_Data);
        }
    }
}
