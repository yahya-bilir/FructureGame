using Characters;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public abstract class AmmoBase : TriggerWeapon
    {
        protected RangedWeapon _ownerWeapon;

        public void SetOwnerAndColor(RangedWeapon owner, Color color)
        {
            _ownerWeapon = owner;
            //modelRenderer.material.SetColor("_OuterOutlineColor", color);

            if (TryGetComponent(out TrailRenderer trail))
            {
                // color.a /= 2;
                // trail.startColor = color;
                // trail.endColor = color;
            }
        }

        public abstract void FireAt(Character target);
    }
}