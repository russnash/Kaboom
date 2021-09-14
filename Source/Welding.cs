// Based on the https://github.com/UmbraSpaceIndustries/Konstruction/tree/master/Source/Konstruction/Konstruction/Welding
// GPLV3

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using PreFlightTests;
using TestScripts;

namespace Kaboom
{

    public class Welding
    {
        Vessel vessel;
        Part part;
        public Welding(Vessel vessel, Part part)
        {
            this.vessel = vessel;
            this.part = part;
        }

        public void MergeParts(bool compress)
        {
            if (vessel.rootPart == part)
            {
                ScreenMessages.PostScreenMessage("You cannot weld the root part!");
                return;
            }

            var wData = LoadWeldingData();
            if (wData == null)
                return;

            PerformWeld(wData, compress);
        }

        private WeldingData LoadWeldingData(bool silent = false)
        {
            /**********************
             * 
             *  (root)-...-LPA==KGP==LPB
             * 
             *     LPA==LPB
             * 
             **********************/

            var wData = new WeldingData();
            wData.KaboomGluedPart = part;


            if (part.attachNodes.Count == 2)
            {
                
                wData.LinkedPartA = part.parent;

                foreach(var n in part.attachNodes)
                    if (n.attachedPart != part.parent)
                        wData.LinkedPartB = n.attachedPart;
            }

            if (wData.LinkedPartA == null || wData.LinkedPartB == null)
            {
                if (!silent)
                    ScreenMessages.PostScreenMessage("This part need to have 2 parts on attachment nodes");
                return null;
            }

            if (wData.KaboomGluedPart == vessel.rootPart)
            {
                if (!silent)
                    ScreenMessages.PostScreenMessage("This part is the root part!  Cancelling");
                return null;
            }

            return wData;
        }


        private Vector3 GetOffset(WeldingData wData)
        {
            var nodeA = WeldingNodeUtilities.GetLinkingNode(wData.LinkedPartA, wData.KaboomGluedPart);
            var nodeB = WeldingNodeUtilities.GetLinkingNode(wData.LinkedPartB, wData.KaboomGluedPart);

            Vector3 offset = nodeA.position - nodeB.position;
            Debug.Log("offset: " + offset);
            return offset;
        }

        private void PerformWeld(WeldingData wData, bool compress)
        {
            var nodeA = WeldingNodeUtilities.GetLinkingNode(wData.LinkedPartA, wData.KaboomGluedPart);
            var nodeB = WeldingNodeUtilities.GetLinkingNode(wData.LinkedPartB, wData.KaboomGluedPart);

            var offset = GetOffset(wData);


            WeldingNodeUtilities.DetachPart(wData.KaboomGluedPart);

            WeldingNodeUtilities.SwapLinks(
                wData.LinkedPartA,
                wData.KaboomGluedPart,
                wData.LinkedPartB);

            WeldingNodeUtilities.SwapLinks(
                wData.LinkedPartB,
                wData.KaboomGluedPart,
                wData.LinkedPartA);

            wData.KaboomGluedPart.SetCollisionIgnores();

            WeldingNodeUtilities.SpawnStructures(wData.LinkedPartA, nodeA);
            WeldingNodeUtilities.SpawnStructures(wData.LinkedPartB, nodeB);


            if (compress)
            {
                WeldingNodeUtilities.MovePart(wData.LinkedPartB, offset);
            }


            PartJoint newJoint = PartJoint.Create(
                wData.LinkedPartB,
                wData.LinkedPartA,
                nodeB,
                nodeA,
                AttachModes.STACK);

            wData.LinkedPartB.attachJoint = newJoint;

            SoftExplode(wData.KaboomGluedPart);
        }

        private static void SoftExplode(Part thisPart)
        {
            thisPart.explosionPotential = 0.1f;
            thisPart.explode();
        }
    }
}
