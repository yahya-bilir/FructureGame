using Characters.StationaryGunHolders;
using CollectionSystem;
using CommonComponents;
using EventBusses;
using Events;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;
using VContainer;

namespace Video
{
    public class VideoManager : SingletonMonoBehaviour<VideoManager>
    {
        [SerializeField] private CinemachineCamera ballCam;
        [SerializeField] private CinemachineCamera normalCam;
        [SerializeField] private CinemachineCamera cannonCam;
        [SerializeField] private StationaryGunHolderCharacter cannon;
        
        
        private CamerasManager _camerasManager;
        private IEventBus _eventBus;
        private Transform _spawnedBallTrf;
        private CollectionArea _collectionArea;

        [Inject]
        private void Inject(CamerasManager camerasManager, IEventBus eventBus, CollectionArea collectionArea)
        {
            _camerasManager = camerasManager;
            _eventBus = eventBus;
            _collectionArea = collectionArea;
        }

        private void OnEnable()
        {
            Events.OnBallSpawned += OnBallSpawned;
            Events.OnBallClashed += OnBallClashed;
        }

        private void OnBallSpawned(Transform obj)
        {
            _spawnedBallTrf = obj;
        }

        private void OnBallClashed(Transform obj)
        {
            //_camerasManager.ChangeActivePlayerCamera(normalCam);
        }

        private void FollowBall()
        {
            ballCam.Follow = _spawnedBallTrf;
            _camerasManager.ChangeActivePlayerCamera(ballCam);
            _collectionArea.waitBeforeCollection = 2f;
        }        

        private void Update()
        {
            //İlk L ye bas
            //Sonra top yerleşince 1
            //Top kafadan vurunca Space hemen ardından I
            //Birkaç vuruş olsun. Ardından Ice ölünce C'e bas
            //Hemen akabinde R'ye bas ki Cannon'a geçsin.
            //Top yerleşince 1'e bas
            //Kırılınca Space'e bas
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FollowBall();
            }            
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _camerasManager.ChangeActivePlayerCamera(normalCam);
                _collectionArea.waitBeforeCollection = 1f;

            }     
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                _eventBus.Publish(new OnWeaponsCreated(new []{cannon}));
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                _camerasManager.ChangeActivePlayerCamera(cannonCam);
            }
        }

        private void OnDisable()
        {
            Events.OnBallSpawned -= OnBallSpawned;
            Events.OnBallClashed -= OnBallClashed;
        }
    }
}