using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public class ShineEffect : IDisposable
    {
        private readonly List<SpriteRenderer> _spriteRenderers;
        private readonly Color _shineColor;
        private readonly float _shineDuration;
        private Color _baseColor;
        private List<Material> _materials;
        private CancellationTokenSource _source;
        private bool _isShining;

        public ShineEffect(List<SpriteRenderer> spriteRenderers, Color shineColor, float shineDuration)
        {
            _spriteRenderers = spriteRenderers;
            _shineColor = shineColor;
            _shineDuration = shineDuration;
            SetupMaterials();
        }

        private void SetupMaterials()
        {
            _materials = new List<Material>();
            foreach (var sr in _spriteRenderers)
                _materials.Add(sr.material);

            //_baseColor = _materials[0].GetColor("Color_207CF4A");
        }

        public void Shine()
        {
            if (_isShining) return;

            _source?.Cancel();
            _source = new CancellationTokenSource();
            ShineAnim(_source.Token).Forget();
        }

        private async UniTaskVoid ShineAnim(CancellationToken ct)
        {
            _isShining = true;
            _materials.ForEach(m => m.SetColor("Color_207CF4A", _shineColor));

            float elapsed = 0f;

            while (elapsed < _shineDuration && !ct.IsCancellationRequested)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _shineDuration);
                Color lerped = Color.Lerp(_shineColor, Color.white, t);
                _materials.ForEach(m => m.SetColor("Color_207CF4A", lerped));

                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            _materials.ForEach(m => m.SetColor("Color_207CF4A", Color.white));
            _isShining = false;
        }

        public void Dispose()
        {
            _source?.Cancel();
            _source?.Dispose();
        }
    }
}
