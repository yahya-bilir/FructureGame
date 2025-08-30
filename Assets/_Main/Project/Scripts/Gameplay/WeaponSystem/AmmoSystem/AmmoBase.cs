using System;
using BasicStackSystem;
using Characters;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public abstract class AmmoBase : TriggerWeapon, IStackable
    {
        public Rigidbody Rigidbody {private set; get;}
        
        protected RangedWeapon _ownerWeapon;
        private ParticleSystem[] _particleSystems;
        public GameObject GameObject { get; private set; }

        private void OnEnable()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in _particleSystems)
            {
                particle.Clear();
            }

            GameObject = gameObject;
        }

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            
        }

        public void OnObjectStartedBeingCarried()
        {
            
        }

        public void OnObjectCollected()
        {
            
        }

        public void OnObjectDropped()
        {
            
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

        public abstract void FireAt(Character target);

    }
}