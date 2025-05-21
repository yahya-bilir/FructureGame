using System.Collections.Generic;
using UnityEngine;

namespace WeaponSystem.Managers
{
    public abstract class CharacterWeaponManager
    {
        protected List<ObjectWithDamage> Weapons = new();
        public abstract void ReplaceWeapon(ObjectWithDamage weapon);
    }
}