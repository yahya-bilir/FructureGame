using Characters.StationaryGunHolders.BaseSystem;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Characters.BaseSystem
{
    public class MainBase : Character
    {
        public Collider Collider { get; private set; }
        protected override void Awake()
        {
            base.Awake();

            // MainBaseVisualEffects ile override ediliyor
            CharacterVisualEffects = new MainBaseVisualEffects(
                healthBar,
                onDeathVfx,
                this,
                AnimationController,
                hitVfx,
                Feedback,
                model,
                spawnVfx
            );
            
            Collider = GetComponent<Collider>();
        }
    }
}