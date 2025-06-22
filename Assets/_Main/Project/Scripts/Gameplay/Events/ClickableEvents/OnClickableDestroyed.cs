using UI.PerksAndDraggables;

namespace Events.ClickableEvents
{
    public class OnClickableDestroyed
    {
        public Clickable Clickable { get; private set; }

        public OnClickableDestroyed(Clickable clickable)
        {
            Clickable = clickable;
        }
    }
}