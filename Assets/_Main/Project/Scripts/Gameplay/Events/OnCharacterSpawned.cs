using Characters;

namespace Events
{
    public class OnCharacterSpawned
    {
        public Character SpawnedCharacter { get; private set; }
        public OnCharacterSpawned(Character character)
        {
            SpawnedCharacter = character;
        }
    }
}