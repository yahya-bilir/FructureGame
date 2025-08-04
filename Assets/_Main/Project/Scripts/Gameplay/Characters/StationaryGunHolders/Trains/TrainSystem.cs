using System;
using System.Collections.Generic;
using CommonComponents;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using Trains;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

[Serializable]
public class TrainSystem
{
    [field: SerializeField] public SplineComputer Spline { get; private set; }
    [field: SerializeField] public Transform EnginePlacementField { get; private set; }
    [field: SerializeField] public CinemachineCamera CameraToActivate { get; private set; }
    [field: SerializeField] public RaySegmentSpawner RaySpawner { get; private set; }
    [field: SerializeField] public bool IsReversed { get; private set; }

    private IObjectResolver _resolver;
    private List<TrainEngine> _engineInstances = new();

    private bool _hasOpened = false;
    private CamerasManager _camerasManager;

    public void Initialize(IObjectResolver resolver, CamerasManager camerasManager)
    {
        _resolver = resolver;
        _camerasManager = camerasManager;
    }

    public async UniTask AddEngineToSystem(TrainEngine enginePrefab)
    {
        Spline.gameObject.SetActive(true);

        if (!_hasOpened)
        {
            _camerasManager.ChangeActivePlayerCamera(CameraToActivate);
            await RaySpawner.SpawnSegments();
            await UniTask.WaitForSeconds(0.33f);
            _hasOpened = true;
        }

        var instance = GameObject.Instantiate(
            enginePrefab,
            EnginePlacementField.position,
            EnginePlacementField.rotation,
            EnginePlacementField
        );

        _resolver.Inject(instance);
        instance.SetSplineComputer(Spline, IsReversed);

        await UniTask.WaitForSeconds(0.25f);

        for (int i = 0; i < 3; i++)
        {
            instance.SpawnWagon();
        }

        _engineInstances.Add(instance);
    }

    public void Disable()
    {
        Spline.gameObject.SetActive(false);

        foreach (var engine in _engineInstances)
        {
            if (engine != null)
                engine.gameObject.SetActive(false);
        }
    }
}