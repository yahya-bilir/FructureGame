using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;
using Utils;

namespace Characters
{
    public class CharacterVisualEffects
    {
        private readonly List<SpriteRenderer> _spriteRenderers;
        private readonly CharacterDataHolder _characterDataHolder;
        private ShineEffect _shineEffect;
        private readonly UIPercentageFiller _healthBar;
        private bool _isHealthStillRunning;
        public CharacterVisualEffects(List<SpriteRenderer> spriteRenderers, CharacterDataHolder characterDataHolder, UIPercentageFiller healthBar)
        {
            _spriteRenderers = spriteRenderers;
            _characterDataHolder = characterDataHolder;
            _healthBar = healthBar;
            Initialize();
        }

        private void Initialize()
        {
            _shineEffect = new ShineEffect(_spriteRenderers, _characterDataHolder.ShineColor, _characterDataHolder.ShineDuration);
        }


        public void OnCharacterTookDamage(float newHealth, float maxHealth)
        {
            _shineEffect.Shine();
            
            
            if(_healthBar == null) return; 
                
                
            SetHealthBarValue(newHealth, maxHealth);
            
            if(_isHealthStillRunning) return;
            OnDamageTakenHealthBarDisablingChecker().Forget();
            
        }

        private void SetHealthBarValue(float health, float maxHeaHealth)
        {
            var percentage = health / maxHeaHealth * 100; 
            _healthBar.SetUIPercentage(Mathf.RoundToInt(percentage));
        }

        public async UniTask OnCharacterDied()
        {
            _healthBar.DisableOrEnableObjectsVisibility(false);
            foreach (var renderer in _spriteRenderers)
            {
                DOVirtual.Color(renderer.color, new Color(1, 1, 1, 0), 2,
                    (color) => { renderer.color = color; });
            }
        }

        private async UniTask OnDamageTakenHealthBarDisablingChecker()
        {
            _isHealthStillRunning = true;
            _healthBar.DisableOrEnableObjectsVisibility(true);
            await UniTask.WaitForSeconds(2f);
            _isHealthStillRunning = false;
            _healthBar.DisableOrEnableObjectsVisibility(false);
        }
    }
}