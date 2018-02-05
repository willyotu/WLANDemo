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
    [DisplayName("PaceUK\\BCM4366 Tx SEM Test")]
    [Description("Insert a description here")]
    [AllowAsChildIn(typeof(BCM4366_Transmitter_Tests))] // this line makes sure that this step can only be inserted as a child to "Transmitter Tests"
    public class BCM4366TxSem : BCM4366TxBase
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        BCM4366 BCM4366 = null;
        #endregion

        public BCM4366TxSem()
        {
            // ToDo: Set default values for properties / settings.
            BCM4366 = new BCM4366();
        }

        public override void Run()
        {
            BCM4366 dut = TxParent.BCM4366;
            int bw = TxParent.BW;
            int channel = TxParent.Channel;
            // Select frequency based on bandwidth
            double frequency = BCM4366.choose_freq(bw, channel);
       
            Run_SEM_Test(log, dut, frequency);
            
        }

        private void Run_SEM_Test(System.Diagnostics.TraceSource log, BCM4366 BCM4366, double frequency)
        {
            //Set set-top power level to pwrdB value
            BCM4366.BCM4366_SetPowerLevel(TxParent.pwrdB);
            // Initialise SEM settings    
            XAPP.Set_frequency = (frequency * 1000000).ToString();
            XAPP.Set_WlanMode(BW, mode.ToString());
            XAPP.Set_SEM_Conf();
            XAPP.Set_RFB_level = ABS_Trig_Level.ToString();
            XAPP.Set_SEM_Trig();
            XAPP.Set_Power_Range();
           
             // Returns SEM Pass/Fail Test results
             M9391A_XAPPS.SEM_Results SEM_Results = XAPP.MeasureSEM(average,Aver_Num);

             // Returns SEM data results
             M9391A_XAPPS.SEM_Data SEM_Data = XAPP.MeasureSEMData();
           
             // Log SEM results into TAP message window
             Log_SEM_TAP(log, SEM_Results, SEM_Data);

            
            // Code to Increase Power if Channel Power measured does not exceed the SEM limit Or decrese power if measured power exceeds SEM limit
            Inc_Dec_PWR_SEM(log, SEM_Results, BCM4366, frequency);
        }

        public void Log_SEM_TAP(System.Diagnostics.TraceSource log,  M9391A_XAPPS.SEM_Results SEM_Results, M9391A_XAPPS.SEM_Data SEM_Data)
        {
            // Log SEM pass/Fail test into TAP message window
            log.Info("  Running Spectrum Emission Mask Test ");
            log.Info("  Negative Offset Frequency (A)        : {0,4:0.00} ", SEM_Results.NegOFFSFREQA);
            log.Info("  Positive Offset Frequency (A)        : {0,5:0.00} ", SEM_Results.PosOFFSFREQA);
            log.Info("  Negative Offset Frequency (B)        : {0,6:0.00} ", SEM_Results.NegOFFSFREQB);
            log.Info("  Positive Offset Frequency (B)        : {0,7:0.00} ", SEM_Results.PosOFFSFREQB);

            if (SEM_Results.NegOFFSFREQA == 0 & SEM_Results.PosOFFSFREQA == 0 & SEM_Results.NegOFFSFREQB == 0 & SEM_Results.PosOFFSFREQB == 0)
            {
                Verdict = Verdict.Pass;
            }
            else
            {
                Verdict = Verdict.Fail;
             }

            // Log SEM Data into TAP message window
            log.Info("LowerAbsPowerA          : {0,12:0.00} dB", SEM_Data.LowerAbsPowerA);
            log.Info("LowerDeltaLimitA        : {0,70:0.00} dB", SEM_Data.LowerDeltaLimitA);
            log.Info("LowerFreqA              : {0,14:0.00} MHz", SEM_Data.LowerFreqA);
            log.Info("UpperAbsPowerA          : {0,17:0.00} dB", SEM_Data.UpperAbsPowerA);
            log.Info("UpperDeltaLimitA        : {0,71:0.00} dB", SEM_Data.UpperDeltaLimitA);
            log.Info("UpperFreqA              : {0,19:0.00} MHz", SEM_Data.UpperFreqA);
            log.Info("LowerAbsPowerB          : {0,22:0.00} dB", SEM_Data.LowerAbsPowerB);
            log.Info("LowerDeltaLimitB        : {0,72:0.00} dB", SEM_Data.LowerDeltaLimitB);
            log.Info("LowerFreqB              : {0,24:0.00} MHz", SEM_Data.LowerFreqB);
            log.Info("UpperAbsPowerB          : {0,27:0.00} dB", SEM_Data.UpperAbsPowerB);
            log.Info("UpperDeltaLimitB        : {0,73:0.00} dB", SEM_Data.UpperDeltaLimitB);
            log.Info("UpperFreqB              : {0,29:0.00} MHz", SEM_Data.UpperFreqB);
           
            // Change measurement type to Total Power Reference to log the measured power
            XAPP.Set_SEM_MeasTyp_TP();
            M9391A_XAPPS.SEM_Data data = XAPP.MeasureSEMData();
            log.Info("  Measured Channel Power           : {0,1:0.00} dBm", data.TotalPowerRef);
           
            // Saves results to csv file
            StoreResults_TAP(SEM_Results, data);


        }

        private void Inc_Dec_PWR_SEM(System.Diagnostics.TraceSource log, M9391A_XAPPS.SEM_Results SEM_Results, BCM4366 BCM4366,  double frequency)
        {
            double pwr = pwrdB;
            
            // Code to Increase Power if Channel Power measured does not fail SEM limit
            if (Verdict == Verdict.Pass)
            {

                pwr = pwr + 1;

                while (Verdict == Verdict.Pass)
                {
                    pwr = pwr + 1;

                    BCM4366.BCM4366_SetPowerLevel(pwr);
                   
                    // Change measurement type to Spectrum Peak Reference
                    XAPP.Set_SEM_MeasTyp_SP();
                    
                    // Pause temporarily to ensure measurement type has changed
                    TestPlan.Sleep(100);

                    // Optimize power level going to ADC before making measurement
                    XAPP.Set_Power_Range();

                    // Returns SEM Pass/Fail Test results
                    M9391A_XAPPS.SEM_Results sem = XAPP.MeasureSEM(average,Aver_Num);
                  
                    if (sem.NegOFFSFREQA == 1 | sem.PosOFFSFREQA == 1 | sem.NegOFFSFREQB == 1 | sem.PosOFFSFREQB == 1)
                    {
                       
                        log.Info("  Negative Offset Frequency (A)        : {0,4:0.00} ", sem.NegOFFSFREQA);
                        log.Info("  Positive Offset Frequency (A)        : {0,5:0.00} ", sem.PosOFFSFREQA);
                        log.Info("  Negative Offset Frequency (B)        : {0,6:0.00} ", sem.NegOFFSFREQB);
                        log.Info("  Positive Offset Frequency (B)        : {0,7:0.00} ", sem.PosOFFSFREQB);

                        // Change measurement type to Total Power Reference to log the measured power
                        XAPP.Set_SEM_MeasTyp_TP();
                        M9391A_XAPPS.SEM_Data data = XAPP.MeasureSEMData();
                        log.Info("  Final Measured Channel Power         : {0,1:0.00} dBm", data.TotalPowerRef);

                        // Saves results to csv file
                        StoreFinalResults_TAP(pwr, sem, data);

                        // Optimize power level going to ADC top prevent high power from damaging analyzer
                        XAPP.Set_Power_Range();

                        // Update results to show test condition has been met
                        Verdict = Verdict.Pass;

                        break;
                    }
                    else
                    {
                        log.Info("  Negative Offset Frequency (A)        : {0,4:0.00} ", sem.NegOFFSFREQA);
                        log.Info("  Positive Offset Frequency (A)        : {0,5:0.00} ", sem.PosOFFSFREQA);
                        log.Info("  Negative Offset Frequency (B)        : {0,6:0.00} ", sem.NegOFFSFREQB);
                        log.Info("  Positive Offset Frequency (B)        : {0,7:0.00} ", sem.PosOFFSFREQB);

                        // Change measurement type to Total Power Reference to log the measured power
                        XAPP.Set_SEM_MeasTyp_TP();
                        M9391A_XAPPS.SEM_Data data = XAPP.MeasureSEMData();
                        log.Info("  Measured Channel Power               : {0,1:0.00} dBm", data.TotalPowerRef);
                    }
                  
                }

            }
            // Code to Decrease Power if SEM measured exceeds the SEM limit
            else
            {

                while (Verdict == Verdict.Fail)
                {
                    TestPlan.Sleep();
                    pwr = pwr - 1;

                   BCM4366.BCM4366_SetPowerLevel(pwr);

                   // Change measurement type to Spectrum Peak Reference
                   XAPP.Set_SEM_MeasTyp_SP();

                   // Pause temporarily to ensure measurement type has changed
                   TestPlan.Sleep(100);

                   // Optimize power level going to ADC before making measurement
                   XAPP.Set_Power_Range();

                   // Returns SEM Pass/Fail Test results
                   M9391A_XAPPS.SEM_Results sem = XAPP.MeasureSEM(average,Aver_Num);
                                      
                   if (sem.NegOFFSFREQA == 1 | sem.PosOFFSFREQA == 1 | sem.NegOFFSFREQB == 1 | sem.PosOFFSFREQB == 1)
                   {
                        log.Info("  Negative Offset Frequency (A)        : {0,4:0.00} ", sem.NegOFFSFREQA);
                        log.Info("  Positive Offset Frequency (A)        : {0,5:0.00} ", sem.PosOFFSFREQA);
                        log.Info("  Negative Offset Frequency (B)        : {0,6:0.00} ", sem.NegOFFSFREQB);
                        log.Info("  Positive Offset Frequency (B)        : {0,7:0.00} ", sem.PosOFFSFREQB);
                       
                        // Change measurement type to Total Power Reference to log the measured power
                        XAPP.Set_SEM_MeasTyp_TP();
                        M9391A_XAPPS.SEM_Data data = XAPP.MeasureSEMData();
                        log.Info("  Measured Channel Power               : {0,1:0.00} dBm", data.TotalPowerRef);
                    }
                    else
                    {
                        log.Info("  Negative Offset Frequency (A)        : {0,4:0.00} ", sem.NegOFFSFREQA);
                        log.Info("  Positive Offset Frequency (A)        : {0,5:0.00} ", sem.PosOFFSFREQA);
                        log.Info("  Negative Offset Frequency (B)        : {0,6:0.00} ", sem.NegOFFSFREQB);
                        log.Info("  Positive Offset Frequency (B)        : {0,7:0.00} ", sem.PosOFFSFREQB);
                        
                        // Change measurement type to Total Power Reference to log the measured power 
                        XAPP.Set_SEM_MeasTyp_TP();
                        M9391A_XAPPS.SEM_Data data = XAPP.MeasureSEMData();
                        log.Info("  Final Measured Channel Power         : {0,1:0.00} dBm", data.TotalPowerRef);
                        
                        StoreFinalResults_TAP(pwr, sem, data);

                        // Optimize power level going to ADC top prevent high power from damaging analyzer
                        XAPP.Set_Power_Range();

                        Verdict = Verdict.Pass;
                        break;
                    }
                }

            }

        }

        private void StoreResults_TAP(M9391A_XAPPS.SEM_Results sem, M9391A_XAPPS.SEM_Data data)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Channel");
            type.DimensionTitles.Add("Start Power");
            type.DimensionTitles.Add("Measured Channel Power [dBm]");
            type.DimensionTitles.Add("Pass/Fail");
            type.DimensionTitles.Add("NegOFFSFREQA");//4
            type.DimensionTitles.Add("PosOFFSFREQA");//5
            type.DimensionTitles.Add("NegOFFSFREQB");//6
            type.DimensionTitles.Add("PosOFFSFREQB");//7
            type.DimensionTitles.Add("LowerAbsPowerA");//12
            type.DimensionTitles.Add("LowerDeltaLimitA");//70
            type.DimensionTitles.Add("LowerFreqA");//14
            type.DimensionTitles.Add("UpperAbsPowerA");//17
            type.DimensionTitles.Add("UpperDeltaLimitA");//71
            type.DimensionTitles.Add("UpperFreqA");//19
            type.DimensionTitles.Add("LowerAbsPowerB");//22
            type.DimensionTitles.Add("LowerDeltaLimitB");//72
            type.DimensionTitles.Add("LowerFreqB");//24
            type.DimensionTitles.Add("UpperAbsPowerB");//27
            type.DimensionTitles.Add("UpperDeltaLimitB");//73
            type.DimensionTitles.Add("UpperFreqB");//29

            type.Name = "SEM Results";

            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(TxParent.Channel);
            res.Doubles.Add(TxParent.pwrdB);
            res.Doubles.Add(data.TotalPowerRef);
            if (sem.NegOFFSFREQA == 1 | sem.PosOFFSFREQA == 1 | sem.NegOFFSFREQB == 1 | sem.PosOFFSFREQB == 1)
            {
                res.Strings.Add("Fail");
            }
            else
            {
                res.Strings.Add("Pass");
            }
            res.Doubles.Add(sem.NegOFFSFREQA);//4
            res.Doubles.Add(sem.PosOFFSFREQA);//5
            res.Doubles.Add(sem.NegOFFSFREQB);//6
            res.Doubles.Add(sem.PosOFFSFREQB);//7
            res.Doubles.Add(data.LowerAbsPowerA);//12
            res.Doubles.Add(data.LowerDeltaLimitA);//70
            res.Doubles.Add(data.LowerFreqA);//14
            res.Doubles.Add(data.UpperAbsPowerA);//17
            res.Doubles.Add(data.UpperDeltaLimitA);//71
            res.Doubles.Add(data.UpperFreqA);//19
            res.Doubles.Add(data.LowerAbsPowerB);//22
            res.Doubles.Add(data.LowerDeltaLimitB);//72
            res.Doubles.Add(data.LowerFreqB);//24
            res.Doubles.Add(data.UpperAbsPowerB);//27
            res.Doubles.Add(data.UpperDeltaLimitB);//73
            res.Doubles.Add(data.UpperFreqB);//29
            this.Results.StoreResult(res);
        }

        private void StoreFinalResults_TAP(double pwr, M9391A_XAPPS.SEM_Results sem, M9391A_XAPPS.SEM_Data data)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Final Power");
            type.DimensionTitles.Add("Final Measured Channel Power [dBm]");
            type.DimensionTitles.Add("Pass/Fail");
            type.DimensionTitles.Add("NegOFFSFREQA");//4
            type.DimensionTitles.Add("PosOFFSFREQA");//5
            type.DimensionTitles.Add("NegOFFSFREQB");//6
            type.DimensionTitles.Add("PosOFFSFREQB");//7
            type.DimensionTitles.Add("LowerAbsPowerA");//12
            type.DimensionTitles.Add("LowerDeltaLimitA");//70
            type.DimensionTitles.Add("LowerFreqA");//14
            type.DimensionTitles.Add("UpperAbsPowerA");//17
            type.DimensionTitles.Add("UpperDeltaLimitA");//71
            type.DimensionTitles.Add("UpperFreqA");//19
            type.DimensionTitles.Add("LowerAbsPowerB");//22
            type.DimensionTitles.Add("LowerDeltaLimitB");//72
            type.DimensionTitles.Add("LowerFreqB");//24
            type.DimensionTitles.Add("UpperAbsPowerB");//27
            type.DimensionTitles.Add("UpperDeltaLimitB");//73
            type.DimensionTitles.Add("UpperFreqB");//29

            type.Name = "SEM Results";

            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(pwr);
            res.Doubles.Add(data.TotalPowerRef);
            if (sem.NegOFFSFREQA == 0 & sem.PosOFFSFREQA == 0 & sem.NegOFFSFREQB == 0 & sem.PosOFFSFREQB == 0)
            {
                res.Strings.Add("Pass");
                res.Doubles.Add(sem.NegOFFSFREQA);//4
                res.Doubles.Add(sem.PosOFFSFREQA);//5
                res.Doubles.Add(sem.NegOFFSFREQB);//6
                res.Doubles.Add(sem.PosOFFSFREQB);//7
            }
            else
            {
                res.Strings.Add("Fail");
                res.Doubles.Add(sem.NegOFFSFREQA);//4
                res.Doubles.Add(sem.PosOFFSFREQA);//5
                res.Doubles.Add(sem.NegOFFSFREQB);//6
                res.Doubles.Add(sem.PosOFFSFREQB);//7
            }
            res.Doubles.Add(data.LowerAbsPowerA);//12
            res.Doubles.Add(data.LowerDeltaLimitA);//70
            res.Doubles.Add(data.LowerFreqA);//14
            res.Doubles.Add(data.UpperAbsPowerA);//17
            res.Doubles.Add(data.UpperDeltaLimitA);//71
            res.Doubles.Add(data.UpperFreqA);//19
            res.Doubles.Add(data.LowerAbsPowerB);//22
            res.Doubles.Add(data.LowerDeltaLimitB);//72
            res.Doubles.Add(data.LowerFreqB);//24
            res.Doubles.Add(data.UpperAbsPowerB);//27
            res.Doubles.Add(data.UpperDeltaLimitB);//73
            res.Doubles.Add(data.UpperFreqB);//29

            this.Results.StoreResult(res);
        }
    }
}