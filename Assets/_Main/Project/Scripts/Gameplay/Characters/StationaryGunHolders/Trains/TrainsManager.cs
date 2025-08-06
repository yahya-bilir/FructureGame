using System.Collections.Generic;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Trains
{
    public class TrainsManager : MonoBehaviour
    {
        [Header("Debug Components")]
        [SerializeField] private TrainEngine debugEngine;
        [SerializeField] private TrainEngine flameThrowerEngine;
        [SerializeField] private TrainEngine rocketEngine;
        [SerializeField] private TrainEngine electricEngine;

        private IObjectResolver _resolver;
        private IEventBus _eventBus;
        private CamerasManager _camerasManager;

        [SerializeField] private List<TrainSystem> trainSystems;

        private int _openedSystemsCount;

        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus, CamerasManager camerasManager)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            _camerasManager = camerasManager;
        }

        private void Awake()
        {
            foreach (var system in trainSystems)
            {
                system.Initialize(_resolver, _camerasManager);
                system.Disable();
            }
        }

        [Button]
        private void Start()
        {
            SpawnAllTrains().Forget();
        }

        private async UniTask SpawnAllTrains()
        {
            _eventBus.Publish(new OnEngineSelected(flameThrowerEngine, 0));
            
            await UniTask.WaitForSeconds(10);

            _eventBus.Publish(new OnEngineSelected(debugEngine, 1));

            await UniTask.WaitForSeconds(12);
            
            _eventBus.Publish(new OnEngineSelected(rocketEngine, 2));
            
            await UniTask.WaitForSeconds(15);
            
            _eventBus.Publish(new OnEngineSelected(electricEngine, 2));
            
            // for (int i = 0; i < 2; i++)
            // {
            //     if (debugEngine != null)
            //     {
            //     }
            //
            //     await UniTask.WaitForSeconds(10f);
            // }
        } 
        
        private void OnEnable()
        {
            _eventBus.Subscribe<OnEngineSelected>(HandleEngineSelected);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnEngineSelected>(HandleEngineSelected);
        }

        private void HandleEngineSelected(OnEngineSelected eventData)
        {
            OnEngineSelectedAsync(eventData).Forget();
        }

        private async UniTask OnEngineSelectedAsync(OnEngineSelected eventData)
        {
            if (eventData.SystemIndex < 0 || eventData.SystemIndex >= trainSystems.Count)
            {
                Debug.LogWarning($"Invalid system index: {eventData.SystemIndex}");
                return;
            }

            var system = trainSystems[eventData.SystemIndex];
            
            await system.AddEngineToSystem(eventData.Engine);
        }

    }
}