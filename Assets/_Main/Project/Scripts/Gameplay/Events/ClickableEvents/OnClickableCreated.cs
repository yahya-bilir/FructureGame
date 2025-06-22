using UI.PerksAndDraggables;

namespace Events.ClickableEvents
{
    public class OnClickableCreated
    {
        public Clickable Clickable { get; private set; }
        public OnClickableCreated(Clickable clickable)
        {
            Clickable = clickable;
        }
    }
}