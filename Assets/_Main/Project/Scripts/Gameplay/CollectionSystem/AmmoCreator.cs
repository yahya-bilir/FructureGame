using System.Collections.Generic;
using UnityEngine;
using WeaponSystem.RangedWeapons;

namespace CollectionSystem
{
    public class AmmoCreator : MonoBehaviour
    {
        private readonly List<RangedWeaponWithExternalAmmo> _weapons = new();
        [SerializeField] private Transform finalDestination;
        public void OnRangedWeaponCreated(RangedWeaponWithExternalAmmo weapon) => _weapons.Add(weapon);
        public void CreateAmmo()
        {
            
        }
    }
}