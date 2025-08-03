using AI.Base.Interfaces;
using Characters;
using EventBusses;
using Events;
using TMPro;
using UnityEngine;

public class Attacking : IState
{
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");
    private readonly CharacterCombatManager _combatManager;
    private readonly RangedWeapon _rangedWeapon;
    private readonly Transform _weaponTransform;
    private readonly TextMeshPro _aiText;
    private readonly Animator _rangedWeaponAnimator;
    private readonly IEventBus _eventBus;

    private float _cooldown;
    private bool _initialized;

    private const float requiredAngleThreshold = 5f; // derece cinsinden

    public Attacking(CharacterCombatManager combatManager, RangedWeapon rangedWeapon, Transform weaponTransform,
        TextMeshPro aiText, Animator rangedWeaponAnimator, IEventBus eventBus)
    {
        _combatManager = combatManager;
        _rangedWeapon = rangedWeapon;
        _weaponTransform = weaponTransform;
        _aiText = aiText;
        _rangedWeaponAnimator = rangedWeaponAnimator;
        _eventBus = eventBus;
    }

    public void Tick()
    {
        if (_rangedWeapon == null) return;

        var target = _combatManager.LastFoundEnemy;
        if (target == null || target.IsCharacterDead) return;

        Vector3 direction = (target.transform.position - _weaponTransform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // sadece Y ekseni dönüşü
        float targetY = targetRotation.eulerAngles.y;
        Vector3 currentEuler = _weaponTransform.rotation.eulerAngles;
        float currentY = currentEuler.y;

        float angleDifference = Mathf.DeltaAngle(currentY, targetY);

        if (Mathf.Abs(angleDifference) > requiredAngleThreshold)
        {
            float step = 360f * Time.deltaTime;
            float newY = Mathf.MoveTowardsAngle(currentY, targetY, step);
            _weaponTransform.rotation = Quaternion.Euler(0, newY, 0);
            return;
        }

        _cooldown += Time.deltaTime;
        //_rangedWeaponAnimator.SetBool(CanAttack, false);

        if (_cooldown >= _rangedWeapon.CurrentAttackInterval)
        {
            _rangedWeaponAnimator.SetBool(CanAttack, true);
            _cooldown = 0f;
        }
    }

    public void OnEnter()
    {
        _aiText.text = "Attacking to Enemy";
        _eventBus.Subscribe<OnCharacterAttacked>(OnCharacterAttacked);
        if (!_initialized)
        {
            _cooldown = 0f;
            _initialized = true;
        }
    }

    private void OnCharacterAttacked(OnCharacterAttacked eventData)
    {
        if(eventData.AttackedCharacter != _combatManager.Character) return;
        _rangedWeapon.Shoot(_combatManager.LastFoundEnemy);
    }

    public void OnExit()
    {
        _eventBus.Unsubscribe<OnCharacterAttacked>(OnCharacterAttacked);
    }
}
