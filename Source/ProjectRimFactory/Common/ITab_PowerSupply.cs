﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;
using static ProjectRimFactory.AutoMachineTool.Ops;

namespace ProjectRimFactory.Common
{
    public interface IPowerSupplyMachineHolder
    {
        IPowerSupplyMachine RangePowerSupplyMachine { get; }
    }

    public interface IAdditionalPowerConsumption
    {
        Dictionary<string, int> AdditionalPowerConsumption { get; }
    }

    public interface IPowerSupplyMachine
    {
        int BasePowerConsumption { get; }
        int CurrentPowerConsumption { get; }

        //Strig will be formated for the overview and the value will hold the additional consumption
        Dictionary<string,int> AdditionalPowerConsumption { get; }

        int MinPowerForSpeed { get; }
        int MaxPowerForSpeed { get; }
        int MinPowerForRange { get; }
        int MaxPowerForRange { get; }

        float SupplyPowerForSpeed { get; set; }
        float SupplyPowerForRange { get; set; }

        float RangeInterval { get; }

        bool Glowable { get; }
        bool Glow { get; set; }
        bool SpeedSetting { get; }
        bool RangeSetting { get; }

        void RefreshPowerStatus();
    }

    class ITab_PowerSupply : ITab
    {
        private static readonly Vector2 WinSize = new Vector2(600f, 130f);

        private static readonly float HeightSpeed = 120;

        private static readonly float HeightRange = 100;

        private static readonly float HeightGlow = 30;

        public ITab_PowerSupply()
        {
            this.size = WinSize;
            this.labelKey = "PRF.AutoMachineTool.SupplyPower.TabName";

            this.descriptionForSpeed = "PRF.AutoMachineTool.SupplyPower.Description".Translate();
            this.descriptionForRange = "PRF.AutoMachineTool.SupplyPower.DescriptionForRange".Translate();
        }
        
        private string descriptionForSpeed;

        private string descriptionForRange;

        private IPowerSupplyMachine Machine => (this.SelThing as IPowerSupplyMachineHolder)?.RangePowerSupplyMachine;

        public override bool IsVisible => this.Machine != null && (this.Machine.SpeedSetting || this.Machine.RangeSetting);

        public override void TabUpdate()
        {
            base.TabUpdate();

            float additionalHeight = (this.Machine.SpeedSetting ? HeightSpeed : 0) + (this.Machine.RangeSetting ? HeightRange : 0) + (this.Machine.Glowable ? HeightGlow : 0);
            this.size = new Vector2(WinSize.x, WinSize.y + additionalHeight);
            this.UpdateSize();
        }

        public override void OnOpen()
        {
            base.OnOpen();
        }

        protected override void FillTab()
        {
            Listing_Standard list = new Listing_Standard();
            Rect inRect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);

            list.Begin(inRect);

            list.Gap();
            var rect = new Rect();

            //Add Power usage Breackdown
            rect = list.GetRect(50f);
            //TODO Use string builder
            string powerUsageBreackdown;
            powerUsageBreackdown = String.Format("Total Power Usage:\nBase [{0}] + Work Speed [{1}] + Range [{2}]", this.Machine.BasePowerConsumption, this.Machine.SupplyPowerForSpeed, this.Machine.SupplyPowerForRange);
            //Add breackdown for additional Power usage if any
            if (this.Machine.AdditionalPowerConsumption != null && this.Machine.AdditionalPowerConsumption.Count > 0)
            {
                foreach(KeyValuePair<string,int> pair in this.Machine.AdditionalPowerConsumption)
                {
                    powerUsageBreackdown += String.Format(" + {0} [{1}]",pair.Key,pair.Value);
                }
            }
            //Display the Sum
            powerUsageBreackdown += String.Format(" = [{0}]", -1 * this.Machine.CurrentPowerConsumption);
            Widgets.Label(rect, powerUsageBreackdown);

            list.Gap();
            //----------------------------

            if (this.Machine.SpeedSetting)
            {
                int minPowerSpeed = this.Machine.MinPowerForSpeed;
                int maxPowerSpeed = this.Machine.MaxPowerForSpeed;

                string valueLabelForSpeed = "PRF.AutoMachineTool.SupplyPower.ValueLabelForSpeed".Translate() + " (" + minPowerSpeed + " to " + maxPowerSpeed + ") ";

                // for speed
                rect = list.GetRect(50f);
                Widgets.Label(rect, descriptionForSpeed);
                list.Gap();

                rect = list.GetRect(50f);
                var speed = (int)Widgets.HorizontalSlider(rect, (float)this.Machine.SupplyPowerForSpeed, (float)minPowerSpeed, (float)maxPowerSpeed, true, valueLabelForSpeed, minPowerSpeed.ToString(), maxPowerSpeed.ToString(), 50);
                this.Machine.SupplyPowerForSpeed = speed;
                list.Gap();

                //Check if this.Machine.RangeSetting is active to place a Devider line
                if (this.Machine.RangeSetting)
                {
                    rect = list.GetRect(10f);
                    Widgets.DrawLineHorizontal(rect.x, rect.y, WinSize.x);
                }
            }

            if (this.Machine.RangeSetting)
            {
                int minPowerRange = this.Machine.MinPowerForRange;
                int maxPowerRange = this.Machine.MaxPowerForRange;

                string valueLabelForRange = "PRF.AutoMachineTool.SupplyPower.ValueLabelForRange".Translate() + " (" + minPowerRange + " to " + maxPowerRange + ") ";

                // for range
                rect = list.GetRect(50f);
                Widgets.Label(rect, descriptionForRange);
                list.Gap();

                rect = list.GetRect(50f);
                var range = Widgets.HorizontalSlider(rect, (float)this.Machine.SupplyPowerForRange, (float)minPowerRange, (float)maxPowerRange, true, valueLabelForRange, minPowerRange.ToString(), maxPowerRange.ToString(), this.Machine.RangeInterval);
                this.Machine.SupplyPowerForRange = range;
                list.Gap();
            }


            //TODO Maybe move this to the settings tab
            if (this.Machine.Glowable)
            {
                rect = list.GetRect(30f);
                bool glow = this.Machine.Glow;
                Widgets.CheckboxLabeled(rect, "PRF.AutoMachineTool.SupplyPower.SunLampText".Translate(), ref glow);
                this.Machine.Glow = glow;
            }
            list.Gap();

            list.End();
        }
    }
}
