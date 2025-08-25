using System.Collections.Generic;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Characters.StationaryGunHolders
{
    public class GunHolderPlacer : MonoBehaviour
    {
        [SerializeField] private Transform[] placementPoints;
        private int _lastPlacementPoint;
        private IEventBus _eventBus;
        private List<StationaryGunHolderCharacter> _createdWeapons = new();
        private IObjectResolver _resolver;
        public bool IsThereAnyWeapon => _createdWeapons.Count > 0;

        [Inject]
        private void Inject(IEventBus eventBus, IObjectResolver resolver)
        {
            _eventBus =  eventBus;
            _resolver = resolver;
            _eventBus.Subscribe<OnWeaponsCreated>(OnWeaponsCreated);
        }

        private void OnWeaponsCreated(OnWeaponsCreated eventData)
        {
            foreach (var characterPrefab in eventData.StationaryGunHolderCharacters)
            {
                Transform targetPoint = placementPoints[_lastPlacementPoint];
                
                var instance = Instantiate(characterPrefab, targetPoint.position, targetPoint.rotation);
                
                _resolver.Inject(instance);
                
                instance.transform.SetParent(targetPoint);
                
                _lastPlacementPoint++;
                if (_lastPlacementPoint >= placementPoints.Length)
                    _lastPlacementPoint = 0;
                
                _createdWeapons.Add(instance);
            }
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnWeaponsCreated>(OnWeaponsCreated);
        }
    }
}