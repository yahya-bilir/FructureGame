using System.Collections.Generic;
using System.Linq;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Characters.Tree
{
    public class Tree : Character
    {
        [SerializeField] private List<GameObject> treeParts;
        private IEventBus _eventBus;
        private Collider2D[] _colliders;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnCharacterDied>(OnCharacterDied);
        }
        protected override void Awake()
        {
            base.Awake();
            CharacterCombatManager = new TreeCombatManager(CharacterPropertyManager, CharacterVisualEffects, this, treeParts);
            CharacterVisualEffects = new TreeVisualEffects(ChildrenSpriteRenderers.ToList(), CharacterDataHolder,
                healthBar, onDeathVfx, this, AnimationController);
            _colliders = GetComponents<Collider2D>();
        }

        private void OnCharacterDied(OnCharacterDied data)
        {
            if (data.Character != this)
            {
                return;
            }
            foreach (var col in _colliders)
            {
                col.enabled = false;
            }
        }
        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnCharacterDied>(OnCharacterDied);

        }
    }
}