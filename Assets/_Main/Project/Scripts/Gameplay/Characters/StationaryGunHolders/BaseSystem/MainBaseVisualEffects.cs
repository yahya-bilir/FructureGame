using Characters;
using UI;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace Characters.BaseSystem
{
    public class MainBaseVisualEffects : CharacterVisualEffects
    {
        public MainBaseVisualEffects(UIPercentageFiller healthBar,
            ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController animationController,
            ParticleSystem hitVfx, MMF_Player feedback)
            : base(healthBar, onDeathVfx, character, animationController, hitVfx, feedback)
        {
        }

        public override void OnCharacterTookDamage(float newHealth, float maxHealth)
        {
            // Şimdilik boş
        }
    }
}