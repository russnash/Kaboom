using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine;

namespace Kaboom
{
    public class Kaboom : PartModule
    {
        Timer kaboomTimer;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kaboom delay", guiUnits = "Seconds"), UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]
        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool timerActive = false;

        [KSPField(isPersistant = true)]
        public double kaboomTime;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Kaboom on staging")]
        public bool activeInStaging = false;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            if (this.part.stagingIcon == "")
            {
                Events["ToggleKaboomStaging"].active = true;
            }
            else
            {
                Events["ToggleKaboomStaging"].active = false;
            }

            if (activeInStaging)
            {
                part.stackIcon.SetIconColor(XKCDColors.Orange);
                part.ActivatesEvenIfDisconnected = true;
                Staging.SortIcons();
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, guiActiveEditor = true, unfocusedRange = 2f, guiName = "Toggle Kaboom on staging")]
        public void ToggleKaboomStaging()
        {
            activeInStaging = !activeInStaging;

            if (activeInStaging)
            {
                part.deactivate();
                part.stackIcon.CreateIcon();
                part.stackIcon.SetIconColor(XKCDColors.Orange);
                part.ActivatesEvenIfDisconnected = true;
                ScreenMessages.PostScreenMessage("Added Kaboom action to staging", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            else
            {
                part.stackIcon.RemoveIcon();
                ScreenMessages.PostScreenMessage("Removed Kaboom action from staging", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            Staging.SortIcons();
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiName = "Kaboom part", active = true)]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiName = "Cancel Kaboom", active = false)]
        public void CancelKaboomEvent()
        {
            CancelKaboomIt();
        }

        [KSPAction("Kaboom!")]
        public void KaboomAction(KSPActionParam param)
        {
            KaboomIt();
        }

        private void KaboomIt()
        {
            Events["CancelKaboomEvent"].active = true;
            Events["KaboomEvent"].active = false;

            if (delay == 0)
            {
                part.explode();
            }
            else
            {
                ScreenMessages.PostScreenMessage("Kaboom set for " + delay + " seconds.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                kaboomTime = Planetarium.GetUniversalTime() + delay;
                timerActive = true;
                kaboomTimer = new Timer(1000);
                kaboomTimer.Elapsed += kaboomTimer_Elapsed;
                kaboomTimer.Enabled = true;
            }
        }

        void kaboomTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScreenMessages.PostScreenMessage("Tick...");
            if (timerActive)
            {
                if (Planetarium.GetUniversalTime() >= kaboomTime)
                {
                    timerActive = false;
                    part.explode();
                }
            }
            else
            {
                kaboomTimer.Enabled = false;
            }
        }

        private void CancelKaboomIt()
        {
            Events["CancelKaboomEvent"].active = false;
            Events["KaboomEvent"].active = true;
            ScreenMessages.PostScreenMessage("Kaboom cancelled.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            timerActive = false;
        }
    }
}
