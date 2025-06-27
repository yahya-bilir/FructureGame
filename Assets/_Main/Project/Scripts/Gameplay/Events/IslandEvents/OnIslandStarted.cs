using IslandSystem;

namespace Events.IslandEvents
{
    public class OnIslandStarted
    {
        public Island StartedIsland { get; private set; }

        public OnIslandStarted(Island island)
        {
            StartedIsland = island;
        }
    }
}