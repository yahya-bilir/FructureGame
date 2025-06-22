using UnityEngine;

namespace UI.PerksAndDraggables
{
    public class ClickableAndConnectedTransform
    {
        public Transform ParentTransform { get; private set; }
        public Clickable Clickable { get; private set; }
        public ClickableAndConnectedTransform(Transform parentTransform, Clickable clickable)
        {
            ParentTransform = parentTransform;
            Clickable = clickable;
        }
    }
}