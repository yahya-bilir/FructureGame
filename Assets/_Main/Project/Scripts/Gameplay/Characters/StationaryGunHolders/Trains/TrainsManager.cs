using System.Collections.Generic;
using Characters.StationaryGunHolders.Trains;
using CommonComponents;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events;
using PropertySystem;
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
        private TrainEventsHandler _trainEventsHandler;
        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus, CamerasManager camerasManager)
        {
            _resolver = resolver;
            _eventBus = eventBus;
            _camerasManager = camerasManager;
        }

        private void Awake()
        {
            _trainEventsHandler =  new TrainEventsHandler(trainSystems);
            foreach (var system in trainSystems)
            {
                system.Initialize(_resolver, _camerasManager);
                system.Disable();
            }
        }

        #region Old spawn system

        [Button]
        
        private void Start()
        {
            //SpawnAllTrains().Forget();
            _resolver.Inject(_trainEventsHandler);
        }

        private async UniTask SpawnAllTrains()
        {
            await UniTask.WaitForSeconds(10);

            _eventBus.Publish(new OnEngineSelected(debugEngine, 0));
            //_eventBus.Publish(new OnEngineSelected(electricEngine, 0));
            
            await UniTask.WaitForSeconds(20);

            //_eventBus.Publish(new OnEngineSelected(flameThrowerEngine, 1));
            
            //_eventBus.Publish(new OnEngineSelected(debugEngine, 1));
            _eventBus.Publish(new OnEngineSelected(electricEngine, 1));

            await UniTask.WaitForSeconds(20);
            
            //_eventBus.Publish(new OnEngineSelected(rocketEngine, 2));
            _eventBus.Publish(new OnEngineSelected(flameThrowerEngine, 2));
            
            //_eventBus.Publish(new OnEngineSelected(electricEngine, 2));
            
            //await UniTask.WaitForSeconds(10);
            
            //_eventBus.Publish(new OnEngineSelected(electricEngine, 2));
                
            // for (int i = 0; i < 2; i++)
            // {
            //     if (debugEngine != null)
            //     {
            //     }
            //
            //     await UniTask.WaitForSeconds(10f);
            // }
        } 


        #endregion
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _eventBus.Publish(new OnEngineSelected(debugEngine, 0));
                _eventBus.Publish(new OnWagonCreationSelected(debugEngine, 2));
            }
            if(Input.GetKeyDown(KeyCode.I)) _eventBus.Publish(new OnEngineSelected(rocketEngine, 0));
            if(Input.GetKeyDown(KeyCode.O)) _eventBus.Publish(new OnEngineSelected(flameThrowerEngine, 1));
            if(Input.GetKeyDown(KeyCode.P)) _eventBus.Publish(new OnEngineSelected(electricEngine, 1));
        }
    }
}