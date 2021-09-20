using KSP.Localization;
using UnityEngine;

namespace Kaboom
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleKaboom : PartModule
    {
        [KSPField(isPersistant = true,
            guiName = "Kaboom delay", groupName = "Kaboom", groupStartCollapsed = true, guiUnits = " Seconds",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiActiveEditor = true),
            UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]
        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool isGlued = false;

        [KSPEvent(groupName = "Kaboom",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiActiveEditor = true,
            active = true, guiActiveUncommand = true)]
        public void GluedEvent()
        {
            isGlued = !isGlued;
            GUITextUpdate();
        }

        private void GUITextUpdate()
        {
            if (isGlued)
                Events["GluedEvent"].guiName = "Superglue: " + Localizer.Format("#autoLOC_6001072")/*Enabled*/;
            else
                Events["GluedEvent"].guiName = "Superglue: " + Localizer.Format("#autoLOC_6001071")/*Disabled*/;
        }

        [KSPEvent(guiName = "Kaboom!", groupName = "Kaboom",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f,
            active = true, guiActiveUncommand = true)]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPEvent(guiName = "Cancel Kaboom!", groupName = "Kaboom",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, 
            active = false, guiActiveUncommand = true)]
        public void CancelKaboomEvent()
        {
            CancelKaboomIt();
        }

        [KSPAction("Kaboom!")]
        public void KaboomAction(KSPActionParam _) => KaboomIt();

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.CurrentGame.Parameters.CustomParams<KaboomSettings>().coloredPAW)
                Fields["delay"].group.displayName = "<color=red>Kaboom Safety Cover</color>";
            else
                Fields["delay"].group.displayName = "Kaboom Safety Cover";

            GUITextUpdate();
        }

        private void KaboomIt()
        {
            Events["CancelKaboomEvent"].active = true;
            Events["KaboomEvent"].active = false;

            if (delay == 0)
            {
                Proceed();
            }
            else
            {
                float delay_scaled = delay;

                if (TimeWarp.WarpMode == TimeWarp.Modes.HIGH)
                    delay_scaled /= TimeWarp.CurrentRate;

                Invoke("Proceed", delay_scaled);
                ScreenMessages.PostScreenMessage("Kaboom set for " + delay + " seconds.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        private void Proceed()
        {
            bool success;
            if (isGlued)
            {
                var k = new Welding(vessel, part);
                success = k.MergeParts(true);
            }
            else
            {
                part.force_activate();
                WeldingUtilities.Explode(part);
                success = true;
            }

            if (!success)
                CancelKaboomIt();
        }

        private void CancelKaboomIt()
        {
            Events["CancelKaboomEvent"].active = false;
            Events["KaboomEvent"].active = true;
            ScreenMessages.PostScreenMessage("Kaboom canceled.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
