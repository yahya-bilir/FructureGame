using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events.IslandEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace IslandSystem
{
    public class CloudMovementManager : MonoBehaviour
    {
        [SerializeField] private List<IslandCloud> clouds;
        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IObjectResolver objectResolver, IEventBus eventBus)
        {
            _objectResolver = objectResolver;
            _eventBus = eventBus;
        }
        private void Start()
        {
            foreach (var cloud in clouds)
            {
                _objectResolver.Inject(cloud);
            }
        }
        
        [Button]
        public async UniTask StartCloudActions()
        {
            await UniTask.WhenAll(
                clouds.ConvertAll(cloud => cloud.PerformCloudActions())
            );
            
            _eventBus.Publish(new OnCloudActionsCompleted());
        }
    }
}