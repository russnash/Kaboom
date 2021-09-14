using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace Kaboom
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleKaboom : PartModule
    {
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kaboom delay",
            groupDisplayName = "<color=red><b>Switch Safety Cover</b></color>", groupName = "Kaboom", groupStartCollapsed = true,
            guiUnits = "Seconds"),
            UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]

        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool timerActive = false;

        [KSPField(isPersistant = true)]
        public double kaboomTime;

        [KSPField(isPersistant = true)]
        public bool isGlued = false;

        [KSPField(guiName = "Superglue", guiActive = true, guiActiveEditor = true, groupName = "Kaboom")]
        public string gluedText = Localizer.Format("#autoLOC_6001071"); /*Disabled*/


        [KSPEvent(guiName = "Enable Superglue", guiActive = true, guiActiveEditor = true, groupName = "Kaboom", active = true)]
        public void GluedEvent()
        {
            isGlued = !isGlued;
            if (isGlued)
            {
                gluedText = Events["GluedEvent"].guiName = Localizer.Format("#autoLOC_6001072")/*Enabled*/;
                Events["GluedEvent"].guiName = "Disable Superglue";
            }
            else
            {
                gluedText = Events["GluedEvent"].guiName = Localizer.Format("#autoLOC_6001071")/*Disabled*/;
                Events["GluedEvent"].guiName = "Enable Superglue";
            }
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiName = "Kaboom!", groupName = "Kaboom", active = true)]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiName = "Cancel Kaboom!", groupName = "Kaboom", active = false)]
        public void CancelKaboomEvent()
        {
            CancelKaboomIt();
        }

        [KSPAction("Kaboom!")]
        public void KaboomAction(KSPActionParam _) => KaboomIt();

        private void KaboomIt()
        {
            Events["CancelKaboomEvent"].active = true;
            Events["KaboomEvent"].active = false;
            part.force_activate();

            if (delay == 0)
            {
                Proceed();
            }
            else
            {
                ScreenMessages.PostScreenMessage("Kaboom set for " + delay + " seconds.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                kaboomTime = Planetarium.GetUniversalTime() + delay;
                timerActive = true;
            }
        }

        private void Proceed()
        {
            if (isGlued)
            {
                var k = new Welding(vessel, part);
                k.MergeParts(true);
            }
            else
            {
                part.explode();
            }
        }

        private void CancelKaboomIt()
        {
            Events["CancelKaboomEvent"].active = false;
            Events["KaboomEvent"].active = true;
            ScreenMessages.PostScreenMessage("Kaboom cancelled.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            timerActive = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (timerActive)
            {
                if (Planetarium.GetUniversalTime() >= kaboomTime)
                {
                    timerActive = false;
                    Proceed();
                }
            }
            //base.OnUpdate();
        }
    }
}
