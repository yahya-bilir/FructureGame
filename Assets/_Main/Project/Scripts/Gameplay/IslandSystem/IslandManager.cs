using System;
using System.Collections.Generic;
using EventBusses;
using Events.IslandEvents;
using UnityEngine;
using VContainer;

namespace IslandSystem
{
    public class IslandManager : MonoBehaviour
    {
        [SerializeField] private Island firstIsland;
        [SerializeField] private List<Island> allIslands;
        
        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;
        private AstarPath _astarPath;

        [Inject]
        private void Inject(IObjectResolver objectResolver, IEventBus eventBus, AstarPath aStarPath)
        {
            _objectResolver = objectResolver;
            _eventBus = eventBus;
            _astarPath = aStarPath;
        }
        private void Awake()
        {
            _objectResolver.Inject(firstIsland);
            foreach (var island in allIslands)
            {
                _objectResolver.Inject(island);
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnIslandStarted>(OnIslandStarted);
        }

        private void OnIslandStarted(OnIslandStarted obj)
        {
            _astarPath.Scan();
        }

        private void Start()
        {
            firstIsland.StartIslandOpeningActions();
        }
    }
}