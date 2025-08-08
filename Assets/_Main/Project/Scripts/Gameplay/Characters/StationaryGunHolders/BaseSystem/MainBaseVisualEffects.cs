using DG.Tweening;
using Events;
using MoreMountains.Feedbacks;
using UI;
using UnityEngine;

namespace Characters.StationaryGunHolders.BaseSystem
{
    public class MainBaseVisualEffects : CharacterVisualEffects
    {
        private readonly MMF_Player _feedback;
        private readonly GameObject _model;

        public MainBaseVisualEffects(UIPercentageFiller healthBar,
            ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController animationController,
            ParticleSystem hitVfx, MMF_Player feedback, GameObject model, ParticleSystem spawnVfx)
            : base(healthBar, onDeathVfx, character, animationController, hitVfx, feedback, spawnVfx)
        {
            _feedback = feedback;
            _model = model;
        }
        
        public override void OnCharacterTookDamage(float newHealth, float maxHealth, DamageTypes damageType)
        {
            _model.transform.DOKill();
            _model.transform.localScale = Vector3.one;
            // _model.transform.DOShakeScale(
            //     0.1f,
            //     0.5f,
            //     10,
            //     90,
            //     fadeOut: true
            // ).SetEase(Ease.OutBack);

            _feedback.PlayFeedbacks();    
            
            _eventBus.Publish(new OnBaseGotAttacked());
            
            if(newHealth <= 0f) _eventBus.Publish(new OnBaseDied());
        }

    }
}