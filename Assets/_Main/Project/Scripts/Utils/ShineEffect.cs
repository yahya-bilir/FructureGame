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
        private readonly float _shineDuration;
        private readonly Color _shineColor = new Color(2.5f, 2.5f, 1.5f, 1f);

        private readonly Dictionary<SpriteRenderer, MaterialPropertyBlock> _propertyBlocks;
        private CancellationTokenSource _source;
        private bool _isShining;

        public ShineEffect(List<SpriteRenderer> spriteRenderers, Color shineColor, float shineDuration)
        {
            _spriteRenderers = spriteRenderers;
            _shineDuration = shineDuration;
            _shineColor = shineColor;
            _propertyBlocks = new Dictionary<SpriteRenderer, MaterialPropertyBlock>();
            SetupPropertyBlocks();
        }

        private void SetupPropertyBlocks()
        {
            foreach (var sr in _spriteRenderers)
            {
                var block = new MaterialPropertyBlock();
                sr.GetPropertyBlock(block);
                block.SetColor("_Color", Color.white);
                sr.SetPropertyBlock(block);
                _propertyBlocks[sr] = block;
            }
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
            
            foreach (var (sr, block) in _propertyBlocks)
            {
                block.SetColor("_Color", _shineColor);
                sr.SetPropertyBlock(block);
            }
            
            float elapsed = 0f;
            
            while (elapsed < _shineDuration && !ct.IsCancellationRequested)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _shineDuration);
                Color lerped = Color.Lerp(_shineColor, Color.white, t);

                try
                {
                    foreach (var (sr, block) in _propertyBlocks)
                    {
                        block.SetColor("_Color", lerped);
                        sr.SetPropertyBlock(block);
                    }
                }
                catch (Exception e)
                {

                }

            
                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }
            try
            {
                foreach (var (sr, block) in _propertyBlocks)
                {
                    block.SetColor("_Color", Color.white);
                    sr.SetPropertyBlock(block);
                }
            }
            catch (Exception e)
            {

            }
            
            _isShining = false;
        }

        public void Dispose()
        {
            _source?.Cancel();
            _source?.Dispose();
        }
    }
}
