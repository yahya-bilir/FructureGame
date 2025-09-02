using System;
using BasicStackSystem;
using Characters;
using UnityEngine;
using UnityEngine.VFX;

namespace WeaponSystem.AmmoSystem
{
    public abstract class AmmoBase : TriggerWeapon
    {
        [SerializeField] private VisualEffect visualEffect;
        
        public Rigidbody Rigidbody {private set; get;}
        protected RangedWeapon _ownerWeapon;
        

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            
        }
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

        protected void StartVisualEffect()
        {
            if(visualEffect == null) return;
            visualEffect.SendEvent("loop");
        }

        protected void HitVisualEffect()
        {
            if(visualEffect == null) return;
            visualEffect.transform.parent = null;
            visualEffect.SendEvent("hit");
        }

        public virtual void FireAt(Character target)
        {
            StartVisualEffect();
        }

    }
}