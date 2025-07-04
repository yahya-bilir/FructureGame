using System.Collections.Generic;
using Characters;
using Characters.Transforming;
using Perks.Base;
using UnityEngine;
using VContainer;

namespace Perks
{
    [CreateAssetMenu(fileName = "UpgradeCharactersPerk", menuName = "Scriptable Objects/Perks/Upgrade Characters Perk")]
    public class UpgradeCharactersPerk : ClickableActionSo
    {
        private CharacterTransformManager _characterTransformManager;
        
        [Inject]
        private void Inject(CharacterTransformManager characterTransformManager)
        {
            _characterTransformManager = characterTransformManager;
        }

        public override void OnDrag(Vector2 worldPos, float radius)
        {
            base.OnDrag(worldPos, radius);
            SelectCharacters();
            DeselectCharacters();
        }

        public void OnSelected()
        {
            
        }

        public override void OnDragEndedOnScene(Vector2 worldPos, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius);
            var characters = new List<Character>();
            foreach (var hit in hits)
            {
                var character = hit.GetComponent<Character>();
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            
            _characterTransformManager.TryUpgradeCharacters(characters);
        }
    }
}