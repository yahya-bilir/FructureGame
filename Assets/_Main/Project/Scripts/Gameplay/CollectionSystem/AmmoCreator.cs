using System.Collections.Generic;
using BasicStackSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using WeaponSystem.AmmoSystem;

namespace CollectionSystem
{
    public class AmmoCreator : MonoBehaviour
    {
        private readonly Dictionary<BasicStack, AmmoBase> _stacksAndAmmo = new();
        [SerializeField] private Transform finalDestination;
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

            var ammoPrefab = _stacksAndAmmo[targetStack];
            var ammoInstance = Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            
            SendAmmoToStack(ammoInstance.transform, targetStack).Forget();
        }

        private async UniTask SendAmmoToStack(Transform ammoTransform, BasicStack stack)
        {
            await ammoTransform.DOMove(finalDestination.position, 0.25f).ToUniTask();
            stack.TryAddFromOutside(ammoTransform.gameObject.GetComponent<IStackable>());
        }
    }
}