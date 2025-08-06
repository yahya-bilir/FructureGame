using UnityEngine;
using VContainer;

public class ElectricArcCharacter : StationaryGunHolderCharacter
{
    private MainBaseGetterAsATarget _targetProvider;
    private FlamethrowerSearchingForEnemy _searchingState;
    private ElectricAttacking _attackingState;

    [Inject]
    private void Inject(MainBaseGetterAsATarget targetProvider)
    {
        _targetProvider = targetProvider;
    }

    protected override void SetStates()
    {
        _searchingState = new FlamethrowerSearchingForEnemy(
            CharacterCombatManager,
            AIText,
            _rangedWeapon.Animator,
            name,
            _targetProvider
        );

        _attackingState = new ElectricAttacking(
            CharacterCombatManager,
            _rangedWeapon,
            _weaponTransform,
            AIText,
            _rangedWeapon.Animator,
            _eventBus,
            _targetProvider
        );

        _stateMachine.AddTransition(_searchingState, _attackingState, () => _targetProvider.HasValidTarget);
        _stateMachine.AddTransition(_attackingState, _searchingState, () => !_targetProvider.HasValidTarget);

        _stateMachine.SetState(_searchingState);
    }
}