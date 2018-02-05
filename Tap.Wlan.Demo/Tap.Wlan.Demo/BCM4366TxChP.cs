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
using System.Reflection;

namespace TapPlugin.PaceUK
{
    [DisplayName("PaceUK\\BCM4366 Tx Channel Power Test")]
    [Description("Insert a description here")]
  
    public class BCM4366TxChP : BCM4366TxBase
    {
        #region Settings
        [DisplayName("Limits \\ Channel Power Upper Limit [dBm]")]
        public double CHPUpperLimit { get; set; }

        [DisplayName("Limits \\ Channel Power Lower Limit [dBm]")]
        public double CHPLowerLimit { get; set; }
         BCM4366 BCM4366 = null;
        #endregion
        public BCM4366TxChP()
        {
            // Sets Channel Power Upper Limit Text Box to 15 which determines Upper Limit of the Power Test

            CHPUpperLimit = 15;

            // Sets Channel Power Lower Limit Text Box to 5 which determines Lower Limit of the Power Test
            CHPLowerLimit = 5;
            BCM4366 = new BCM4366();
        }

        public override void Run()
        {
            BCM4366 dut = TxParent.BCM4366;
            int bw = TxParent.BW;
            int channel = TxParent.Channel;
            // Select frequency based on bandwidth
            double frequency = BCM4366.choose_freq(bw, channel);
            Run_CHP_Test(log, dut,frequency);
        }

        private void Run_CHP_Test(System.Diagnostics.TraceSource log, BCM4366 BCM4366, double frequency)
        {
            // Set set-top power level to pwrdB value
            BCM4366.BCM4366_SetPowerLevel(TxParent.pwrdB);
           
            // Initialise CHP settings  
            XAPP.Set_frequency = (frequency * 1000000).ToString();
            XAPP.Set_WlanMode(BW, mode.ToString());
            XAPP.Set_ChannelPower_Conf();
            XAPP.Set_RFB_level = ABS_Trig_Level.ToString();
            XAPP.Set_CHP_Trig();
            XAPP.Set_Power_Range();
          
            // Return CHP in dBm
            M9391A_XAPPS.CHP_Results power_result = XAPP.MeasureChannelPower(average, Aver_Num);
                     
            // Log info to TAP message window
            Log_CHP_TAP(log, power_result);

            // Create excel spreadsheet report with all values
            StoreResults_TAP(power_result);

        }

        private void Log_CHP_TAP(System.Diagnostics.TraceSource log, M9391A_XAPPS.CHP_Results power_result)
        {
            // Log info to TAP message window
            log.Info("  Running Channel Power Test ");

            log.Info("  Channel Power          : {0,6:0.00} dBm", power_result.power_result);
           
            // Compare power_result value to upper and lower limits
            if (CHPLowerLimit < power_result.power_result & power_result.power_result < CHPUpperLimit)
            {
                Verdict = Verdict.Pass;
            }
            else
            {
                Verdict = Verdict.Fail;
            }

        }

        private void StoreResults_TAP(M9391A_XAPPS.CHP_Results chp)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("Channel");
            type.DimensionTitles.Add("Start Power");
            type.DimensionTitles.Add("Measured Channel Power [dBm]");
            type.DimensionTitles.Add("Pass/Fail");

            type.Name = "CHP Results";
            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(TxParent.Channel);
            res.Doubles.Add(TxParent.pwrdB);
            res.Doubles.Add(chp.power_result);
            if (Verdict == Verdict.Pass)
            {
                res.Strings.Add("Pass");
            }
            else
            {
                res.Strings.Add("Fail");
            }
            this.Results.StoreResult(res);
        }

     

    }
}
