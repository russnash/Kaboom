// Based on the https://github.com/UmbraSpaceIndustries/Konstruction/tree/master/Source/Konstruction/Konstruction/Welding
// GPLV3

using System;
using UnityEngine;

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
            if (vessel.rootPart == part)
            {
                ScreenMessages.PostScreenMessage("You cannot weld the root part!");
                return false;
            }

            var wData = LoadWeldingData();
            if (wData == null)
                return false;

            bool sucess =  PerformWeld(wData, compress);
            return sucess;
        }

        public WeldingData LoadWeldingData()
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

            int attachedPartsCount = 0;
            foreach (var n in part.attachNodes)
                if (n.attachedPart != null)
                    attachedPartsCount++;

            //Debug.Log("attachedPartsCount: " + attachedPartsCount + " part.children.Count: " + part.children.Count);

            if (attachedPartsCount == 2 && part.children.Count == 1)
            {
                wData.LinkedPartA = part.parent;
                wData.LinkedPartB = part.children[0];
            }

            if (wData.LinkedPartA == null || wData.LinkedPartB == null)
            {
                ScreenMessages.PostScreenMessage("This part need to have 2 parts on attachment nodes");
                return null;
            }

            if (wData.KaboomGluedPart == vessel.rootPart)
            {
                ScreenMessages.PostScreenMessage("This part is the root part!  Cancelling");
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

            var nodeA = WeldingUtilities.GetLinkingNode(wData.KaboomGluedPart, wData.LinkedPartA);
            var nodeB = WeldingUtilities.GetLinkingNode(wData.KaboomGluedPart, wData.LinkedPartB);

            Vector3 offset = part.transform.rotation * (nodeA.position - nodeB.position);   
            
            return offset;
        }

        

        private bool PerformWeld(WeldingData wData, bool compress)
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

            return true;
        }
    }
}
