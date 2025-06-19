using Characters;

public class MeleeAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;

    public MeleeAttacking(CharacterAnimationController animationController, float interval, CharacterCombatManager combatManager)
        : base(animationController, interval)
    {
        _combatManager = combatManager;
    }

    protected override void OnAttack()
    {
        //_combatManager.TryDealMeleeDamage();
    }
}