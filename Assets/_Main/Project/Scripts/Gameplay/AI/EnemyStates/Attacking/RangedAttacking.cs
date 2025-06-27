using Characters;
using UnityEngine;

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

    public override void Tick()
    {
        base.Tick();
        Debug.Log("Ranged Attacking");
    }
}