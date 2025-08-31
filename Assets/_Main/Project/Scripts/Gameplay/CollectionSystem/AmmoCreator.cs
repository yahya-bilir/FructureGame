using System.Collections.Generic;
using BasicStackSystem;
using Characters.StationaryGunHolders;
using Cysharp.Threading.Tasks;
using Database;
using Dreamteck.Splines;
using UnityEngine;
using VContainer;
using WeaponSystem.AmmoSystem;
using WeaponSystem.AmmoSystem.Logic;

namespace CollectionSystem
{
    public class AmmoCreator : MonoBehaviour
    {
        private IObjectResolver _resolver;
        private CollectionSystemDataHolder _collectionSystemDataHolder;
        private GameDatabase _gameDatabase;
        private PhysicsStack _stack;

        private ElementType _currentElementType = ElementType.Normal;
        private int _requestedAmmoCreationCount;

        [SerializeField] private SplineComputer splineComputer;

        private readonly Dictionary<StationaryGunHolderCharacter, AmmoLogicType> _gunToLogicMap = new();

        [Inject]
        private void Inject(IObjectResolver resolver, GameDatabase gameDatabase, PhysicsStack stack)
        {
            _resolver = resolver;
            _gameDatabase = gameDatabase;
            _collectionSystemDataHolder = gameDatabase.CollectionSystemDataHolder;
            _stack = stack;
        }

        public async UniTask OnRangedWeaponCreated(StationaryGunHolderCharacter gunHolder, AmmoLogicType logicType)
        {
            _gunToLogicMap.Add(gunHolder, logicType);

            for (int i = 0; i < 3; i++)
            {
                CreateAmmo();
                await UniTask.WaitForSeconds(0.25f);
            }
        }

        public void CreateAmmo()
        {
            _requestedAmmoCreationCount++;

            int estimatedCount = _stack.Count + _requestedAmmoCreationCount;
            if (!_stack.IsThereAnySpace || estimatedCount >= _stack.Capacity) return;

            var visualPrefab = GetVisualOnlyPrefab(_currentElementType);

            var instance = Instantiate(visualPrefab, transform.position, Quaternion.identity);
            
            var roller = new AmmoRailMovement(instance.transform, splineComputer, _collectionSystemDataHolder.CreatedAmmoSplineSpeed);
            
            _resolver.Inject(roller);
            roller.InitiateMovementActions().Forget();
        }

        private GameObject GetVisualOnlyPrefab(ElementType element)
        {
            return element switch
            {
                ElementType.Fire => _collectionSystemDataHolder.FireAmmo,
                ElementType.Ice => _collectionSystemDataHolder.IceAmmo,
                ElementType.Normal => _collectionSystemDataHolder.NormalAmmo,
                _ => null
            };
        }
        
        public void ReduceRequest() => _requestedAmmoCreationCount--;

        public AmmoBase GetAmmoPrefab(StationaryGunHolderCharacter gunHolder)
        {
            var logic = _gunToLogicMap.TryGetValue(gunHolder, out var logicType) ? logicType : default;
            return _gameDatabase.GetAmmoPrefab(logic, _currentElementType);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N)) _currentElementType = ElementType.Normal;
            if (Input.GetKeyDown(KeyCode.F)) _currentElementType = ElementType.Fire;
            if (Input.GetKeyDown(KeyCode.I)) _currentElementType = ElementType.Ice;
        }
    }
}
