using Characters;
using DG.Tweening;
using UI;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace Characters.BaseSystem
{
    public class MainBaseVisualEffects : CharacterVisualEffects
    {
        private readonly MMF_Player _feedback;
        private readonly GameObject _model;

        public MainBaseVisualEffects(UIPercentageFiller healthBar,
            ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController animationController,
            ParticleSystem hitVfx, MMF_Player feedback, GameObject model)
            : base(healthBar, onDeathVfx, character, animationController, hitVfx, feedback)
        {
            _feedback = feedback;
            _model = model;
        }

        public override void OnCharacterTookDamage(float newHealth, float maxHealth)
        {
            _model.transform.DOKill();
            _model.transform.localScale = Vector3.one;
            _model.transform.DOShakeScale(
                0.1f,
                0.5f,
                10,
                90,
                fadeOut: true
            ).SetEase(Ease.OutBack);

            _feedback.PlayFeedbacks();
        }
    }
}