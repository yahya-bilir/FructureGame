using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

namespace Characters.Tree
{
    public class TreeVisualEffects : CharacterVisualEffects
    {
        private readonly ParticleSystem _onDeathVfx;

        public TreeVisualEffects(List<SpriteRenderer> spriteRenderers, CharacterDataHolder characterDataHolder, UIPercentageFiller healthBar, ParticleSystem onDeathVfx) : base(spriteRenderers, characterDataHolder, healthBar, onDeathVfx)
        {
            _onDeathVfx = onDeathVfx;
        }

        public override async UniTask OnCharacterDied()
        {
            if (_onDeathVfx != null)
            {
                _onDeathVfx.transform.parent = null;
                _onDeathVfx.Play();
            }
        }
    }
}