using IslandSystem;

namespace Events
{
    public class OnAllIslandEnemiesKilled
    {
        public Island Island { get; set; }

        public OnAllIslandEnemiesKilled(Island island)
        {
            Island = island;
        }
    }
}