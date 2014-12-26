using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kaboom
{
    public class Kaboom : PartModule
    {
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kaboom delay", guiUnits = "Seconds"), UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]
        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool timerActive = false;

        [KSPField(isPersistant = true)]
        public double kaboomTime;

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 2f, guiName = "Kaboom!")]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPAction("Kaboom!")]
        public void KaboomAction(KSPActionParam param)
        {
            KaboomIt();
        }

        private void KaboomIt()
        {
            part.force_activate();

            if (delay == 0)
            {
                part.explode();
            }
            else
            {
                kaboomTime = Planetarium.GetUniversalTime() + delay;
                timerActive = true;
            }
        }

        public override void OnUpdate()
        {
            base.OnFixedUpdate();
            if (timerActive)
            {
                if (Planetarium.GetUniversalTime() >= kaboomTime)
                {
                    timerActive = false;
                    part.explode();
                }
            }
        }
    }
}
