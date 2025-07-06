using UI.PerksAndDraggables;

namespace Events.ClickableEvents
{
    public class OnDraggableDroppedToScene
    {
        public Draggable Draggable { get; private set; }

        public OnDraggableDroppedToScene(Draggable draggable)
        {
            Draggable = draggable;
        }
    }
}