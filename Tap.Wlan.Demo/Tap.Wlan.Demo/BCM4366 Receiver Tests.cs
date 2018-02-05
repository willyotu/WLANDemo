// Responsible: TEAM (wilattoh)
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
    [DisplayName("Arris\\BCM4366 Receiver Tests")]
    [Description("Broadcom 4366 Chipset Wlan Transmitter Tests")]

    public class BCM4366_Receiver_Tests : TestStep
    {
        public enum Rate_Setting
        {
            r,
            h,
            v,
            c,
            s
        };
       
        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change

        [DisplayName("Configured Chipset")]
        [Description("Select DUT for test")]
        public BCM4366 BCM4366 { get; set; }

        [DisplayName("Generator")]
        [Description("Choose VSG for tests")]
        public M9381A_VSG Generator { get; set; }

        [DisplayName("Enable Switch")]
        public bool Enable_Switch { get; set; }

        [DisplayName("SetTop\\\t\t\t\tChannel")]
        public int channel { get; set; }

        [DisplayName("SetTop\\\t\t\tAntenna")]
        public int antenna { get; set; }

        [DisplayName("SetTop\\\t\tRate Setting")]
        [Description("Choose rate setting for wlan mode ")]
        public Rate_Setting rate { get; set; }

        [DisplayName("SetTop\\\tMCS Index")]
        public double OFDM_rate { get; set; }

        [DisplayName("SetTop\\Chipset WLAN mode")]
        [Description("Set WLAN mode for DUT")]
        public string mode { get; set; }

        [DisplayName("SetTop\\ISM Band")]
        [Description("Choose 2g or 5g depnding on wifi channel ")]
        public string ISM_Band { get; set; }

        [DisplayName("SetTop\\Start Power")]
        public Double pwrdB { get; set; }
        
        [DisplayName("SetTop\\Bandwidth [MHz]")]
        public Double BW { get; set; }

        [DisplayName("VSG\\Amplitude[dBm]")]
        public Double Amptd { get; set; }

        [DisplayName("VSG\\RMS Power")]
        public Double rms { get; set; }

        [DisplayName("VSG\\Repetition")]
        public int repeat { get; set; }

        [DisplayName("VSG\\Number of Frames")]
        public int frames { get; set; }

        [FilePath]
        [DisplayName("VSG\\Waveform")]
        public string ArbName { get; set; }

        [DisplayName(" Limits \\ Max PER [%]")]
        [Description("If measured PER is greater than the specified number the teststep will fail.")]
        public int PER_limit { get; set; }

        #endregion
        public BCM4366_Receiver_Tests()
        {
            // ToDo: Set default values for properties / settings.
            // Enables switch for measurements
            Enable_Switch = false;
            TraceBar limitBar = new TraceBar { LowerLimit = 0, UpperLimit = PER_limit, ShowResult = false };
            rate = Rate_Setting.h;
            OFDM_rate = 0;
            mode = "n";
            ISM_Band = "5g";
            pwrdB = 60;
            channel = 36;
            Amptd = -20;
            ArbName = "C:\\802.11n_Pace_100frames_MCS0.wfm";
            PER_limit = 10;
            rms = 12.0;
            repeat = 1;
            frames = 100;
            BW = 20;
            antenna = 1;
            

            // Add default child TestSteps

            ChildTestSteps.Add(new BCM4366RxMinimumInputSensitivity());
           // ChildTestSteps.Add(new BeInRxMinimumInputSensitivityWithTransmitFrequencyOffset { Enabled = false });
        }
        public override void PrePlanRun()
        {
            base.PrePlanRun();
            Generator.M9381Server_Startup();

            // ToDo: Optionally add any setup code this steps needs to run before the testplan starts
        }
        public override void Run()
        {
            System.Diagnostics.TraceSource log = Log.CreateSource("Chipset Initialization");
            log.Debug("Sending wl commands for BCM4366 Receiver ");
            Enable_Switching();
            BCM4366.init4366rx(mode, BW, antenna, ISM_Band, rate, OFDM_rate, pwrdB, channel);
            Generator.InitializeGenerator();
            RunChildSteps(); //If step has child steps.
            
        }
        public void Enable_Switching()
        {
            Agilent.AgMWSwitch.Interop.IAgMWSwitch M9155C = new Agilent.AgMWSwitch.Interop.AgMWSwitchClass();
            Agilent.AgMWSwitch.Interop.IAgMWSwitch M9157C = new Agilent.AgMWSwitch.Interop.AgMWSwitchClass();

            M9155C.Initialize("PXI20::15::0::INSTR", false, false, "");
            M9157C.Initialize("PXI20::14::0::INSTR", false, false, "");
            if (Enable_Switch == true & antenna == 1)
            {
                M9155C.Route.CloseChannel("b1ch2");
                M9157C.Route.CloseChannel("b1ch4");
                M9155C.Close();
                M9157C.Close();
            }
            else if (Enable_Switch == true & antenna == 2)
            {
                M9155C.Route.CloseChannel("b1ch2");
                M9157C.Route.CloseChannel("b1ch5");
                M9155C.Close();
                M9157C.Close();
            }
            else if (Enable_Switch == true & antenna == 3)
            {
                M9155C.Route.CloseChannel("b1ch2");
                M9157C.Route.CloseChannel("b1ch6");
                M9155C.Close();
                M9157C.Close();
            }
            else if (Enable_Switch == false)
            {
                Enable_Switch = false;

            }
        }
             
    }
}
