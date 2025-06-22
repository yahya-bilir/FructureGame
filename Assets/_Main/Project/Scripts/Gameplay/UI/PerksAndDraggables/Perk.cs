using UnityEngine;

namespace UI.PerksAndDraggables
{
    public class Perk : Draggable
    {
        protected override void OnDragEndedOnScene(Vector2 worldPos)
        {
            base.OnDragEndedOnScene(worldPos);
            
        }

        public override void OnClickedSuccessfully()
        {
        }
    }
}