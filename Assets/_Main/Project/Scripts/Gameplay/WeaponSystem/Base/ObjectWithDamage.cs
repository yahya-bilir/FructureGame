using Characters;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;

namespace WeaponSystem
{
    public abstract class ObjectWithDamage : MonoBehaviour
    {
        [field: SerializeField] public ObjectUIIdentifierSO ObjectUIIdentifierSo { get; protected set; }
        protected float Damage { get; private set; }
        protected CharacterCombatManager ConnectedCombatManager;
        
        [Button]
        public virtual void SetNewDamage(float damage) => Damage = damage;

        public virtual void Initialize(CharacterCombatManager connectedCombatManager)
        {
            ConnectedCombatManager = connectedCombatManager;
        }
    }
}