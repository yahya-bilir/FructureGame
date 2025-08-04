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
                system.Initialize(_resolver);
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
            _eventBus.Publish(new OnEngineSelected(debugEngine));
            
            await UniTask.WaitForSeconds(10f);
            
            _eventBus.Publish(new OnEngineSelected(flameThrowerEngine));
            
            await UniTask.WaitForSeconds(10f);
            
            _eventBus.Publish(new OnEngineSelected(rocketEngine));
            
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
            if (_openedSystemsCount >= trainSystems.Count) return;

            var system = trainSystems[_openedSystemsCount];
            
            _camerasManager.ChangeActivePlayerCamera(system.CameraToActivate);
            
            await system.AddEngineToSystem(eventData.Engine); // async çağrı burada
            
            _openedSystemsCount++;
        }
    }
}