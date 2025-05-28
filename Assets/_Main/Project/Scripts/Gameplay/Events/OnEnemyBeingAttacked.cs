using Characters.Enemy;
using UnityEngine;

namespace Events
{
    public class OnEnemyBeingAttacked
    {
        public EnemyBehaviour AttackedEnemy { get; private set; }
        public Vector3 EnemyBeingAttackedPosition { get; private set; }

        public OnEnemyBeingAttacked(EnemyBehaviour attackedEnemy, Vector3 enemyBeingAttackedPosition)
        {
            AttackedEnemy = attackedEnemy;
            EnemyBeingAttackedPosition = enemyBeingAttackedPosition;
        }
    }
}