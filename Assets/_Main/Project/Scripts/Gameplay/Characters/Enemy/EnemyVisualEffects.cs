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
        private readonly Dictionary<DamageTypes, GameObject> _damageAndGameObjects;
        private readonly EnemyDestructionManager _enemyDestructionManager;
        private GameDatabase _gameDatabase;
        private GameObject _activeVfx;
        public EnemyVisualEffects(UIPercentageFiller healthBar, ParticleSystem onDeathVfx, Character character,
            CharacterAnimationController animationController, ParticleSystem hitVfx, MMF_Player feedback,
            List<Renderer> renderers, ParticleSystem spawnVfx, Dictionary<DamageTypes, GameObject> damageAndGameObjects,
            EnemyDestructionManager enemyDestructionManager) : base(healthBar, onDeathVfx, character, animationController, hitVfx, feedback, spawnVfx)
        {
            _renderers = renderers;
            _damageAndGameObjects = damageAndGameObjects;
            _enemyDestructionManager = enemyDestructionManager;
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
                if(renderer == null) continue;
                var originalMaterial = renderer.material;
                var originalTexture = originalMaterial.GetTexture("_BaseMap");
                var originalColor = originalMaterial.GetColor("_BaseColor");
                //var normalMap = originalMaterial.GetTexture("_NormalMap");

                var newMaterial = new Material(dissolveMaterial);
                newMaterial.SetTexture("_BaseMap", originalTexture);
                newMaterial.SetColor("_BaseColor", originalColor);

                //newMaterial.SetTexture("_NormalMap", normalMap);

                renderer.material = newMaterial;
                
                DoDissolveAfterWaiting(renderer).Forget();
            }
            
            _enemyDestructionManager.DestroyAllParts();
            
        }

        public override void OnCharacterTookDamage(float newHealth, float maxHealth, DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.Fire:
                    SpawnVfx(_damageAndGameObjects[DamageTypes.Fire]).Forget();
                    break;
                case DamageTypes.Electric:
                    SpawnVfx(_damageAndGameObjects[DamageTypes.Electric]).Forget();
                    break;
                case DamageTypes.Normal:
                    base.OnCharacterTookDamage(newHealth, maxHealth, damageType);
                    break;
            }
            
        }
        private async UniTask DoDissolveAfterWaiting(Renderer renderer)
        {
            await UniTask.WaitForSeconds(1f);
            if(renderer == null) return;
            renderer.material.DOFloat(1f, "_Dissolve", 0.5f).SetEase(Ease.Linear);
        }

        private async UniTask SpawnVfx(GameObject vfxObj)
        {
            if(_activeVfx == vfxObj) return;
            if(_activeVfx != null) _activeVfx.SetActive(false);
            _activeVfx =  vfxObj;
            vfxObj.SetActive(true);
            await UniTask.WaitForSeconds(2f);
            _activeVfx.SetActive(false);
            
        }

    }
}