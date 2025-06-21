using Characters;

namespace Events
{
    public class OnCharacterUpgraded
    {
        public Character DestroyedCharacter { get; private set; }
        public Character AddedCharacter { get; private set; }

        public OnCharacterUpgraded(Character destroyedCharacter, Character addedCharacter)
        {
            DestroyedCharacter = destroyedCharacter;
            AddedCharacter = addedCharacter;
        }
    }
}