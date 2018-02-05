using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tap.Wlan.Demo
{
    public class AnalyzerModels
    {
        public AnalyzerModels()
        {
            
        }
        public List<string> AnalyzerModel()
        {
            Configuration myConfig = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);

            AppSettingsSection appSettingsSection = (AppSettingsSection)myConfig.GetSection("appSettings");

            string analyzer1 = appSettingsSection.Settings["benchSA1"].Value;
            string analyzer2 = appSettingsSection.Settings["benchSA2"].Value;
            string analyzer3 = appSettingsSection.Settings["benchSA3"].Value;
            string analyzer4 = appSettingsSection.Settings["modSA1"].Value;
            string analyzer5 = appSettingsSection.Settings["modSA2"].Value;
            string analyzer6 = appSettingsSection.Settings["modSA3"].Value;
            string analyzer7 = appSettingsSection.Settings["modSA4"].Value;
            string analyzer8 = appSettingsSection.Settings["modSA5"].Value;

            List<string> analyzer = new List<string>();
            analyzer.Add(analyzer1);
            analyzer.Add(analyzer2);
            analyzer.Add(analyzer3);
            analyzer.Add(analyzer4);
            analyzer.Add(analyzer5);
            analyzer.Add(analyzer6);
            analyzer.Add(analyzer7);
            analyzer.Add(analyzer8);
            return analyzer;
        }
        public void SEMMeasurementData()
        {
            Configuration myConfig = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);

            AppSettingsSection appSettingsSection = (AppSettingsSection)myConfig.GetSection("appSettings");

            //string TotalPowerRef = appSettingsSection.Settings["TotalPowerRef"].Value;
           
            //string LowerAbsPowerA  = appSettingsSection.Settings["LowerAbsPowerA"].Value;
            //string LowerDeltaLimitA = appSettingsSection.Settings["LowerDeltaLimitA"].Value;
            //string LowerFreqA = appSettingsSection.Settings["LowerFreqA"].Value;
            //string UpperAbsPowerA = appSettingsSection.Settings["UpperAbsPowerA"].Value;
            //string UpperDeltaLimitA = appSettingsSection.Settings["UpperDeltaLimitA"].Value;
            //string UpperFreqA = appSettingsSection.Settings["UpperFreqA"].Value;
            //string LowerAbsPowerB = appSettingsSection.Settings["LowerAbsPowerB"].Value;
            //string LowerDeltaLimitB = appSettingsSection.Settings["LowerDeltaLimitB"].Value;
            //string LowerFreqB = appSettingsSection.Settings["LowerFreqB"].Value;
            //string UpperAbsPowerB = appSettingsSection.Settings["UpperAbsPowerB"].Value;
            //string UpperDeltaLimitB = appSettingsSection.Settings["UpperDeltaLimitB"].Value;
            //string UpperFreqB = appSettingsSection.Settings["UpperFreqB"].Value;


            string sem1 = appSettingsSection.Settings["sem1"].Value;
            string sem12 = appSettingsSection.Settings["sem12"].Value;
            string sem70 = appSettingsSection.Settings["sem70"].Value;
            string sem14 = appSettingsSection.Settings["sem14"].Value;
            string sem17 = appSettingsSection.Settings["sem17"].Value;
            string sem71 = appSettingsSection.Settings["sem71"].Value;
            string sem19 = appSettingsSection.Settings["sem19"].Value;
            string sem22 = appSettingsSection.Settings["sem22"].Value;
            string sem72 = appSettingsSection.Settings["sem72"].Value;
            string sem24 = appSettingsSection.Settings["sem24"].Value;
            string sem27 = appSettingsSection.Settings["sem27"].Value;
            string sem73 = appSettingsSection.Settings["sem73"].Value;
            string sem29 = appSettingsSection.Settings["sem29"].Value;


        }
    }
}
