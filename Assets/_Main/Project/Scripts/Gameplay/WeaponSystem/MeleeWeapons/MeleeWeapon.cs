using System.Collections.Generic;
using Characters;
using Events;
using UnityEngine;
using Utilities.Vibrations;
using WeaponSystem;
using WeaponSystem.MeleeWeapons;

public class MeleeWeapon : UpgradeableWeapon, ITriggerWeapon
{
    private List<Character> _triggeredCharacters = new();
    private float _attackingTimer;
    private WeaponSO _weaponSo;
    [SerializeField] private ParticleSystem attackVfx;

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

        foreach (var character in _triggeredCharacters)
        {
            if (attackVfx != null)
            {
                Vector3 directionToCharacter = character.transform.position - transform.position;
                float angle = Mathf.Atan2(directionToCharacter.y, directionToCharacter.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

                // VFX karakterin pozisyonunda oluşturulacak
                var instantiatedVfx = Instantiate(attackVfx, character.transform.position, rotation);

                // Rengi ayarla
                //Color vfxColor = character.CharacterDataHolder.OnAttackedVFXColor;
                //SetVfxColor(instantiatedVfx, vfxColor);
            }
            EventBus.Publish(new OnEnemyBeingAttacked(character, transform.position));
            
            character.CharacterCombatManager.GetDamage(Damage);
        }
        
        Vibrations.Soft();
    }

    private void SetVfxColor(ParticleSystem vfx, Color color)
    {
        var mainModule = vfx.main;
        mainModule.startColor = color;

        var childParticles = vfx.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in childParticles)
        {
            var childMain = ps.main;
            childMain.startColor = color;
        }
    }

    public void OnTriggerEnter2D(Collider2D other) => TryProcessTrigger(other, true);
    public void OnTriggerExit2D(Collider2D other) => TryProcessTrigger(other, false);

    private void TryProcessTrigger(Collider2D other, bool isEntering)
    {
        // if (!other.CompareTag("Enemy")) return;
        //
        // var enemy = other.GetComponent<Character>();
        // if (enemy == null) return;
        //
        // if (isEntering)
        // {
        //     if (_triggeredCharacters.Contains(enemy)) return;
        //     _triggeredCharacters.Add(enemy);
        // }
        // else
        // {
        //     if (!_triggeredCharacters.Contains(enemy)) return;
        //     _triggeredCharacters.Remove(enemy);
        // }
    }

    protected override void ApplyUpgradeEffects()
    {

    }
}
