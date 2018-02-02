// Responsible: TEAM (asgeiver)
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

namespace TapPlugin.PaceUK
{
    [DisplayName("PaceUK\\BeIN Tx EVM Test")]
    [Description("Insert a description here")]
    public class BeInTxEvm : BeINTxBase
    {
        #region Settings
        [DisplayName("VSA\\Search Length(EVM)")]
        public string Search_Len { get; set; }
        
        [DisplayName("VSA\\No. Symbols")]
        public int Symbols { get; set; }
        
        [DisplayName("Limits \\ RMS EVM(Avg) Limit [dBm]")]
        public double RmsEvm_avgLimit { get; set; }
        #endregion

    
        public BeInTxEvm()
        {
            // Sets search length for burst 
            Search_Len = "10ms";
            // Sets no. of symbols to analyze 
            Symbols = 100;
            // Sets RMS EVM Average Limit Text Box to -27 which determines the pass fail point for EVM test
            RmsEvm_avgLimit = -27;
        }

        public override void Run()
        {
            BeInDut dut = TxParent.Bein;
            int channel = TxParent.Channel;
            double frequency = WLanChannels.ChannelToFrequencyMHz(channel);
            TraceBar limitBar = new TraceBar();

            Run_EVM_Test(log, dut, limitBar, frequency);
        }

        public void Run_EVM_Test(System.Diagnostics.TraceSource log, BeInDut Bein, TraceBar limitBar, double frequency)
        {
            // Initialise EVM settings
            XAPP.Set_EVM_Conf();
            XAPP.Set_EVM_Trig();
            XAPP.Set_Meas_Time(Search_Len);
            XAPP.Num_Syms(Symbols);
            XAPP.Set_EVM_Tracking();

            // Creates new variable for EVM data
            M9391A_XAPPS.EVM_Results EVM_Results;

            // Sets power level for set top box
            Bein.SetPowerLevel(pwrdB);

            // Returns EVM measurement data
            EVM_Results = XAPP.MeasureEvm(frequency,ABS_Trig_Level, Mode, Average);

            // Sets limit for RMS EVM Avg
            limitBar.UpperLimit = RmsEvm_avgLimit;
                    
            

            // Log info to TAP message window
            LOG_EVM_TAP(log, limitBar, EVM_Results);


            // Code to Increase Power if EVM measured does not exceed the set EVM limit Or decrese power if measured EVM exceeds EVM limit
            Inc_Dec_PWR_EVM(log, Bein, limitBar, frequency);

            // Logs results into Excel
            StoreResults_TAP(pwrdB, EVM_Results);

            //Compare EVM to limit and report verdict in Tap log if desired
            //UpgradeVerdict(limitBar.AllPassed ? VerdictType.Pass : VerdictType.Fail);
        }

        internal static void LOG_EVM_TAP(System.Diagnostics.TraceSource log, TraceBar limitBar, M9391A_XAPPS.EVM_Results EVM_Results)
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
            log.Info(" Average Burst Power (Avg): {0,19:0.00} dBm", EVM_Results.AVG_Burst_Power_AVG);
            log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB    {1}", EVM_Results.RMS_EVM_AVG_DB, limitBar.GetBar(EVM_Results.RMS_EVM_AVG_DB));
        }

        protected void Inc_Dec_PWR_EVM(System.Diagnostics.TraceSource log, BeInDut Bein, TraceBar limitBar, double frequency)
        {
            // Code to Increase Power if EVM measured does not exceed the set EVM limit
            if (limitBar.AllPassed == true)
            {
                double pwr = pwrdB;
                while (limitBar.AllPassed == true)
                {
                    pwr = pwr + 1;

                    Bein.SetPowerLevel(pwr);

                    M9391A_XAPPS.EVM_Results EVM_Results = XAPP.MeasureEvm(frequency, ABS_Trig_Level, Mode, Average);

                    log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);

                    // Log BeIN Power, EVM and Limit data to excel
                    //Excel_EVM_RMS_AVG_DB(ref EVM_Results, oSheet, ref row_evm, ref row_evm_prwdB, ref row_evm_limit);
                    StoreResults_TAP(pwr, EVM_Results);

                    if (EVM_Results.RMS_EVM_AVG_DB < RmsEvm_avgLimit)
                    {
                        //oSheet.Cells[row_evm_limit, 4] = "Pass";
                    }
                    else
                    {
                        //oSheet.Cells[row_evm_limit, 4] = "Fail";

                        log.Info(" RMS EVM (Avg)        : {0,6:0.00} dB    {1}", EVM_Results.RMS_EVM_AVG_DB, limitBar.GetBar(EVM_Results.RMS_EVM_AVG_DB));

                    }

                }

            }
            // Code to Decrease Power if EVM measured exceeds the set EVM limit
            else
            {
                double pwr = pwrdB;
                while (limitBar.AllPassed == false)
                {
                    pwr = pwr - 1;

                    Bein.SetPowerLevel(pwr);

                    M9391A_XAPPS.EVM_Results EVM_Results = XAPP.MeasureEvm(frequency, ABS_Trig_Level, Mode, Average);

                    log.Info("  RMS EVM (Avg)           : {0,1:0.00} dB", EVM_Results.RMS_EVM_AVG_DB);

                    // Log BeIN Power, EVM and Limit data to excel
                    //Excel_EVM_RMS_AVG_DB(ref EVM_Results, oSheet, ref row_evm, ref row_evm_prwdB, ref row_evm_limit);
                    StoreResults_TAP(pwr, EVM_Results);

                    if (EVM_Results.RMS_EVM_AVG_DB < RmsEvm_avgLimit)
                    {
                        //oSheet.Cells[row_evm_limit, 4] = "Pass";
                        log.Info("  RMS EVM (Avg)         : {0,6:0.00} dB    {1}", EVM_Results.RMS_EVM_AVG_DB, limitBar.GetBar(EVM_Results.RMS_EVM_AVG_DB));
                    }
                    else
                    {
                        //oSheet.Cells[row_evm_limit, 4] = "Fail";
                        log.Info("  RMS EVM (Avg)          : {0,6:0.00} dB    {1}", EVM_Results.RMS_EVM_AVG_DB, limitBar.GetBar(EVM_Results.RMS_EVM_AVG_DB));
                    }
                }
            }
        }

        private void StoreResults_TAP(double targetPower, M9391A_XAPPS.EVM_Results evm)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Target Power [dBm]");
            type.DimensionTitles.Add("RMS EVM (Avg) [dB]");
            type.DimensionTitles.Add("RMS EVM (Max) [dB]");
            type.DimensionTitles.Add("Pass/Fail");
            type.DimensionTitles.Add(" AVG_Burst_Power_AVG");
            type.Name = "EVM Results";
            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(targetPower);
            res.Doubles.Add(evm.RMS_EVM_AVG_DB);
            res.Doubles.Add(evm.RMS_EVM_MAX_DB);
            res.Booleans.Add(evm.RMS_EVM_AVG_DB > RmsEvm_avgLimit);
            res.Doubles.Add(evm.AVG_Burst_Power_AVG);
            
            this.Results.StoreResult(res);

           

        }

    }
}
