using Characters;

public class RangedAttacking : BaseAttacking
{
    private readonly CharacterCombatManager _combatManager;

    public RangedAttacking(CharacterAnimationController animationController, float interval, CharacterCombatManager combatManager)
        : base(animationController, interval)
    {
        _combatManager = combatManager;
    }

    protected override void OnAttack()
    {
        //_combatManager.TryShootProjectile();
    }
}