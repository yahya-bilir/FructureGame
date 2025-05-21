using _Main.Project.Scripts.Utils;
using Characters;
using UnityEngine;

namespace WeaponSystem
{
    public class ThrowableWeapon : TriggerWeapon
    {
        protected override void TryProcessTrigger(Collider2D other, bool isEntering)
        {
            if (!other.CompareTag(Tags.Enemy)) return;

            var enemy = other.GetComponent<Character>();
            enemy.CharacterCombatManager.GetDamage(Damage);
        }
    }
}