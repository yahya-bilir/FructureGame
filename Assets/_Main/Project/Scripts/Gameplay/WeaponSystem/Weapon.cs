using _Main.Project.Scripts.Utils;
using Characters;
using UnityEngine;

namespace WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [field: SerializeField] public WeaponSO WeaponSo { get; private set; }
        private float _damage;
        private void OnTriggerStay2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Enemy)) return;
            var enemy = other.GetComponent<Character>();
            enemy.CharacterCombatManager.OnGettingAttacked(_damage, WeaponSo.AttackInterval);
        }
        
        public void SetNewDamage(float damage) => _damage = damage;
    }
}