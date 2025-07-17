using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using Events;
using UI;
using UnityEngine;
using Utils;
using VContainer;

namespace Characters
{
    public class CharacterVisualEffects : IDisposable
    {
        private readonly List<SpriteRenderer> _spriteRenderers;
        private readonly CharacterDataHolder _characterDataHolder;
        private ShineEffect _shineEffect;
        private readonly UIPercentageFiller _healthBar;
        private readonly ParticleSystem _onDeathVfx;
        private readonly Character _character;
        private readonly CharacterAnimationController _characterAnimationController;
        private readonly ParticleSystem _hitVfx;
        private bool _isHealthStillRunning;
        private IEventBus _eventBus;

        public CharacterVisualEffects(List<SpriteRenderer> spriteRenderers, CharacterDataHolder characterDataHolder,
            UIPercentageFiller healthBar, ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController characterAnimationController, ParticleSystem hitVfx)
        {
            _spriteRenderers = spriteRenderers;
            _characterDataHolder = characterDataHolder;
            _healthBar = healthBar;
            _onDeathVfx = onDeathVfx;
            _character = character;
            _characterAnimationController = characterAnimationController;
            _hitVfx = hitVfx;
            Initialize();
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnCharacterSelected>(OnCharacterSelected);
            _eventBus.Subscribe<OnCharacterDeselected>(OnCharacterDeselected);
        }

        private void Initialize()
        {
            _shineEffect = new ShineEffect(_spriteRenderers, _characterDataHolder.ShineColor, _characterDataHolder.ShineDuration, _character);
        }


        public void OnCharacterTookDamage(float newHealth, float maxHealth)
        {
            _shineEffect.Shine();
            
            
            if(_healthBar == null) return;

            if(_hitVfx != null) _hitVfx.Play();
            
            //_characterAnimationController.GetHit();
            SetHealthBarValue(newHealth, maxHealth);
            
            if(_isHealthStillRunning) return;
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
            //await UniTask.WaitForSeconds(_characterDataHolder.ShineDuration + 0.05f);

            await UniTask.WaitForSeconds(0.4f);
             foreach (var renderer in _spriteRenderers)
             {
                 renderer.enabled = false;
                 // var propertyBlock = new MaterialPropertyBlock();
                 // renderer.GetPropertyBlock(propertyBlock);
                 // var startColor = Color.white;
                 //
                 // DOVirtual.Color(startColor, new Color(startColor.r, startColor.g, startColor.b, 0f), 1f,
                 //     color =>
                 //     {
                 //         propertyBlock.SetColor("_Color", color);
                 //         renderer.SetPropertyBlock(propertyBlock);
                 //     });
             }
        }

        private async UniTask OnDamageTakenHealthBarDisablingChecker()
        {
            _isHealthStillRunning = true;
            _healthBar.DisableOrEnableObjectsVisibility(true);
            await UniTask.WaitForSeconds(2f);
            _isHealthStillRunning = false;
            if(_healthBar != null) _healthBar.DisableOrEnableObjectsVisibility(false);
        }

        public void SpawnCharacter()
        {
            //_characterAnimationController.DisableAnimator();
            var originalScale = _character.transform.localScale;
            _character.transform.localScale = Vector3.zero;

            // DOTween ile sanki yerden çıkıyormuş gibi ölçeği büyüt
            _character.transform.DOScale(originalScale, 0.1f)
                .SetEase(Ease.OutBack) // Tatlı bir geri zıplama efekti verir
                .OnComplete(() =>
                {
                    // Ölçeklenme tamamlanınca animasyon tetikle
                    //_characterAnimationController.EnableAnimator();
                    _characterAnimationController.Spawn();
                });
        }

        private void OnCharacterDeselected(OnCharacterDeselected eventData)
        {
            if(eventData.DeselectedCharacter != _character) return;
            //todo burada secimi kaldir
        }

        private void OnCharacterSelected(OnCharacterSelected eventData)
        {
            if(eventData.SelectedCharacter != _character) return;
            //todo burada sec
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe<OnCharacterSelected>(OnCharacterSelected);
            _eventBus.Unsubscribe<OnCharacterDeselected>(OnCharacterDeselected);
        }
    }
}