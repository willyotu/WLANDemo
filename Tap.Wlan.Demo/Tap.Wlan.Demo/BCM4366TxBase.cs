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
    [AllowAsChildIn(typeof(BCM4366_Transmitter_Tests))] // this line makes sure that this step can only be inserted as a child to "Transmitter Tests"
    public abstract class BCM4366TxBase : TestStep
    {
        #region Accessors to the parent steps settings
        protected BCM4366_Transmitter_Tests TxParent { get { return this.Parent as BCM4366_Transmitter_Tests; } }
        protected M9391A_XAPPS XAPP { get { return TxParent.XAPP; } }
        protected double pwrdB { get { return TxParent.pwrdB; } }
        protected double ABS_Trig_Level { get { return TxParent.ABS_Trig_Level; } }
        protected int BW { get { return TxParent.BW; } }
        protected string mode { get { return TxParent.mode; } }
        protected bool average { get { return TxParent.average; } }
        protected int Aver_Num { get { return TxParent.Aver_Num; } }
        #endregion

        public BCM4366TxBase()
        {
            // ToDo: Set default values for properties / settings.
        }
    }
}
