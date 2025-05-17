using Characters;

namespace Events
{
    public class OnCharacterDiedEvent
    {
        public Character Character { get; }

        public OnCharacterDiedEvent(Character character)
        {
            Character = character;
        }
    }
}