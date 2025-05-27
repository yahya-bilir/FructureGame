using UnityEngine;

namespace WeaponSystem
{
    public interface ITriggerWeapon
    {
        void OnTriggerEnter2D(Collider2D other);
        void OnTriggerExit2D(Collider2D other);
    }
}