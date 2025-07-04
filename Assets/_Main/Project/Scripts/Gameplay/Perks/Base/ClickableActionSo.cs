using System.Collections.Generic;
using Characters;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Perks.Base
{
    public abstract class ClickableActionSo : ScriptableObject
    {
        protected List<Character> Characters = new List<Character>();
        protected List<Character> ExCharacters = new List<Character>();
        [field: SerializeField] public ClickableActionInfo ClickableActionInfo { get; private set; }

        private IEventBus _eventBus;
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        public abstract void OnDragEndedOnScene(Vector2 worldPos, float radius);

        public virtual void OnDrag(Vector2 worldPos, float radius)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius);
            ExCharacters = new List<Character>(Characters);
            Characters = new List<Character>();
            foreach (var hit in hits)
            {
                var character = hit.GetComponent<Character>();
                if (character != null)
                {
                    Characters.Add(character);
                }
            }
        }

        protected void SelectCharacters()
        {
            Characters.ForEach(i => _eventBus.Publish(new OnCharacterSelected(i)));
        }

        protected void DeselectCharacters()
        {
            var toDeselect = ExCharacters.FindAll(c => !Characters.Contains(c));

            foreach (var character in toDeselect)
            {
                _eventBus.Publish(new OnCharacterDeselected(character));
            }
        }
    }
}