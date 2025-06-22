using UI.PerksAndDraggables;

namespace Events.ClickableEvents
{
    public class OnDraggableStartedBeingDragged
    {
        public Draggable Draggable { get; private set; }
        
        public OnDraggableStartedBeingDragged(Draggable draggable)
        {
            Draggable = draggable;
        }
    }
}