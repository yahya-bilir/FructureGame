using BasicStackSystem;
using UnityEngine;

namespace WeaponSystem.AmmoSystem
{
    public class StackableAmmo : AmmoProjectile, IStackable
    {
        public GameObject GameObject { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            GameObject = gameObject;
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
    }
}