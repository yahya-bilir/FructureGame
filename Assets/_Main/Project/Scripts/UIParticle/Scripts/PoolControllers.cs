using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FlingTamplate.UIParticle
{
    public class PositionController
    {
        private List<PositionData> _positionDatas = new List<PositionData>();
        private Transform _parent;
        private readonly int _offset = 20;
        private int _posCount = 0;
        private UIParticleManager _uiParticleManager;

        public PositionController(Transform parent, UIParticleManager manager)
        {
            _uiParticleManager = manager;
            _parent = parent;
        }

        // public void Initialize(List<Transform> positions)
        // {
        //     positions.ForEach(x => _positionDatas.Add(new PositionData { positionTransform = x, isFull = false }));
        // }

        public PositionData GetPosition()
        {
            for (int i = 0; i < _positionDatas.Count; i++)
            {
                if (!_positionDatas[i].isFull)
                {
                    _positionDatas[i].isFull = true;
                    return _positionDatas[i];
                }
            }

            return CreateNewPosition();
        }


        public void EmtyPosition(PositionData positionData)
        {
            if (positionData == null) return;


            _positionDatas.Find(x => x.positionTransform == positionData.positionTransform).isFull = false;
        }

        private PositionData CreateNewPosition()
        {
            var position = new PositionData();
            float xPos = _posCount * _offset;
            _posCount++;
            var pos = new GameObject($"new position {_positionDatas.Count}");
            pos.transform.parent = _uiParticleManager.transform;

            UnityEngine.Object.DontDestroyOnLoad(pos.gameObject);
            position.positionTransform = pos.transform;
            position.positionTransform.position = new Vector3(xPos, 0, 0);
            position.positionTransform.SetParent(_parent);
            _positionDatas.Add(position);
            return position;
        }
    }


    public class CamerePoolController
    {
        private GameObject _cameraPrefab;
        private List<Camera> _cameraPool;
        private UIParticleManager _particleManager;

        public void InitializePool(GameObject camPrefab, UIParticleManager particleManager)
        {
            _particleManager = particleManager;
            _cameraPrefab = camPrefab;
            _cameraPool = new();
        }

        public Camera GetCamera(float cameraSize = 1f)
        {
            if (_cameraPool.Count > 0)
            {
                Camera cam = _cameraPool[0];
                _cameraPool.Remove(cam);
                cam.orthographicSize = cameraSize;
                cam.gameObject.SetActive(true);
                return cam;
            }
            else
            {
                var cam = Object.Instantiate(_cameraPrefab, _particleManager.transform, true).GetComponent<Camera>();
                cam.orthographicSize = cameraSize;
                cam.gameObject.SetActive(true);
                UnityEngine.Object.DontDestroyOnLoad(cam.gameObject);
                return cam;
            }
        }

        public void BackToPool(Camera camera)
        {
            _cameraPool.Add(camera);
            camera.gameObject.SetActive(false);
        }
    }


    public class RenderTexturePoolController
    {
        private List<RenderTexture> _renderTexturePool;
        private List<RenderTexture> _allRenderTexture;

        public void InitializePool()
        {
            _renderTexturePool = new();
            _allRenderTexture = new();
        }

        public RenderTexture GetRenderTexture(int width = 256, int height = 256)
        {
            RenderTexture rt;
            if (_renderTexturePool.Count > 0)
            {
                rt = _renderTexturePool[0];

                _renderTexturePool.Remove(rt);
            }
            else
            {
                rt = new RenderTexture(256, 256, 32);
                rt.name = "RenderTexture" + Random.Range(0, 100);

                rt.dimension = TextureDimension.Tex2D;
                rt.memorylessMode = RenderTextureMemoryless.None;
                _allRenderTexture.Add(rt);
            }

            rt.width = width;
            rt.height = height;
            rt.graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;
            rt.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D24_UNorm_S8_UInt;
            rt.useMipMap = false;
            return rt;
        }


        public void BackToPool(RenderTexture renderTexture)
        {
            _renderTexturePool.Add(renderTexture);
            renderTexture.Release();
        }
    }


    public class ParticlePoolController
    {
        //  private List<GameObject> _particlePool;
        // private List<VisualEffect> _particlePool = new List<VisualEffect>();
        // private Dictionary<ParticleType,List<VisualEffect>> 
        private Dictionary<ParticleType, List<VisualEffect>> _particlePool;
        private Dictionary<ParticleType, GameObject> _particlePrefab;

        private Dictionary<ParticleType, List<ParticleSystem>> _legacyParticlePool;
        private Dictionary<ParticleType, GameObject> _legacyParticlePrefab;
        private UIParticleManager _particleManager;

        public void InitializePool(List<ParticleData> particleData, UIParticleManager particleManager)
        {
            _particleManager = particleManager;
            _particlePool = new();
            _legacyParticlePool = new();
            SetupData(particleData);
        }

        private void SetupData(List<ParticleData> particleData)
        {
            _particlePrefab = new();
            _legacyParticlePrefab = new();
            particleData.ForEach(pd =>
            {
                if (pd.usesLegacySystem) _legacyParticlePrefab.Add(pd.particleType, pd.particlePrefab);
                else _particlePrefab.Add(pd.particleType, pd.particlePrefab);
            });
        }

        public VisualEffect GetParticle(ParticleType type)
        {
            if (_particlePool.ContainsKey(type))
            {
                var obj = _particlePool[type][0];
                _particlePool[type].Remove(obj);
                if (_particlePool[type].Count == 0) _particlePool.Remove(type);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                var prefab = Object.Instantiate(_particlePrefab[type], _particleManager.transform, true);
                UnityEngine.Object.DontDestroyOnLoad(prefab);
                prefab.gameObject.SetActive(true);
                return prefab.GetComponent<VisualEffect>();
            }
        }

        public ParticleSystem GetLegacyParticle(ParticleType type)
        {
            if (_legacyParticlePool.ContainsKey(type) && _legacyParticlePool[type].Count > 0)
            {
                var ps = _legacyParticlePool[type][0];
                _legacyParticlePool[type].RemoveAt(0);
                ps.gameObject.SetActive(true);
                if (ps != null) return ps;
            }

            var obj = Object.Instantiate(_legacyParticlePrefab[type], _particleManager.transform, true);
            UnityEngine.Object.DontDestroyOnLoad(obj);
            obj.SetActive(true);
            return obj.GetComponent<ParticleSystem>();
        }

        public void BackToPool(ParticleType type, VisualEffect particle)
        {
            if (_particlePool.ContainsKey(type)) _particlePool[type].Add(particle);
            else _particlePool.Add(type, new List<VisualEffect>() { particle });

            particle.gameObject.SetActive(false);
        }

        public void BackToLegacyPool(ParticleType type, ParticleSystem ps)
        {
            if (!_legacyParticlePool.ContainsKey(type))
                _legacyParticlePool[type] = new List<ParticleSystem>();

            _legacyParticlePool[type].Add(ps);
            ps.gameObject.SetActive(false);
        }
    }
}