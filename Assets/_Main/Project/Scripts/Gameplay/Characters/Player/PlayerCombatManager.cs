using PropertySystem;

namespace Characters.Player
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        public PlayerCombatManager(Character connectedCharacter, CharacterPropertyManager characterPropertyManager, CharacterDataHolder characterDataHolder) : base(connectedCharacter, characterPropertyManager, characterDataHolder)
        {
        }
        
        public override void OnGettingAttacked(float damage, float attackInterval)
        {
            GetDamage(damage);
        }
    }
}