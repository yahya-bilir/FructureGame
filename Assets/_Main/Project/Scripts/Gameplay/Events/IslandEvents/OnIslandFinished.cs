using IslandSystem;

namespace Events.IslandEvents
{
    public class OnIslandFinished
    {
        public Island FinishedIsland { get; private set; }

        public OnIslandFinished(Island island)
        {
            FinishedIsland = island;
        }
    }
}