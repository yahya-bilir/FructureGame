using Characters;

namespace Events
{
    public class OnCharacterAttacked
    {
        public Character AttackedCharacter { get; private set; }

        public OnCharacterAttacked(Character attackedCharacter)
        {
            AttackedCharacter = attackedCharacter;
        }
    }
}