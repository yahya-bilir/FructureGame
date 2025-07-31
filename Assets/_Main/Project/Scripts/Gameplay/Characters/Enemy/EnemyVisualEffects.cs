using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Database;
using DG.Tweening;
using EventBusses;
using MoreMountains.Feedbacks;
using UI;
using UnityEngine;
using VContainer;

namespace Characters.Enemy
{
    public class EnemyVisualEffects : CharacterVisualEffects
    {
        private readonly List<Renderer> _renderers;
        private GameDatabase _gameDatabase;

        public EnemyVisualEffects(UIPercentageFiller healthBar, ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController animationController, ParticleSystem hitVfx, MMF_Player feedback,
            List<Renderer> renderers) : base(healthBar, onDeathVfx, character, animationController, hitVfx, feedback)
        {
            _renderers = renderers;
        }
        
        [Inject]
        private void Inject(GameDatabase gameDatabase)
        {
            _gameDatabase = gameDatabase;
        }

        public async override UniTask OnCharacterDied()
        {
            base.OnCharacterDied().Forget();
            var dissolveMaterial = _gameDatabase.DissolveMaterial;

            foreach (var renderer in _renderers)
            {
                var originalMaterial = renderer.material;
                var originalTexture = originalMaterial.GetTexture("_BaseMap");
                //var normalMap = originalMaterial.GetTexture("_NormalMap");

                var newMaterial = new Material(dissolveMaterial);
                newMaterial.SetTexture("_BaseMap", originalTexture);
                //newMaterial.SetTexture("_NormalMap", normalMap);

                renderer.material = newMaterial;

                newMaterial.DOFloat(1f, "_Dissolve", 0.5f).SetEase(Ease.Linear);
            }
        }

    }
}