using Characters.Enemy;
using UnityEngine;

namespace Events
{
    public class OnEnemyCrushed
    {
        public EnemyBehaviour CrushedEnemy { get; }
        public Vector3 ImpactPoint { get; }
        public RagdollDataHolder RagdollData { get; }

        public OnEnemyCrushed(EnemyBehaviour crushedEnemy, Vector3 impactPoint, RagdollDataHolder ragdollData)
        {
            CrushedEnemy = crushedEnemy;
            ImpactPoint = impactPoint;
            RagdollData = ragdollData;
        }
    }
}