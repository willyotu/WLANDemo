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
    [DisplayName("PaceUK\\BCM4366 Rx Minimum Input Sensitivity")]
    [Description("Insert a description here")]
    public class BCM4366RxMinimumInputSensitivity : BCM4366RxBase
    {
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change
        [DisplayName("VSG\\Amplitude Upper Limit[dBm]")]
        public Double AmptdULimit { get; set; }

        [DisplayName("VSG\\Amplitude Lower Limit[dBm]")]
        public Double AmptdLLimit { get; set; }

        #endregion

        public BCM4366RxMinimumInputSensitivity()
        {
            // ToDo: Set default values for properties / settings.
            AmptdULimit = -15;
            AmptdLLimit = -85;
        }

        public override void Run()
        {
            // Uses Channel Variable from TAP GUI to and returns frequency variable to set frequency of Set Top Box and Analyser 
            int channel = RxParent.channel;
            double frequency = WLanChannels.ChannelToFrequencyMHz(channel);
            Generator.Set_Amplitude(RxParent.Amptd);

            log.Info("  Running Minimum Input Sensitivity Test ");
            log.Info("  Initial Power level : {0,0:0.00} dBm", RxParent.Amptd);
            TestPlan.Sleep(200);
            Tuple<int, int> before = BCM4366.RxPackagesReceived();  //Get number of received packages (as a reference)
            TestPlan.Sleep(500);
            Generator.Play_Waveform(frequency, RxParent.Amptd, RxParent.ArbName,RxParent.rms, RxParent.repeat);
            TestPlan.Sleep(1000);
            Tuple<int, int> after = BCM4366.RxPackagesReceived(); //Get number of received packages after waveform is completed

            double ack = after.Item1 - before.Item1;

            double perError = (1 - (double)ack / (int)RxParent.frames) * 100;

            
            // Check if PER is within limit. If yes, decrease the Signal Generator power. If no, increase the Signal Generator power. 
            if (perError < RxParent.PER_limit)
            {
                double pwr = RxParent.Amptd;
                while (perError < RxParent.PER_limit & pwr > AmptdLLimit)
                {
                    pwr = pwr - 1;
                    PER_Test(ref before, ref after, ref ack, ref perError, pwr,frequency);

                    if (perError >= RxParent.PER_limit)
                    {
                        log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        Verdict = Verdict.Pass;
                        StoreResults_TAP(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                        log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                        log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                        StoreResults_TAP(perError, pwr, ack);
                }
            }
            else
            {
                double pwr = RxParent.Amptd;
                while (perError > RxParent.PER_limit & pwr < AmptdULimit)
                {
                    pwr = pwr + 1;
                    PER_Test(ref before, ref after, ref ack, ref perError, pwr,frequency);

                    if (perError <= RxParent.PER_limit)
                    {
                        log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        log.Info("Minimum Input Sensitivty is: : {0,0:0.00} dBm", pwr);
                        log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                        Verdict = Verdict.Pass;
                        StoreResults_TAP(perError, pwr, ack);
                        break;
                    }
                    else
                    {
                        log.Info("  Error Rate          : {0,6:0.00} % ", perError);
                        log.Info("  Power Level         : {0,0:0.00} dBm", pwr);
                        log.Info("  No. of Packets Received         : {0,0:0.00} ", ack);
                    }
                    StoreResults_TAP(perError, pwr, ack);
                }
            }
         
           

        }

        private void PER_Test(ref Tuple<int, int> before, ref Tuple<int, int> after, ref double ack, ref double perError, double pwr, double frequency)
        {
            Generator.Set_Amplitude(pwr);
            TestPlan.Sleep(200);
            before = BCM4366.RxPackagesReceived();  //Get number of received packages (as a reference)
            TestPlan.Sleep(500);
            Generator.Play_Waveform(frequency, RxParent.Amptd, RxParent.ArbName, RxParent.rms, RxParent.repeat);
            TestPlan.Sleep(1000);
            after = BCM4366.RxPackagesReceived(); //Get number of received packages after waveform is completed

            ack = after.Item1 - before.Item1;

            perError = (1 - (double)ack / (int)RxParent.frames) * 100;
        }

        private void StoreResults_TAP(double perError, double pwr, double ack)
        {
            TestStepResultType type = new TestStepResultType();
            type.DimensionTitles.Add("PER");
            type.DimensionTitles.Add("Minimum Input Sensitivity Level");
            type.DimensionTitles.Add("No. of Packets Sent");
            type.DimensionTitles.Add("No. of Packets Received");
            type.DimensionTitles.Add("Pass/Fail");
            type.Name = "PER Results";
            TestStepResult res = new TestStepResult();
            res.Type = type;
            res.Doubles.Add(perError);
            res.Doubles.Add(pwr);
            res.Doubles.Add(RxParent.frames);
            res.Doubles.Add(ack);
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
