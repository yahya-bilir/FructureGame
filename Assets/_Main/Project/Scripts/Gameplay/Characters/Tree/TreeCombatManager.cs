using System.Collections.Generic;
using PropertySystem;
using UnityEngine;

namespace Characters.Tree
{
    public class TreeCombatManager : CharacterCombatManager
    {
        private readonly List<GameObject> _treeObjects;

        public TreeCombatManager(CharacterPropertyManager characterPropertyManager,
            CharacterVisualEffects characterVisualEffects, Character character, List<GameObject> treeObjects) : base(characterPropertyManager, characterVisualEffects, character)
        {
            _treeObjects = treeObjects;
        }
        
        public override void GetDamage(float damage)
        {
            base.GetDamage(damage);
            var newHealthAfterDamage = _characterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue;
            var maxHealth = _characterPropertyManager.GetProperty(PropertyQuery.MaxHealth).TemporaryValue;
            SetTreeObjects( (int) newHealthAfterDamage, (int) maxHealth);
        }

        private void SetTreeObjects(int health, int maxHealth)
        {
            var objectToOpen = maxHealth / health;
            _treeObjects.ForEach(i => i.SetActive(false));
            _treeObjects[objectToOpen].SetActive(true);
        }
    }
}