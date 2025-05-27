using System.Collections.Generic;
using Characters;
using UnityEngine;
using WeaponSystem;
using WeaponSystem.MeleeWeapons;

public class MeleeWeapon : UpgradeableWeapon, ITriggerWeapon
{
    private List<Character> _triggeredCharacters = new();
    private float _attackingTimer;
    private WeaponSO _weaponSo;

    private void Awake()
    {
        _weaponSo = ObjectUIIdentifierSo as WeaponSO;
        CurrentAttackInterval = _weaponSo.AttackInterval;
    }

    private void Update()
    {
        if (_triggeredCharacters.Count == 0) return;

        if (_attackingTimer < CurrentAttackInterval)
        {
            _attackingTimer += Time.deltaTime;
            return;
        }

        _attackingTimer = 0;
        _triggeredCharacters.ForEach(i => i.CharacterCombatManager.GetDamage(Damage));
    }

    public void OnTriggerEnter2D(Collider2D other) => TryProcessTrigger(other, true);
    public void OnTriggerExit2D(Collider2D other) => TryProcessTrigger(other, false);

    private void TryProcessTrigger(Collider2D other, bool isEntering)
    {
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Character>();
        if (enemy == null) return;

        if (isEntering)
        {
            if (_triggeredCharacters.Contains(enemy)) return;
            _triggeredCharacters.Add(enemy);
        }
        else
        {
            if (!_triggeredCharacters.Contains(enemy)) return;
            _triggeredCharacters.Remove(enemy);
        }
    }

    protected override void ApplyUpgradeEffects()
    {
        // İstenirse burada attack interval azaltımı eklenebilir
    }
}