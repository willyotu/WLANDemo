// Responsible: TEAM (Will Attoh)
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

namespace TapPlugin.PaceUK
{
    [DisplayName("PaceUK\\BCM4366 Tx EVM Test")]
    [Description("Insert a description here")]
    public class BCM4366TxEvm : BCM4366TxBase
    {
        #region Settings
       
        [DisplayName("Search Length")]
        public string Search_Len { get; set; }

        [DisplayName("No. of Symbols")]
        public int Symbols { get; set; }

        [DisplayName("Limits \\ RMS EVM(Avg) Limit [dBm]")]
        public double RmsEvm_avgLimit { get; set; }

         BCM4366 BCM4366 = null;
        #endregion

        public BCM4366TxEvm()
        {
            // Sets search length for burst 
            Search_Len = "10ms";
            // Sets no. of symbols to analyze 
            Symbols = 100;
            // Sets RMS EVM Average Limit Text Box to -27 which determines the pass fail point for EVM test
            RmsEvm_avgLimit = -27;
            BCM4366 = new BCM4366();
        }

        public override void Run()
        {
            BCM4366 dut = TxParent.BCM4366;
            int bw = TxParent.BW;
            int channel = TxParent.Channel;
            // Select frequency based on bandwidth
            double frequency = BCM4366.choose_freq(bw, channel);
            TraceBar limitBar = new TraceBar();
            Run_EVM_Test(log, dut, frequency);
           
        }

        private void Run_EVM_Test(System.Diagnostics.TraceSource log, BCM4366 BCM4366, double frequency)
        {
            // Sets power level for set top box
            BCM4366.BCM4366_SetPowerLevel(pwrdB);

            // Initialise EVM settings
            XAPP.Set_frequency = (frequency * 1000000).ToString();
            XAPP.Set_WlanMode(BW, mode.ToString());
            XAPP.Set_EVM_Conf();
            XAPP.Set_RFB_level = ABS_Trig_Level.ToString();
            XAPP.Set_EVM_Trig();
            XAPP.Set_Meas_Time(Search_Len);
            XAPP.Num_Syms(Symbols);
            XAPP.Set_EVM_Tracking();
            XAPP.Set_Power_Range();

            // Creates new variable for EVM data
            M9391A_XAPPS.EVM_Results EVM_Results;

            // Returns EVM measurement data
            EVM_Results = XAPP.MeasureEvm(average, Aver_Num);

            // Logs results into Excel
            StoreResults_TAP(EVM_Results);

            // Log info to TAP message window
            LOG_EVM_TAP(log, EVM_Results);

            if (EVM_Results.RMS_EVM_AVG_DB < RmsEvm_avgLimit)
            {
                Verdict = Verdict.Pass;
            }
            else
            {
                Verdict = Verdict.Fail;
            }


            // Code to Increase Power if EVM measured does not exceed the set EVM limit Or decrese power if measured EVM exceeds EVM limit
            Inc_Dec_PWR_EVM(log, BCM4366,frequency);

        }

        internal static void LOG_EVM_TAP(System.Diagnostics.TraceSource log, M9391A_XAPPS.EVM_Results EVM_Results)
        {
            log.Info("  Running Modulation Analysis test ");
            log.Info("  RMS EVM (Max)           : {0,0:0.00} dB", EVM_Results.RMS_EVM_MAX_DB);
            log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);
            log.Info("  Peak EVM (Max)          : {0,2:0.00} dB", EVM_Results.PEAK_EVM_MAX_DB);
            log.Info("  Peak EVM (Avg)          : {0,3:0.00} dB", EVM_Results.PEAK_EVM_AVG_DB);
            log.Info("  Pilot EVM (Max)         : {0,26:0.00} dB", EVM_Results.Pilot_EVM_MAX_DB);
            log.Info("  Pilot EVM (Avg)         : {0,27:0.00} dB", EVM_Results.Pilot_EVM_AVG_DB);
            log.Info("  Data EVM (Max)          : {0,28:0.00} dB", EVM_Results.DATA_EVM_MAX_DB);
            log.Info("  Data EVM (Avg)          : {0,29:0.00} dB", EVM_Results.DATA_EVM_AVG_DB);
            log.Info("  Average Burst Power (Avg): {0,19:0.00} dBm", EVM_Results.AVG_Burst_Power_AVG);

        }

        protected void Inc_Dec_PWR_EVM(System.Diagnostics.TraceSource log, BCM4366 BCM4366, double frequency)
        {
            // Code to Increase Power if EVM measured does not exceed the set EVM limit
            if (Verdict == Verdict.Pass)
            {
                double pwr = pwrdB;
                while (Verdict == Verdict.Pass)
                {
                    pwr = pwr + 1;

                    BCM4366.BCM4366_SetPowerLevel(pwr);

                    M9391A_XAPPS.EVM_Results EVM_Results = XAPP.MeasureEvm(average, Aver_Num);

                    log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);

                    if (EVM_Results.RMS_EVM_AVG_DB < RmsEvm_avgLimit)
                    {
                        log.Info(" RMS EVM (Avg)        : {0,6:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);
                    }
                    else
                    {
                        log.Info(" RMS EVM (Avg)        : {0,6:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);
                        StoreFinalResults_TAP(pwr, EVM_Results);
                        // Update results to show test condition has been met
                        Verdict = Verdict.Pass;
                        break;
                     }
                    
                }

            }
            // Code to Decrease Power if EVM measured exceeds the set EVM limit
            else
            {
                double pwr = pwrdB;
                while (Verdict == Verdict.Fail)
                {
                    TestPlan.Sleep();
                    pwr = pwr - 1;

                    BCM4366.BCM4366_SetPowerLevel(pwr);

                    // Code to Increase Power if EVM measured does not exceed the set EVM limit
                    M9391A_XAPPS.EVM_Results EVM_Results = XAPP.MeasureEvm(average, Aver_Num);

                    log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);

                    if (EVM_Results.RMS_EVM_AVG_DB < RmsEvm_avgLimit)
                    {
                        log.Info("  RMS EVM (Avg)          : {0,6:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);
                        StoreFinalResults_TAP(pwr, EVM_Results);
                        Verdict = Verdict.Pass;
                        break;
                    }
                    else
                    {
                        log.Info("  RMS EVM (Avg)          : {0,6:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);
                    }

                }
            }
        }
        
        private void StoreResults_TAP(M9391A_XAPPS.EVM_Results evm)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Channel");
            type.DimensionTitles.Add("Start Power");
            type.DimensionTitles.Add("Measured Channel Power [dBm]");
            type.DimensionTitles.Add("Pass/Fail");
            type.DimensionTitles.Add("Initial RMS EVM (Avg) [dB]");
            type.DimensionTitles.Add("Initial RMS EVM (Max) [dB]");

            type.Name = "EVM Results";
            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(TxParent.Channel);
            res.Doubles.Add(TxParent.pwrdB);
            res.Doubles.Add(evm.AVG_Burst_Power_AVG);
            if (evm.RMS_EVM_AVG_DB > RmsEvm_avgLimit)
            {
                res.Strings.Add("Fail");
            }
            else
            {
                res.Strings.Add("Pass");
            }
            res.Doubles.Add(evm.RMS_EVM_AVG_DB);
            res.Doubles.Add(evm.RMS_EVM_MAX_DB);

            this.Results.StoreResult(res);
        }
        
        private void StoreFinalResults_TAP(double pwr, M9391A_XAPPS.EVM_Results evm)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Pass/Fail");
            type.DimensionTitles.Add("Final Measured Channel Power [dBm]");
            type.DimensionTitles.Add("Final Power");
            type.DimensionTitles.Add("Final RMS EVM (Avg) [dB]");
            type.DimensionTitles.Add("Final RMS EVM (Max) [dB]");

            type.Name = "EVM Results";
            TestStepResult res = new TestStepResult();
            res.Type = type;
            if (evm.RMS_EVM_AVG_DB > RmsEvm_avgLimit)
            {
                res.Strings.Add("Fail");
            }
            else
            {
                res.Strings.Add("Pass");
            }
            res.Doubles.Add(evm.AVG_Burst_Power_AVG);
            res.Doubles.Add(pwr);
            res.Doubles.Add(evm.RMS_EVM_AVG_DB);
            res.Doubles.Add(evm.RMS_EVM_MAX_DB);

            this.Results.StoreResult(res);



        }
    }
}