// FlameThrowerCharacter.cs

using UnityEngine;
using VContainer;

public class FlameThrowerCharacter : StationaryGunHolderCharacter
{
    private MainBaseGetterAsATarget _target;

    private FlamethrowerSearchingForEnemy _searchingState;
    private FlamethrowerAttacking _attackingState;

    [Inject]
    private void Inject(MainBaseGetterAsATarget target)
    {
        Debug.Log("Flamethrowercharacter Injected");
        _target = target;
    }

    protected override void SetStates()
    {
        _searchingState = new FlamethrowerSearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, name, _target);
        _attackingState = new FlamethrowerAttacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus, _target);

        _stateMachine.AddTransition(_searchingState, _attackingState, () => _target.HasValidTarget);
        _stateMachine.AddTransition(_attackingState, _searchingState, () => !_target.HasValidTarget);

        _stateMachine.SetState(_searchingState);
    }
}