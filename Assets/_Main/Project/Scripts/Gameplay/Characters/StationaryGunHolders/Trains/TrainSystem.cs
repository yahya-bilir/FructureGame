using System;
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

        public bool IsOccupied { get; private set; }

        private IObjectResolver _resolver;
        private TrainEngine _engineInstance;

        public void Initialize(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public void AddEngineToSystem(TrainEngine enginePrefab)
        {
            _engineInstance = GameObject.Instantiate(enginePrefab, EnginePlacementField.position, EnginePlacementField.rotation, EnginePlacementField);
            _resolver.Inject(_engineInstance);

            _engineInstance.SetSplineComputer(Spline);
            _engineInstance.SpawnWagon();

            Spline.gameObject.SetActive(true);
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