using EventBusses;
using UnityEngine;
using VContainer;

namespace Perks.PerkActions
{
    public abstract class PerkAction : ScriptableObject
    {
        [field: SerializeField] public string PerkName { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public PerkUIInfo PerkUIInfo { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        public abstract void Execute();
        
        protected IEventBus EventBus {get; private set; }
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            EventBus = eventBus;
        }
        
    }
}