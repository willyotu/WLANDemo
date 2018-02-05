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

namespace Tap.Wlan.Demo
{
    [Display("Modulation Analysis", Groups: new[] { "WLAN Demo", "Transmitter Measurements" }, Description: "Insert a description here")]
    [AllowAsChildIn(typeof(TransmitterStep))]
    public class EVMTestStep : TestStep
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        [Unit("ms", UseEngineeringPrefix: true)]
        [Display("Search Length(EVM)")]
        public double searchLength { get; set; }

        [Display("No. Symbols", Description: "Enter the number of symbols to analyze")]
        public int symbols { get; set; }

        [Unit("dB", UseEngineeringPrefix: true)]
        [Display("RMS EVM(Avg) Limit ", Group: "Limit")]
        public double rmsEvmAvgLimit { get; set; }
        #endregion
        public EVMTestStep()
        {
            // ToDo: Set default values for properties / settings.
            // Sets search length for burst 
            searchLength = 10;
            // Sets no. of symbols to analyze 
            symbols = 100;
            // Sets RMS EVM Average Limit Text Box to -27 which determines the pass fail point for EVM test
            rmsEvmAvgLimit = -27;
        }
        
        public override void Run()
        {
            EVMTestRun();
        }

        private void EVMTestRun()
        {
            BCM4366 chipset = GetParent<TransmitterStep>().bcm4366;
            SAInstrument xAPP = GetParent<TransmitterStep>().signalAnalyzer;
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;

            //Set set-top power level to pwrdB value
            double chipsetPowerLevel = GetParent<TransmitterStep>().pwrdB;
            chipset.bcm4366SetPowerLevel(chipsetPowerLevel);

            // Initialise EVM settings
            xAPP.EVMConfigure();
            xAPP.EVMTrigger();
            xAPP.BurstSearchLength(searchLength);
            xAPP.NumberOfSymbols(symbols);
            xAPP.EVMTracking();

            // Sets power level for set top box
            chipset.bcm4366SetPowerLevel(chipsetPowerLevel);

            // Returns EVM measurement data
            SAMeasurements.EVMResults EVMResults = xAPP.MeasureEvm(average, numberOfAverages);

            // Sets limit for RMS EVM Avg
            //limitBar.UpperLimit = RmsEvmAvgLimit;
            
            // Return EVM Metric results
            ModulationErrorResults(xAPP);

        }

        private void ModulationErrorResults(SAInstrument xAPP)
        {
            bool average = GetParent<TransmitterStep>().average;
            int numberOfAverages = GetParent<TransmitterStep>().numberOfAverages;
            SAMeasurements.EVMResults evmResults = xAPP.MeasureEvm(average, numberOfAverages);
            var EVMMetrics = new string[] {
                 "RMS EVM (Max)  dB",
                 "RMS EVM (Avg)  dB",
                 "Peak EVM (Max) dB",
                 "Peak EVM (Avg) dB",
                 "Pilot EVM (Max) dB",
                 "Pilot EVM (Avg) dB",
                 "Data EVM (Max)  dB",
                 "Data EVM (Avg)  dB",
                 "Average Burst Power (Avg) dB",
                 "RMS EVM (Avg) dB ",
            };

            var EVMResults = new double[] {
                 Math.Round(evmResults.RMS_EVM_MAX_DB,2),
                 Math.Round(evmResults.RMS_EVM_AVG_DB,2),
                 Math.Round(evmResults.PEAK_EVM_MAX_DB,2),
                 Math.Round(evmResults.PEAK_EVM_AVG_DB,2),
                 Math.Round(evmResults.Pilot_EVM_MAX_DB,2),
                 Math.Round(evmResults.Pilot_EVM_AVG_DB,2),
                 Math.Round(evmResults.DATA_EVM_MAX_DB,2),
                 Math.Round(evmResults.DATA_EVM_AVG_DB,2),
                 Math.Round(evmResults.AVG_Burst_Power_AVG ,2),
                 Math.Round(evmResults.RMS_EVM_AVG,2)
            };
       

            var EVMResultsUnit = new string[] {
                 "dB","dB","dB","dB","dB","dB","dB","dB","dB","dB"
            };

            if (evmResults.RMS_EVM_AVG_DB < rmsEvmAvgLimit)
            {
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                UpgradeVerdict(Verdict.Fail);
            }
           
            Results.PublishTable("EVM Data", new List<string> { "EVM Metrics", "EVMResults", "EVM Unit"}, EVMMetrics, EVMResults, EVMResultsUnit);

                        
        }
    }
}
