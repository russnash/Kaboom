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
            guiName = "#BOOM-delay",  groupName = "KaboOm", groupStartCollapsed = true, guiUnits = " " + "#BOOM-delay-Units",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 10f, guiActiveEditor = true),
            UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]
        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool isGlued = false;

        [KSPEvent(groupName = "KaboOm",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 10f, guiActiveEditor = true,
            active = true, guiActiveUncommand = true)]
        public void GluedEvent()
        {
            isGlued = !isGlued;
            GUITextUpdate();
        }

        private void GUITextUpdate()
        {
            if (isGlued)
                Events["GluedEvent"].guiName = Localizer.Format("#BOOM-GluedEvent") + ": " + Localizer.Format("#autoLOC_6001072")/*Enabled*/;
            else
                Events["GluedEvent"].guiName = Localizer.Format("#BOOM-GluedEvent") + ": " + Localizer.Format("#autoLOC_6001071")/*Disabled*/;
        }

        [KSPEvent(guiName = "#BOOM-KaboomEvent", groupName = "KaboOm",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 10f,
            active = true, guiActiveUncommand = true)]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPEvent(guiName = "#BOOM-Cancel", groupName = "KaboOm",
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 10f,
            active = false, guiActiveUncommand = true)]
        public void CancelKaboomEvent()
        {
            CancelKaboomIt();
        }

        [KSPAction("#BOOM-KaboomAction")]
        public void KaboomAction(KSPActionParam _) => KaboomIt();

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.CurrentGame.Parameters.CustomParams<KaboomSettings>().coloredPAW)
                Fields["delay"].group.displayName = System.String.Format("<color=red>" + Localizer.Format("Kaboom Safety Cover") + "</color>");
            else
                Fields["delay"].group.displayName = Localizer.Format("#BOOM-SafetyCover");

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
                ScreenMessages.PostScreenMessage(Localizer.Format("#BOOM-KaboomItMsg", delay), 5.0f, ScreenMessageStyle.UPPER_CENTER);
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
            ScreenMessages.PostScreenMessage(Localizer.Format("#BOOM-CancelKoboomIt"));
        }
    }
}
