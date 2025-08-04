// FlameThrowerCharacter.cs

using UnityEngine;
using VContainer;

public class FlameThrowerCharacter : StationaryGunHolderCharacter
{
    private FlamethrowerLeadTargetProvider _targetProvider;

    private FlamethrowerSearchingForEnemy _searchingState;
    private FlamethrowerAttacking _attackingState;

    [Inject]
    private void Inject(FlamethrowerLeadTargetProvider targetProvider)
    {
        Debug.Log("Flamethrowercharacter Injected");
        _targetProvider = targetProvider;
    }

    protected override void SetStates()
    {
        _searchingState = new FlamethrowerSearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, name, _targetProvider);
        _attackingState = new FlamethrowerAttacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus, _targetProvider);

        _stateMachine.AddTransition(_searchingState, _attackingState, () => _targetProvider.HasValidTarget);
        _stateMachine.AddTransition(_attackingState, _searchingState, () => !_targetProvider.HasValidTarget);

        _stateMachine.SetState(_searchingState);
    }
}