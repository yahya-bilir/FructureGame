using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            _treeObjects.ForEach(i => i.SetActive(false));
            _treeObjects[0].SetActive(true);
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
            if(objectToOpen > _treeObjects.Count - 1 || objectToOpen < 0) objectToOpen = _treeObjects.Count - 1;
            //Debug.Log(objectToOpen + " " + health + " " + maxHealth + " " + _treeObjects.Count);
            _treeObjects.ForEach(i => i.SetActive(false));
            var targetTree = _treeObjects[objectToOpen];
            targetTree.SetActive(true);

            // Punch Scale efekti (Y ekseninde zıplama gibi)
            targetTree.transform.DOPunchScale(
                new Vector3(0f, 0.2f, 0f), // Sadece Y ekseninde
                0.5f,                      // Süre
                10,                         // Vibrations
                1f                       // Elasticity
            );
        }
        
        protected override async UniTask OnCharacterDied()
        {
            await base.OnCharacterDied();
            _character.gameObject.SetActive(false);
            //GameObject.Destroy(_character.gameObject);
        }
    }
}