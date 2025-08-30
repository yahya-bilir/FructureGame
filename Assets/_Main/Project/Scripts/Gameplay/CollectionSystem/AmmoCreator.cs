using System.Collections.Generic;
using BasicStackSystem;
using Cysharp.Threading.Tasks;
using Dreamteck.Splines;
using UnityEngine;
using VContainer;       
using WeaponSystem.AmmoSystem;

namespace CollectionSystem
{
    public class AmmoCreator : MonoBehaviour
    {
        private readonly Dictionary<BasicStack, AmmoBase> _stacksAndAmmo = new();
        private IObjectResolver _resolver;
        [SerializeField] private SplineComputer splineComputer;

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        public void OnRangedWeaponCreated(BasicStack stack, AmmoBase ammo)
        {
            _stacksAndAmmo.Add(stack, ammo);
            for (int i = 0; i < 5; i++)
            {
                var ammoInstance = Instantiate(ammo, stack.transform.position, Quaternion.identity);
                stack.TryAddFromOutside(ammoInstance.GetComponent<IStackable>());
            }
        }

        public void CreateAmmo()
        {
            BasicStack targetStack = null;
            int minCount = int.MaxValue;

            foreach (var kv in _stacksAndAmmo)
            {
                var stack = kv.Key;
                if (!stack.IsThereAnySpace) continue;
                if (stack.Count < minCount)
                {
                    minCount = stack.Count;
                    targetStack = stack;
                }
            }
            
            if (targetStack == null) return;
            if(targetStack.Count > 6) return;
            var ammoPrefab = _stacksAndAmmo[targetStack];
            var ammoInstance = Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            var roller = new AmmoRailMovement(ammoInstance.transform, ammoInstance.Rigidbody, splineComputer);
            _resolver.Inject(roller);
            roller.InitiateMovementActions().Forget();
        }
    }
}