using System;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Trains
{
    [Serializable]
    public class TrainSystem
    {
        [field: SerializeField] public SplineComputer Spline { get; private set; }
        [field: SerializeField] public Transform EnginePlacementField { get; private set; }
        [field: SerializeField] public CinemachineCamera CameraToActivate { get; private set; }
        [field: SerializeField] public RaySegmentSpawner RaySpawner { get; private set; }
        [field: SerializeField] public bool IsReversed { get; private set; }
        public bool IsOccupied { get; private set; }

        private IObjectResolver _resolver;
        private TrainEngine _engineInstance;

        public void Initialize(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public async UniTask AddEngineToSystem(TrainEngine enginePrefab)
        {
            Spline.gameObject.SetActive(true);
            
            await RaySpawner.SpawnSegments();
            await UniTask.WaitForSeconds(0.33f);
            _engineInstance = GameObject.Instantiate(enginePrefab, EnginePlacementField.position, EnginePlacementField.rotation, EnginePlacementField);
            _resolver.Inject(_engineInstance);

            _engineInstance.SetSplineComputer(Spline, IsReversed);

            await UniTask.WaitForSeconds(0.25f);
            _engineInstance.SpawnWagon();

            IsOccupied = true;
        }

        public void Disable()
        {
            Spline.gameObject.SetActive(false);
            if (_engineInstance != null)
            {
                _engineInstance.gameObject.SetActive(false);
            }
        }
    }
}