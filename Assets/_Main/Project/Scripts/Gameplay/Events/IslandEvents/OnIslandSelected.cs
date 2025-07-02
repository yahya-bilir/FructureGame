using IslandSystem;

namespace Events.IslandEvents
{
    public class OnIslandSelected
    {
        public Island SelectedIsland { get; private set; }

        public OnIslandSelected(Island selectedIsland)
        {
            SelectedIsland = selectedIsland;
        }
    }
}