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
            _eventBus.Subscribe<OnCharacterDiedEvent>(OnCharacterDied);
        }
        protected override void Awake()
        {
            base.Awake();
            CharacterCombatManager = new TreeCombatManager(CharacterPropertyManager, CharacterVisualEffects, this, treeParts);
            CharacterVisualEffects = new TreeVisualEffects(_childrenSpriteRenderers.ToList(), CharacterDataHolder,
                healthBar, onDeathVfx);
            _colliders = GetComponents<Collider2D>();
        }

        private void OnCharacterDied(OnCharacterDiedEvent eventData)
        {
            if (eventData.Character != this)
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
            _eventBus.Unsubscribe<OnCharacterDiedEvent>(OnCharacterDied);

        }
    }
}