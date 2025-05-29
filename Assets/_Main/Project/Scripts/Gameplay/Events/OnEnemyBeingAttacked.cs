using Characters;
using UnityEngine;

namespace Events
{
    public class OnEnemyBeingAttacked
    {
        public Character AttackedEnemy { get; private set; }
        public Vector3 EnemyBeingAttackedPosition { get; private set; }

        public OnEnemyBeingAttacked(Character attackedEnemy, Vector3 enemyBeingAttackedPosition)
        {
            AttackedEnemy = attackedEnemy;
            EnemyBeingAttackedPosition = enemyBeingAttackedPosition;
        }
    }
}