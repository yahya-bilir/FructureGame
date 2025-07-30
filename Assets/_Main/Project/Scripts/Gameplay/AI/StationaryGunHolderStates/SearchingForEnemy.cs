using AI.Base.Interfaces;
using Characters;
using TMPro;
using UnityEngine;

public class SearchingForEnemy : IState
{
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");
    private readonly CharacterCombatManager _combatManager;
    private readonly TextMeshPro _aiText;
    private readonly Animator _rangedWeaponAnimator;
    private readonly string _name;

    public SearchingForEnemy(CharacterCombatManager combatManager, TextMeshPro aiText, Animator rangedWeaponAnimator,
        string name)
    {
        _combatManager = combatManager;
        _aiText = aiText;
        _rangedWeaponAnimator = rangedWeaponAnimator;
        _name = name;
    }

    public void Tick()
    {
        var currentEnemy = _combatManager.LastFoundEnemy;
        if (currentEnemy != null && !currentEnemy.IsCharacterDead)
        {
            return;
        }

        _combatManager.FindNearestEnemy();
    }

    public void OnEnter()
    {
        _aiText.text = "Searching For Enemy";
        _rangedWeaponAnimator.SetBool(CanAttack, false);
    }

    public void OnExit() { }
}