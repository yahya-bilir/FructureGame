using Characters;
using IslandSystem;

namespace Events
{
    public class OnCharacterSpawned
    {
        public Character SpawnedCharacter { get; private set; }
        public Island SpawnedIsland { get; private set; }
        public OnCharacterSpawned(Character character, Island island)
        {
            SpawnedCharacter = character;
            SpawnedIsland = island;
        }
    }
}