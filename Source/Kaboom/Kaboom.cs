using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kaboom
{
    public class Kaboom : PartModule
    {
        [KSPEvent(guiActive = true, guiName = "Kaboom!")]
        public void KaboomPart()
        {
            part.explode();
        }
    }
}
