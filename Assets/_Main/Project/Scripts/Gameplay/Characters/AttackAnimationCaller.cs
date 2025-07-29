using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Characters
{
    public class AttackAnimationCaller : MonoBehaviour
    {
        private Character _character;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        private void Awake()
        {
            _character = GetComponentInParent<Character>();
            if (_character == null)
            {
                Debug.LogError("AttackAnimationCaller requires a Character component on the same GameObject.");
            }
        }

        public void CallAttackEvent()
        {
            _eventBus.Publish(new OnCharacterAttacked(_character));
        }
        
    }
}