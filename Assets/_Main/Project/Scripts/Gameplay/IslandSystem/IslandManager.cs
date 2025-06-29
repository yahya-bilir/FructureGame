using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace IslandSystem
{
    public class IslandManager : MonoBehaviour
    {
        [SerializeField] private Island firstIsland;
        [SerializeField] private List<Island> allIslands;
        private IObjectResolver _objectResolver;

        [Inject]
        private void Inject(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        private void Awake()
        {
            _objectResolver.Inject(firstIsland);
            foreach (var island in allIslands)
            {
                _objectResolver.Inject(island);
            }
        }

        private void Start()
        {
            firstIsland.StartIslandOpeningActions();
        }
    }
}