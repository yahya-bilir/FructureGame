using Characters;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace WeaponSystem
{
    public abstract class ObjectWithDamage : MonoBehaviour
    {
        [field: SerializeField] public ObjectUIIdentifierSO ObjectUIIdentifierSo { get; protected set; }
        protected float Damage;
        protected CharacterCombatManager ConnectedCombatManager;
        
        [Button]
        public void SetNewDamage(float damage) => Damage = damage;

        public void Initialize(CharacterCombatManager connectedCombatManager)
        {
            ConnectedCombatManager = connectedCombatManager;
        }
    }
}