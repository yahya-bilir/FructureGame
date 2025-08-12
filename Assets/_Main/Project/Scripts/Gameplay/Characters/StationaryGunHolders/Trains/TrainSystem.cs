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
    public List<TrainEngine> EngineInstances { get; private set; } = new();

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

        double startPercent;

        if (EngineInstances.Count == 0)
        {
            startPercent = Spline.Project(EnginePlacementField.position).percent;
        }
        else
        {
            var firstEngine = EngineInstances[0];
            double firstPercent = Spline.Project(firstEngine.transform.position).percent;
            startPercent = (firstPercent + 0.5) % 1.0;
        }

        var instance = GameObject.Instantiate(enginePrefab, EnginePlacementField.position, EnginePlacementField.rotation, EnginePlacementField);
        _resolver.Inject(instance);
        
        instance.SetSplineComputer(Spline, IsReversed, startPercent);

        await UniTask.WaitForSeconds(0.25f);

        instance.SpawnWagon();
        
        EngineInstances.Add(instance);
    }


    public void Disable()
    {
        Spline.gameObject.SetActive(false);

        foreach (var engine in EngineInstances)
        {
            if (engine != null)
                engine.gameObject.SetActive(false);
        }
    }
}