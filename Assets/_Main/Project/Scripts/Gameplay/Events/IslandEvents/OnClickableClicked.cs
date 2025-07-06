using UI.PerksAndDraggables;

namespace Events.IslandEvents
{
    public class OnClickableClicked
    {
        public Clickable Clickable { get; private set; }

        public OnClickableClicked(Clickable clickable)
        {
            Clickable = clickable;
        }
    }
}