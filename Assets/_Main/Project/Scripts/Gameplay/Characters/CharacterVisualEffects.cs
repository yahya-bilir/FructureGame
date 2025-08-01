using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using MoreMountains.Feedbacks;
using UI;
using UnityEngine;
using VContainer;

namespace Characters
{
    public class CharacterVisualEffects
    {
        private readonly CharacterAnimationController _animationController;
        private readonly Character _character;
        private readonly CharacterAnimationController _characterAnimationController;
        private readonly MMF_Player _feedback;
        private readonly ParticleSystem _spawnVfx;
        private readonly UIPercentageFiller _healthBar;
        private readonly ParticleSystem _hitVfx;
        private readonly ParticleSystem _onDeathVfx;
        protected IEventBus _eventBus;
        private bool _isHealthStillRunning;

        public CharacterVisualEffects(UIPercentageFiller healthBar,
            ParticleSystem onDeathVfx, Character character, CharacterAnimationController animationController,
            ParticleSystem hitVfx, MMF_Player feedback, ParticleSystem spawnVfx)
        {
            _healthBar = healthBar;
            _onDeathVfx = onDeathVfx;
            _character = character;
            _animationController = animationController;
            _hitVfx = hitVfx;
            _feedback = feedback;
            _spawnVfx = spawnVfx;
        }


        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        public virtual void OnCharacterTookDamage(float newHealth, float maxHealth)
        {
            _feedback.PlayFeedbacks();
            if (_hitVfx != null) _hitVfx.Play();
            
            if (_healthBar == null) return;
            if(newHealth <= 0) return;
            //_characterAnimationController.GetHit();
            SetHealthBarValue(newHealth, maxHealth);

            if (_isHealthStillRunning) return;
            OnDamageTakenHealthBarDisablingChecker().Forget();
        }

        private void SetHealthBarValue(float health, float maxHeaHealth)
        {
            var percentage = health / maxHeaHealth * 100;
            _healthBar.SetUIPercentage(Mathf.RoundToInt(percentage));
        }

        public virtual async UniTask OnCharacterDied()
        {
            if (_healthBar != null)
                _healthBar.DisableOrEnableObjectsVisibility(false);

            if (_onDeathVfx != null)
            {
                _onDeathVfx.transform.parent = null;
                _onDeathVfx.Play();
            }
        }

        private async UniTask OnDamageTakenHealthBarDisablingChecker()
        {
            _isHealthStillRunning = true;
            _healthBar.DisableOrEnableObjectsVisibility(true);
            await UniTask.WaitForSeconds(2f);
            _isHealthStillRunning = false;
            if (_healthBar != null) _healthBar.DisableOrEnableObjectsVisibility(false);
        }

        public virtual void OnCharacterSpawnedVisualEffects()
        {
            if(_spawnVfx != null) _spawnVfx.Play();
            // var originalScale = _character.transform.localScale;
            // _character.transform.localScale = Vector3.zero;
            //
            // _character.transform.DOScale(originalScale, 0.1f)
            //     .SetEase(Ease.OutBack) // Tatlı bir geri zıplama efekti verir
            //     .OnComplete(() =>
            //     {
            //         // Ölçeklenme tamamlanınca animasyon tetikle
            //         //_characterAnimationController.EnableAnimator();
            //         _characterAnimationController.Spawn();
            //     });
        }
    }
}