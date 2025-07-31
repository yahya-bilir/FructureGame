using MoreMountains.Feedbacks;

namespace Characters.BaseSystem
{
    public class MainBase : StationaryGunHolderCharacter
    {
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
                model
                
            );
        }
    }
}