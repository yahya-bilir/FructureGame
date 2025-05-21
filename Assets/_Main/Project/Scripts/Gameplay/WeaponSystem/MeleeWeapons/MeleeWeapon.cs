using System.Collections.Generic;
using _Main.Project.Scripts.Utils;
using Characters;
using UnityEngine;

namespace WeaponSystem.MeleeWeapons
{
    public class MeleeWeapon : TriggerWeapon
    {
        private float _attackingTimer;
        private List<Character> _triggeredCharacters = new List<Character>();
        private WeaponSO _weaponSo;
        private void Awake()
        {
            _weaponSo = ObjectUIIdentifierSo as WeaponSO;
        }

        private void Update()
        {
            if(_triggeredCharacters.Count == 0) return;
            if (_attackingTimer < _weaponSo.AttackInterval)
            {
                _attackingTimer += Time.deltaTime;
                return;
            }
            
            _attackingTimer = 0;
            _triggeredCharacters.ForEach(i => i.CharacterCombatManager.GetDamage(Damage));
        }

        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            if (!other.CompareTag(Tags.Enemy)) return;

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
    }
}