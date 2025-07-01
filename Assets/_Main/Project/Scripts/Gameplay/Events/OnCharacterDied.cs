using Characters;

namespace Events
{
    public class OnCharacterDied
    {
        public Character Character { get; }

        public OnCharacterDied(Character character)
        {
            Character = character;
        }
    }
}