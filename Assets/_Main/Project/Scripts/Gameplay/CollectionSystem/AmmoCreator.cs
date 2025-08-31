using BasicStackSystem;
using Cysharp.Threading.Tasks;
using Database;
using Dreamteck.Splines;
using UnityEngine;
using VContainer;

namespace CollectionSystem
{
    public class AmmoCreator : MonoBehaviour
    {
        private IObjectResolver _resolver;
        private CollectionSystemDataHolder _collectionSystemDataHolder;
        private GameObject _gameObjectToCreate;
        private int _requestedAmmoCreationCount;
        [SerializeField] private SplineComputer splineComputer;
        private PhysicsStack _stack;

        [Inject]
        private void Inject(IObjectResolver resolver, GameDatabase gameDatabase, PhysicsStack stack)
        {
            _resolver = resolver;
            _collectionSystemDataHolder = gameDatabase.CollectionSystemDataHolder;
            _gameObjectToCreate = _collectionSystemDataHolder.NormalAmmo;
            _stack = stack;
        }

        public async UniTask OnRangedWeaponCreated()
        {
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
            
            var ammoPrefab = _gameObjectToCreate;
            var ammoInstance = Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            var roller = new AmmoRailMovement(ammoInstance.transform, splineComputer, _collectionSystemDataHolder.CreatedAmmoSplineSpeed);
            _resolver.Inject(roller);
            roller.InitiateMovementActions().Forget();
        }


        public void ReduceRequest() => _requestedAmmoCreationCount -= 1;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.N)) _gameObjectToCreate = _collectionSystemDataHolder.NormalAmmo;
            if(Input.GetKeyDown(KeyCode.F)) _gameObjectToCreate = _collectionSystemDataHolder.FireAmmo;
            if(Input.GetKeyDown(KeyCode.I)) _gameObjectToCreate = _collectionSystemDataHolder.IceAmmo;
        }
    }
}