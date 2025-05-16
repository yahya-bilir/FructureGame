using System.Collections.Generic;
using _Main.Project.Scripts.Utils;
using Characters;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] public WeaponSO WeaponSo { get; private set; }
        private float _damage;
        private List<Character> _triggeredCharacters = new List<Character>();
        private float _attackingTimer;

        private void OnTriggerEnter2D(Collider2D other) => TryProcessTrigger(other, true);

        private void OnTriggerExit2D(Collider2D other) => TryProcessTrigger(other, false);

        [Button]
        public void SetNewDamage(float damage) => _damage = damage;
        
        private void TryProcessTrigger(Collider2D other, bool isEntering)
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

        private void Update()
        {
            if(_triggeredCharacters.Count == 0) return;
            if (_attackingTimer < WeaponSo.AttackInterval)
            {
                _attackingTimer += Time.deltaTime;
                return;
            }
            
            _attackingTimer = 0;
            _triggeredCharacters.ForEach(i => i.CharacterCombatManager.GetDamage(_damage));
        }
    }
}