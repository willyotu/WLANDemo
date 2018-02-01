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
using System.Diagnostics;
using Tap;
using System.Reflection;
using Agilent.AgMWSwitch.Interop;


namespace TapPlugin.PaceUK
{
    // Creates Display Name and Description for GUI
    [DisplayName("Arris\\BCM4366 Transmitter Tests")]
    [Description("Broadcom 4366 Chipset Wlan Transmitter Tests")]
    
    public class BCM4366_Transmitter_Tests : TestStep
    {
        #region Enums for GUI Drop down menus
        // Creates variables and GUI drop down for Rate_Setting
        public enum Rate_Setting
        {
            r,
            h,
            v,
            c,
            s
        };
                   
        #endregion

        #region Settings
        // ToDo: Add property here for each parameter the end user should be able to change from the TAP GUI
        [DisplayName("XAPP ANALYZER")]
        public M9391A_XAPPS XAPP { get; set; }
                
        [DisplayName("Configured Chipset")]
        public BCM4366 BCM4366 { get; set; }

        [DisplayName("Enable Switch")]
        public bool Enable_Switch { get; set; }

        [Browsable(false)]
        public bool Enable_Measurements { get; set; }
               
        [DisplayName("VSA\\Cable Loss [dB]")]
        [Description(" Enter cable loss value ")]
        public double loss { get; set; }

        [DisplayName("VSA\\Average State")]
        public bool average { get; set; }

        [DisplayName("VSA\\Average Number")]
        public int Aver_Num { get; set; }

        [DisplayName("VSA\\Trigger Level [dBm]")]
        [Description("Sets absolute trigger level ")]
        public double ABS_Trig_Level { get; set; }

        [DisplayName("SetTop\\ISM Band")]
        [Description("Choose 2g or 5g depnding on wifi channel ")]
        public string ISM_Band { get; set; }

        [DisplayName("SetTop\\\t\t\tChannel")]
        public int Channel { get; set; }

        [DisplayName("SetTop\\\t\tRate Setting")]
        [Description("Choose rate setting for wlan mode ")]
        public Rate_Setting rate { get; set; }

        [DisplayName("SetTop\\\tOFDM Rate(MCS Index)")]
        [Description("Choose rate for a,b,or MCS index for n,ac wlan mode ")]
        public double OFDM_rate { get; set; }

        [DisplayName("SetTop\\Bandwidth [MHz]")]
        public int BW { get; set; }

        [DisplayName("SetTop\\Antenna TX Chain")]
        public int antenna { get; set; }

        [DisplayName("SetTop\\Chipset WLAN mode")]
        [Description("Set WLAN mode for DUT")]
        public string mode { get; set; }

        [DisplayName("SetTop\\Start Power")]
        public double pwrdB { get; set; }

        Switch switching = null;

        BCM4366TxChP IP_Setter = null;
        #endregion

        #region Initialise variables used in TAP GUI
        public BCM4366_Transmitter_Tests()
        {
           // Enables switch for measurements
            Enable_Switch = true;

            // Sets averaging state for measurements
            average = false;

            // Sets number of averages for measurements
            Aver_Num = 10;

            // Sets cable correction for measurements
            loss = -1.4;

            // Sets ISM Band GUI Text Box to 5g
            ISM_Band = "5g";

            // Sets GUI Text Box to Channel to 36 Used by WlanChannels to determine frequency if Set Top Box and Analyser
            Channel = 36;

            // Sets  the ODFM rate if a, b mode or MCS Index if n, ac mode on the Set Top Box
            OFDM_rate = 7;

            // Sets Rate GUI Drop Down to r which determines the modulation scheme set on Set Top Box
            rate = Rate_Setting.h;

            // Sets Bandwidth Text Box to 20 which determines the bandwidth on Set Top Box
            BW = 20;

            // Sets Antenna Tx Chain Text Box to 1 which determines antenna used (1 2 or 3) on Set Top Box
            antenna = 1;

            // Sets BCM4366 Mode GUI Drop Down to r which determines the modulation scheme set on Set Top Box
            mode = "n";

            // Sets Trig Level Text Box to -25 which determines trigger level on analyser
            ABS_Trig_Level = -26;

            // Sets Start Power Text Box to 30 which determines the Start Power of the Set top Box
            pwrdB = 30;

               
            //this.Rules.Add(new ValidationRule(() => mode == BCM4366_Mode.a && Mode == WLAN_Standard.BG, "Mode has to match VSA mode", "mode", "Mode"));
            this.Rules.Add(new ValidationRule(() => BW < 160, "Bandwidth too large.", "BW"));
           
        #region Add default child TestSteps
              ChildTestSteps.Add(new BCM4366TxChP());
              ChildTestSteps.Add(new BCM4366TxEvm());
              ChildTestSteps.Add(new BCM4366TxSem());
              ChildTestSteps.Add(new BCM4366TxSpectralFlatness());
              ChildTestSteps.Add(new BCM4366TxEvmCeSctCfl());
              ChildTestSteps.Add(new BCM4366TxPowerRamp());
         #endregion

        }

        #endregion

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            XAPP.M9OXA_Startup();
            XAPP.Set_Mode();
        }

        public override void Run()
        {
            // ToDo: Add test case code here
            #region Initialise DUT & Set up VSA for required measurement
            System.Diagnostics.TraceSource log = Log.CreateSource("Chipset Initialization");
            log.Debug("Sending wl commands for BCM4366 transmitter");
            Enable_Switching();
            // Send wl commands to intialise the Set-top Box
            BCM4366.init4366tx(mode, antenna, ISM_Band, rate, OFDM_rate, BW, Channel, pwrdB);
            //Preset WLAN application
            XAPP.System_Preset(); 
            //Apply cable correction
            XAPP.Correction(loss);
         
            
            RunChildSteps(); 
          
            #endregion
        }
           
        public override void PostPlanRun()
        {
            base.PostPlanRun();
            // ToDo: Optionally add any cleanup code this steps needs to run after the entire testplan has finished
            BCM4366.BCM4366_ShutDown();
        }

        public void Enable_Switching()
        {
            Agilent.AgMWSwitch.Interop.IAgMWSwitch M9155C = new Agilent.AgMWSwitch.Interop.AgMWSwitchClass();
            Agilent.AgMWSwitch.Interop.IAgMWSwitch M9157C = new Agilent.AgMWSwitch.Interop.AgMWSwitchClass();
           
            M9155C.Initialize("PXI20::15::0::INSTR", false, false, "");
            M9157C.Initialize("PXI20::14::0::INSTR", false, false, "");

            if (Enable_Switch == true & antenna ==1)
            {
                M9155C.Route.CloseChannel("b1ch1");
                M9157C.Route.CloseChannel("b1ch4");
                M9155C.Close();
                M9157C.Close();
            }
            else if (Enable_Switch == true & antenna == 2)
            {
                M9155C.Route.CloseChannel("b1ch1");
                M9157C.Route.CloseChannel("b1ch5");
                M9155C.Close();
                M9157C.Close();
            }
            else if (Enable_Switch == true & antenna == 3)
            {
                M9155C.Route.CloseChannel("b1ch1");
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
