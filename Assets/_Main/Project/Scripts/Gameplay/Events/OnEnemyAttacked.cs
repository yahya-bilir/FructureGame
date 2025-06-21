using Characters;

namespace Events
{
    public class OnEnemyAttacked
    {
        public Character AttackedCharacter { get; private set; }

        public OnEnemyAttacked(Character attackedCharacter)
        {
            AttackedCharacter = attackedCharacter;
        }
    }
}