using System;
using UnityEngine;


namespace Kaboom
{
    public static class WeldingDockingPorts
    {
        public static Vector3 GetOffset(WeldingData wData)
        {
            var totalThickness = 0f;
            var objA = new GameObject();
            var objB = new GameObject();

            var transformA = objA.transform;
            var transformB = objB.transform;

            transformA.localPosition = wData.DockingPortA.transform.localPosition;
            transformB.localPosition = wData.DockingPortB.transform.localPosition;

            var offset =
                transformA.localPosition - transformB.localPosition;

            offset.Normalize();

            totalThickness += WeldingUtilities.GetPartThickness(wData.DockingPortA);
            totalThickness += WeldingUtilities.GetPartThickness(wData.DockingPortB);

            offset *= totalThickness;

            return offset;
        }


        public static void PerformWeld(WeldingData wData, bool compress)
        {
            var nodeA = WeldingUtilities.GetLinkingNode(wData.LinkedPartA, wData.DockingPortA);
            var nodeB = WeldingUtilities.GetLinkingNode(wData.LinkedPartB, wData.DockingPortB);

            var offset = GetOffset(wData);

            WeldingUtilities.DetachPart(wData.DockingPortA);
            WeldingUtilities.DetachPart(wData.DockingPortB);

            WeldingUtilities.SwapLinks(
                wData.LinkedPartA,
                wData.DockingPortA,
                wData.LinkedPartB);
            WeldingUtilities.SwapLinks(
                wData.LinkedPartB,
                wData.DockingPortB,
                wData.LinkedPartA);

            wData.DockingPortB.SetCollisionIgnores();
            wData.DockingPortA.SetCollisionIgnores();

            WeldingUtilities.SpawnStructures(wData.LinkedPartA, nodeA);
            WeldingUtilities.SpawnStructures(wData.LinkedPartB, nodeB);


            if (compress)
                WeldingUtilities.MovePart(wData.LinkedPartB, offset);

            PartJoint newJoint = PartJoint.Create(
                wData.LinkedPartB,
                wData.LinkedPartA,
                nodeB,
                nodeA,
                AttachModes.STACK);

            wData.LinkedPartB.attachJoint = newJoint;

            WeldingUtilities.Explode(wData.DockingPortA);
            WeldingUtilities.Explode(wData.DockingPortB);
        }


        public static Part FindAttachedPart(Part p, Part xp)
        {
            //Debug.Log(string.Format("Finding attached part for {0} but excluding {1}", p.partInfo.title, xp.partInfo.title));
            //Debug.Log(string.Format("Part {0} has {1} attachment node(s)", p.partInfo.title, p.attachNodes.Count));
            foreach (var an in p.attachNodes)
            {
                //Debug.Log(string.Format("Looking at node {0}", an.id));
                if (an.attachedPart != null && an.attachedPart != xp)
                {
                    //Debug.Log(string.Format("Returning {0}", an.attachedPart.partInfo.title));
                    return an.attachedPart;
                }
            }

            //Debug.Log(string.Format("Part {0} parent part is {1}", p.partInfo.title, p.parent));
            //Debug.Log(string.Format("Part {0} has {1} children", p.partInfo.title, p.children.Count));


            if (p.parent != null)
                return p.parent;

            return p.children.Count == 1 ? p.children[0] : null;
        }
    }
}
