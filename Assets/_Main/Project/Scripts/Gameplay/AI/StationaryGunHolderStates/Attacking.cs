using AI.Base.Interfaces;
using BasicStackSystem;
using Characters;
using EventBusses;
using Events;
using PropertySystem;
using TMPro;
using UnityEngine;
using WeaponSystem.RangedWeapons;

public class Attacking : IState
{
    private static readonly int CanAttack = Animator.StringToHash("CanAttack");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private readonly CharacterCombatManager _combatManager;
    private readonly RangedWeaponWithExternalAmmo _rangedWeapon;
    private readonly Transform _weaponTransform;
    private readonly TextMeshPro _aiText;
    private readonly Animator _rangedWeaponAnimator;
    private readonly IEventBus _eventBus;
    private readonly BasicStack _connectedStack;

    private float _cooldown;
    private bool _initialized;

    private const float requiredAngleThreshold = 5f; // derece cinsinden

    public Attacking(CharacterCombatManager combatManager, RangedWeapon rangedWeapon, Transform weaponTransform,
        TextMeshPro aiText, Animator rangedWeaponAnimator, IEventBus eventBus, BasicStack connectedStack)
    {
        _combatManager = combatManager;
        _rangedWeapon = rangedWeapon as RangedWeaponWithExternalAmmo;
        _weaponTransform = weaponTransform;
        _aiText = aiText;
        _rangedWeaponAnimator = rangedWeaponAnimator;
        _eventBus = eventBus;
        _connectedStack = connectedStack;
    }

    public void Tick()
    {
        var target = _combatManager.LastFoundEnemy;
        if (target == null || target.IsCharacterDead) return;

        // Y ekseninde yönelme
        var dir = (target.transform.position - _weaponTransform.position).normalized;
        var targetRot = Quaternion.LookRotation(dir, Vector3.up);
        float targetY = targetRot.eulerAngles.y;
        float currentY = _weaponTransform.rotation.eulerAngles.y;
        float angleDiff = Mathf.DeltaAngle(currentY, targetY);

        if (Mathf.Abs(angleDiff) > requiredAngleThreshold)
        {
            float step = 35f * Time.deltaTime;
            float newY = Mathf.MoveTowardsAngle(currentY, targetY, step);
            _weaponTransform.rotation = Quaternion.Euler(0, newY, 0);
            return; // hedefe dönmeden saldırma
        }

        // // --- ZAMANLAMA ---
        // float currentAtkSpeed = _combatManager.CharacterPropertyManager
        //     .GetProperty(PropertyQuery.AttackSpeed).TemporaryValue; // 1.0 => normal hız
        // if (currentAtkSpeed <= 0f) currentAtkSpeed = 0.0001f;
        //
        // float baseInterval = _rangedWeapon.CurrentAttackInterval; // RangedWeapon.Initialize'da SO'dan gelir
        // float effectiveInterval = baseInterval / currentAtkSpeed;
        //
        // _cooldown += Time.deltaTime;
        //
        // // Anim hızını güncelle
        //
        // if (_cooldown >= effectiveInterval)
        // {
        //     _rangedWeaponAnimator.SetBool(CanAttack, true);
        //     _cooldown = 0f;
        // }
    }

    public void OnEnter()
    {
        _aiText.text = "Attacking to Enemy";
        _eventBus.Subscribe<OnCharacterAttacked>(OnCharacterAttacked);
        _rangedWeaponAnimator.SetFloat(Speed, 1);

        //_cooldown = 0f;
        _rangedWeaponAnimator.SetBool(CanAttack, true);
    }


    private void OnCharacterAttacked(OnCharacterAttacked eventData)
    {
        if(eventData.AttackedCharacter != _combatManager.Character) return;
        _rangedWeapon.Shoot(_combatManager.LastFoundEnemy);
        _rangedWeaponAnimator.SetBool(CanAttack, false); // aynı karede kapat
        
    }

    public void OnExit()
    {
        _eventBus.Unsubscribe<OnCharacterAttacked>(OnCharacterAttacked);
    }
}
