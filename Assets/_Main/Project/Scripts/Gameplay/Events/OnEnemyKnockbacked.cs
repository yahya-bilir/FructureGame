using Characters.Enemy;
using UnityEngine;

namespace Events
{
    public class OnEnemyKnockbacked
    {
        public EnemyBehaviour KnockbackedEnemy { get; }
        public Vector3 KnockbackDirection { get; }
        public KnockbackDataHolder KnockbackData { get; }

        public OnEnemyKnockbacked(EnemyBehaviour knockbackedEnemy, Vector3 knockbackDirection, KnockbackDataHolder knockbackData)
        {
            KnockbackedEnemy = knockbackedEnemy;
            KnockbackDirection = knockbackDirection;
            KnockbackData = knockbackData;
        }
    }
}