using UnityEngine;

namespace RealismOverhaul.Utils
{
    public static class ScaleUtils
    {
        public static void Scale(this Part part, float scale, bool translateParts)
        {
            ScaleTransform(part, scale);
            ScaleAttachNodes(part, scale, translateParts);
        }

        private static void ScaleAttachNodes(Part part, float scale, bool translateParts)
        {
            for (int i = 0; i < part.attachNodes.Count; ++i)
            {
                TranslateNode(scale, part.attachNodes[i], part.GetPrefab().attachNodes[i], translateParts);
            }
            TranslateNode(scale, part.srfAttachNode, part.GetPrefab().srfAttachNode, translateParts);
        }

        private static void TranslateNode(float scale, AttachNode node, AttachNode baseNode, bool translateParts)
        {
            var oldPosition = node.position;
            TranslateNodePosition(scale, node, baseNode);
            TranslatePartIfNeeded(node, translateParts, oldPosition);
        }

        private static void TranslatePartIfNeeded(AttachNode node, bool translateParts, Vector3 oldPosition)
        {
            if (node.attachedPart == node.owner?.parent && translateParts)
            {
                TranslatePart(node, oldPosition);
            }
        }

        private static void TranslateNodePosition(float scale, AttachNode node, AttachNode baseNode) => node.position = GetTranslatedPosition(scale, baseNode);
        private static void TranslatePart(AttachNode node, Vector3 oldPosition) => node.owner.transform.Translate(oldPosition - node.position);

        private static Vector3 GetTranslatedPosition(float scale, AttachNode baseNode) => baseNode.position * scale;

        private static void ScaleTransform(Part part, float scale)
        {
            part.scaleFactor = part.GetPrefab().scaleFactor * scale;
            var modelTransform = part.GetModelTransform();
            modelTransform.localScale = scale * part.GetPrefab().GetModelTransform().localScale;
            modelTransform.hasChanged = true;
            part.partTransform.hasChanged = true;
        }
    }
}
