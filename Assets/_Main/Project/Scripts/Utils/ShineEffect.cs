using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Main.Project.Scripts.Utils
{
    public class ShineEffect : IDisposable
    {
        private readonly List<SpriteRenderer> _spriteRenderers;
        [ColorUsage(true, true)] private Color _baseColor;
        [ColorUsage(true, true)] private readonly Color _shineColor;
        private List<Material> _materials;
        private readonly float _shineDuration;
        private CancellationTokenSource _source;
        
        public ShineEffect(List<SpriteRenderer> spriteRenderers, Color shineColor, float shineDuration)
        {
            _spriteRenderers = spriteRenderers;
            _shineColor = shineColor;
            _shineDuration = shineDuration;
            Setup();
        }

        private void Setup()
        {
            _materials = new();
            _spriteRenderers.ForEach(x => _materials.Add(x.material));
            _baseColor = _materials[0].GetColor("Color_207CF4A");
        }


        public void Shine()
        {
            _source?.Cancel();
            _source = new CancellationTokenSource();
            ShineAnim(_source.Token).Forget();
        }

        private async UniTaskVoid ShineAnim(CancellationToken ct)
        {
            _materials.ForEach(x => x.SetColor("Color_207CF4A", _shineColor));
            float duration = _shineDuration;
            float elapsed = 0;
            while (elapsed < duration && !ct.IsCancellationRequested)
            {
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t);
                Color currentColor = Color.Lerp(_shineColor, _baseColor, t);
                _materials.ForEach(x => x.SetColor("Color_207CF4A", currentColor));
                // _shineMat.SetColor("Color_207CF4A", currentColor);
                elapsed += Time.deltaTime;
                await UniTask.Yield(cancellationToken: ct);
            }

            _materials.ForEach(x => x.SetColor("Color_207CF4A", _baseColor));
        }

        public void Dispose()
        {
            _source?.Cancel();
            _source?.Dispose();
        }
    }
}