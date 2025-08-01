using System;
using System.Collections.Generic;
using UnityEngine;


namespace FlingTamplate.UIParticle
{
    public class UIParticleManager : MonoBehaviour
    {
        // [SerializeField] private List<MeshRenderer> _renderer;

        [SerializeField] private GameObject cameraPrefab;
        [SerializeField] private List<ParticleData> particleData;
        [SerializeField] private List<Transform> positions;


        private Dictionary<ParticleType, ActiveParticleData> _activeParticlePool =
            new Dictionary<ParticleType, ActiveParticleData>();

        private Dictionary<ParticleType, ActiveSingleBurstParticleData> _activeSingleBurstParticles =
            new Dictionary<ParticleType, ActiveSingleBurstParticleData>();


        private CamerePoolController _cameraPoolController;
        private RenderTexturePoolController _renderTexturePoolController;
        private ParticlePoolController _particlePoolController;
        private PositionController _positionController;

        private void Awake()
        {
            transform.position = new Vector3(-150, -150);
            Setup();
            Config();
        }


        private void Setup()
        {
            _cameraPoolController = new();
            _renderTexturePoolController = new();
            _particlePoolController = new();
            _positionController = new(transform, this);
        }

        private void Config()
        {
            _cameraPoolController.InitializePool(cameraPrefab, this);
            _renderTexturePoolController.InitializePool();
            _particlePoolController.InitializePool(particleData, this);
            
            
            //     _positionController.Initialize(positions);
        }


        public RenderTexture Activate(ParticleType type, ParticleAdditionData additionData)
        {
            bool isContainParticleTypeInPool = _activeParticlePool.ContainsKey(type);
            bool isContainSingleBurstParticlePool = _activeSingleBurstParticles.ContainsKey(type);
            if (additionData.IsSingleUsage)
            {
                var pd = particleData.Find(pd => pd.particleType == type);
                var apd = new ActiveSingleBurstParticleData(pd.ParticleDuration);

                PrepareParticleSetup(apd, type);
                if (isContainSingleBurstParticlePool)
                {
                    _activeSingleBurstParticles[type].OderParticles.Add(apd);
                }
                else
                {
                    _activeSingleBurstParticles.Add(type, apd);
                }

                apd.BackToPoolAll += BackToPoolAll;
                return apd.ActiveRenderTexture;
            }
            else if (isContainParticleTypeInPool)
            {
                //Get pool 
                var crr = _activeParticlePool[type];
                crr.AddUsage(additionData.VisualElementName);
                return crr.ActiveRenderTexture;
            }
            else if (!isContainParticleTypeInPool)
            {
                var apd = new ActiveParticleData();
                PrepareParticleSetup(apd, type);

                apd.AddUsage(additionData.VisualElementName);
                apd.BackToPoolAll += BackToPoolAll;
                _activeParticlePool.Add(type, apd);
                //Add to active pool

                return apd.ActiveRenderTexture;
                //Create new particle setup
            }


            return null;
        }

        private void PrepareParticleSetup(ActiveParticleBase apd, ParticleType type)
        {
            var pd = particleData.Find(pd => pd.particleType == type);
            var positionData = _positionController.GetPosition();
            apd.ActiveCamera = _cameraPoolController.GetCamera(pd.CameraSize);
            apd.ActiveRenderTexture =
                _renderTexturePoolController.GetRenderTexture(pd.RenderTextureSize.x, pd.RenderTextureSize.y);
            //apd.ActiveParticle = _particlePoolController.GetParticle(type);
            if (pd.usesLegacySystem)
            {
                var particle = _particlePoolController.GetLegacyParticle(type);
                particle.transform.position = positionData.positionTransform.position + pd.particleOffset;
                apd.ActiveLegacyParticle = particle;
                apd.IsLegacyParticle = true;
                particle.Play();
            }
            else
            {
                var particle = _particlePoolController.GetParticle(type);
                particle.transform.position = positionData.positionTransform.position + pd.particleOffset;
                apd.ActiveParticle = particle;
                apd.IsLegacyParticle = false;
                particle.Play();
            }

            apd.ActiveCamera.gameObject.transform.position = positionData.positionTransform.position;

            // apd.ActiveParticle.transform.position = positionData.positionTransform.position + pd.particleOffset;
            apd.ActiveCamera.targetTexture = apd.ActiveRenderTexture;
            //apd.ActiveParticle.Play();
            apd.ParticleType = type;
        }

        public void DeActivate(bool b, ParticleType type, ParticleAdditionData particleAdditionData)
        {
            if (particleAdditionData.IsSingleUsage)
            {
                if (_activeSingleBurstParticles[type].OderParticles.Count > 0)
                {
                    _activeSingleBurstParticles[type].OderParticles.RemoveAt(0);
                    return;
                }

                _activeSingleBurstParticles.Remove(type);
                return;
            }

            if (!_activeParticlePool.ContainsKey(type)) Debug.LogWarning($"Particle pool does not contain {type}");
            else
            {
                var crr = _activeParticlePool[type];
                crr.RemoveUsage(particleAdditionData.VisualElementName);
            }
        }

        private void BackToPoolAll(ActiveParticleBase data)
        {
            //  _particlePoolController.BackToPool(data.ParticleType, data.ActiveParticle);
            if (data.IsLegacyParticle)
                _particlePoolController.BackToLegacyPool(data.ParticleType, data.ActiveLegacyParticle);
            else
                _particlePoolController.BackToPool(data.ParticleType, data.ActiveParticle);

            _renderTexturePoolController.BackToPool(data.ActiveRenderTexture);
            _cameraPoolController.BackToPool(data.ActiveCamera);
            _activeParticlePool.Remove(data.ParticleType);
            _positionController.EmtyPosition(data.PositionData);
        }
    }
}