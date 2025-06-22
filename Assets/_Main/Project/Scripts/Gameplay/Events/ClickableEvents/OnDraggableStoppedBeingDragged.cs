using UI.PerksAndDraggables;

namespace Events.ClickableEvents
{
    public class OnDraggableStoppedBeingDragged
    {
        public Draggable Draggable { get; private set; }
        
        public OnDraggableStoppedBeingDragged(Draggable draggable)
        {
            Draggable = draggable;
        }
    }
}