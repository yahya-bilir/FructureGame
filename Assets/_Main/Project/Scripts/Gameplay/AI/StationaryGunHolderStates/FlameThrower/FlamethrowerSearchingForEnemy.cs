using AI.Base.Interfaces;
using Characters;
using TMPro;
using UnityEngine;

public class FlamethrowerSearchingForEnemy : IState
{
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");

    private readonly CharacterCombatManager _combatManager;
    private readonly TextMeshPro _aiText;
    private readonly Animator _rangedWeaponAnimator;
    private readonly string _name;
    private readonly MainBaseGetterAsATarget _target;

    public FlamethrowerSearchingForEnemy(CharacterCombatManager combatManager, TextMeshPro aiText,
        Animator rangedWeaponAnimator, string name, MainBaseGetterAsATarget target)
    {
        _combatManager = combatManager;
        _aiText = aiText;
        _rangedWeaponAnimator = rangedWeaponAnimator;
        _name = name;
        _target = target;
    }

    public void Tick()
    {
        if (!_target.HasValidTarget)
        {
            var found = _combatManager.FindNearestEnemy();
            _target.UpdateTarget(found);
        }
    }

    public void OnEnter()
    {
        _aiText.text = "Searching (FLAME)";
        _rangedWeaponAnimator.SetBool(CanAttack, false);
    }

    public void OnExit() { }
}