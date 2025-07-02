using EventBusses;
using Events.IslandEvents;
using UnityEngine;

namespace IslandSystem
{
    public class IslandCloud : MonoBehaviour
    {
        [SerializeField] private Transform cloudInnerPosition;
        
        private IEventBus _eventBus;

        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);   
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        }

        private void OnIslandSelected(OnIslandSelected eventData)
        {
            
        }
    }
}