using UnityEngine;

namespace WeaponSystem
{
    public abstract class TriggerWeapon : ObjectWithDamage
    {
        private void OnTriggerEnter2D(Collider2D other) => TryProcessTrigger(other, true);

        private void OnTriggerExit2D(Collider2D other) => TryProcessTrigger(other, false);        
        
        private void OnTriggerEnter(Collider other) => TryProcessTrigger(other, true);

        private void OnTriggerExit(Collider other) => TryProcessTrigger(other, false);
        
        protected abstract void TryProcessTrigger(Collider2D other, bool isEntering);
        protected abstract void TryProcessTrigger(Collider other, bool isEntering);
    }
}