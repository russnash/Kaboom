// Based on the https://github.com/UmbraSpaceIndustries/Konstruction/tree/master/Source/Konstruction/Konstruction/Welding
// GPLV3

using System;
using UnityEngine;
using System.Linq;
using KSP.Localization;

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

        public bool MergeParts(bool compress)
        {
            var wData = LoadWeldingData();
            if (wData == null)
                return false;

            bool sucess = PerformWeld(wData, compress);
            return sucess;
        }



        private WeldingData LoadWeldingData()
        {
            /**********************
             * 
             *  (root)-...-LPA==KGP==LPB
             * 
             *     LPA==LPB
             * 
             **********************/

            if (vessel.rootPart == part)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#BOOM-MergePartsMsg"));
                return null;
            }

            var wData = new WeldingData();


            int attachedPartsCount = 0;
            foreach (var n in part.attachNodes)
                if (n.attachedPart != null)
                {
                    //Debug.Log("n.attachedPart: " + n.attachedPart.partInfo.title);
                    attachedPartsCount++;
                }

            //Debug.Log("part.parent: " + part.parent.partInfo.title);
            //Debug.Log("attachedPartsCount: " + attachedPartsCount + " part.children.Count: " + part.children.Count);

            if (attachedPartsCount == 2 && part.children.Count == 1)
            {
                wData.KaboomGluedPart = part;
                wData.LinkedPartA = part.parent;
                wData.LinkedPartB = part.children[0];
            }
            else if (WeldingUtilities.IsWeldablePort(part))
            {
                if (WeldingUtilities.IsWeldablePort(part.parent))
                {
                    //Debug.Log("Both Ports");
                    wData.DockingPortA = part.parent;
                    wData.DockingPortB = part;
                }
                else if (part.children != null && part.children.Count > 0)
                {
                    //Debug.Log("Both Ports Otherwise");
                    foreach (var p in part.children.Where(WeldingUtilities.IsWeldablePort))
                    {
                        wData.DockingPortA = part;
                        wData.DockingPortB = p;
                        break;
                    }
                }

                if (wData.DockingPortA != null && wData.DockingPortB != null)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#BOOM-WeldingData-A", 5));
                    wData.LinkedPartA = WeldingDockingPorts.FindAttachedPart(wData.DockingPortA, wData.DockingPortB);
                    wData.LinkedPartB = WeldingDockingPorts.FindAttachedPart(wData.DockingPortB, wData.DockingPortA);
                }
            }

            if (wData.LinkedPartA == null || wData.LinkedPartB == null)
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#BOOM-WeldingData-B"));
                return null;
            }

            return wData;
        }

        private Vector3 GetOffset(WeldingData wData)
        {
            //Vector3 offset1 =
            //nodeA.owner.transform.rotation * nodeA.position + nodeA.owner.transform.position -
            //nodeB.owner.transform.rotation * nodeB.position + nodeB.owner.transform.position;
            //Vector3 offset2 = wData.LinkedPartA.transform.localPosition - wData.LinkedPartB.transform.localPosition;
            //offset2.Normalize();
            //offset2 *= WeldingNodeUtilities.GetPartThickness(wData.KaboomGluedPart);

            Vector3 offset;

            if (wData.KaboomGluedPart)
            {
                AttachNode a = WeldingUtilities.GetLinkingNode(wData.KaboomGluedPart, wData.LinkedPartA);
                AttachNode b = WeldingUtilities.GetLinkingNode(wData.KaboomGluedPart, wData.LinkedPartB);
                offset = part.transform.rotation * (a.position - b.position);
            }
            else
            {
                offset = WeldingDockingPorts.GetOffset(wData);
            }

            return offset;
        }

        private bool PerformWeld(WeldingData wData, bool compress)
        {
            if (wData.KaboomGluedPart)
            {
                var nodeA = WeldingUtilities.GetLinkingNode(wData.LinkedPartA, wData.KaboomGluedPart);
                var nodeB = WeldingUtilities.GetLinkingNode(wData.LinkedPartB, wData.KaboomGluedPart);

                var offset = GetOffset(wData);

                WeldingUtilities.DetachPart(wData.KaboomGluedPart);

                WeldingUtilities.SwapLinks(
                    wData.LinkedPartA,
                    wData.KaboomGluedPart,
                    wData.LinkedPartB);

                WeldingUtilities.SwapLinks(
                    wData.LinkedPartB,
                    wData.KaboomGluedPart,
                    wData.LinkedPartA);

                wData.KaboomGluedPart.SetCollisionIgnores();

                WeldingUtilities.SpawnStructures(wData.LinkedPartA, nodeA);
                WeldingUtilities.SpawnStructures(wData.LinkedPartB, nodeB);

                if (compress)
                {
                    WeldingUtilities.MovePart(wData.LinkedPartB, offset);
                }

                PartJoint newJoint = PartJoint.Create(
                    wData.LinkedPartB,
                    wData.LinkedPartA,
                    nodeB,
                    nodeA,
                    AttachModes.STACK);

                wData.LinkedPartB.attachJoint = newJoint;

                WeldingUtilities.Explode(wData.KaboomGluedPart);
            }
            else
                WeldingDockingPorts.PerformWeld(wData, compress);

            return true;
        }
    }
}
