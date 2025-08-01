using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;


namespace FlingTamplate.UIParticle
{
    public class ActiveParticleBase
    {
        public ParticleType ParticleType;
        public Camera ActiveCamera;
        public VisualEffect ActiveParticle;
        public RenderTexture ActiveRenderTexture;
        public Action<ActiveParticleBase> BackToPoolAll;
        public PositionData PositionData;

        public ParticleSystem ActiveLegacyParticle;
        public bool IsLegacyParticle;
    }

    public class ActiveSingleBurstParticleData : ActiveParticleBase
    {
        public float ParticleDuration;
        public List<ActiveSingleBurstParticleData> OderParticles=new();

        public ActiveSingleBurstParticleData(float particleDuration)
        {
            ParticleDuration = particleDuration;
            OnCompletedParticle().Forget();
        }

        private async UniTaskVoid OnCompletedParticle()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(ParticleDuration));
            BackToPoolAll?.Invoke(this);
        }

        //Remove ve   
        //Add ve 
    }

    public class ActiveParticleData : ActiveParticleBase
    {
        private List<string> _activeVisualElements = new();

        public void AddUsage(string visualElementName)
        {
            if (_activeVisualElements.Contains(visualElementName)) return;
            _activeVisualElements.Add(visualElementName);
        }

        public void RemoveUsage(string visualElementName)
        {
            _activeVisualElements.ForEach(x => Debug.LogWarning($"ID {x}"));
            _activeVisualElements.Remove(visualElementName);
            Debug.LogWarning($"Remain Count {_activeVisualElements.Count} ");

            if (_activeVisualElements.Count == 0) BackToPoolAll?.Invoke(this);
        }
        //Remove ve   
        //Add ve 
    }

    [System.Serializable]
    public class ParticleData
    {
        public ParticleType particleType;
        public GameObject particlePrefab;
        public Vector3 particleOffset;
        [Header("RenderTexture Info")] public Vector2Int RenderTextureSize;
        [Header("Camera Info")] public float CameraSize;
        public float ParticleDuration = 1f;
        public bool usesLegacySystem;
    }

    [System.Serializable]
    public class PositionData
    {
        public Transform positionTransform;
        public bool isFull;
    }

    public struct ParticleAdditionData
    {
        public bool IsSingleUsage;
        public string VisualElementName;
    }
}