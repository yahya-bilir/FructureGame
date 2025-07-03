using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;
using Utilities.Vibrations;

namespace CommonComponents
{
    public class CamerasManager : MonoBehaviour
    {
        public CinemachineCamera ActivePlayerCam { get; private set; }
        [SerializeField] private CinemachineCamera defaultCamera;
        private MMF_Player _feedbacks;        
        protected void Awake()
        {
            ChangeActivePlayerCamera(defaultCamera);
            _feedbacks = GetComponent<MMF_Player>();
        }

        public void ChangeActivePlayerCamera(CinemachineCamera camera)
        {
            if(ActivePlayerCam != null) ActivePlayerCam.Priority = 1;
            ActivePlayerCam = camera;
            ActivePlayerCam.Priority = 10;
        }
        
        public void EnableCamera(CinemachineCamera camera)
        {
            ActivePlayerCam.Priority = 1;
            camera.Priority = 10;
        }

        public void DisableCamera(CinemachineCamera camera)
        {
            camera.Priority = 1;
            ActivePlayerCam.Priority = 10;
        }

        public void PanCameraCoroutineCaller(CinemachineCamera cam)
        {
            StartCoroutine(PanCamera(cam, 3f));
        }        
        
        public void PanCameraCoroutineCaller(CinemachineCamera cam, float interval)
        {
            StartCoroutine(PanCamera(cam, interval));
        }

        public IEnumerator PanCamera(CinemachineCamera camera, float interval)
        {
            //yield return new WaitForSeconds(0.5f);
            EnableCamera(camera);
            yield return new WaitForSeconds(interval);
            DisableCamera(camera);
        }

        public void ShakeCamera()
        {
            Vibrations.Medium();
            _feedbacks.PlayFeedbacks();
        }

        public async UniTask MoveCameraToPos(Vector3 pos)
        {
            //ActivePlayerCam.transform.DOMove(pos, 0.5f);
            ActivePlayerCam.transform.position = pos;
            await UniTask.WaitForSeconds(0.5f);
        }

        public void ToggleLensSize(float newLensSize)
        {
            var initialSize = ActivePlayerCam.Lens.OrthographicSize;
            ActivePlayerCam.Lens.OrthographicSize = newLensSize;
            
            var seq = DOTween.Sequence();
            
            // seq.Append(DOVirtual.Float(ActivePlayerCam.Lens.OrthographicSize, newLensSize, 0.2f,
            //     value => ActivePlayerCam.Lens.OrthographicSize = value).SetEase(Ease.OutBack));
            
            seq.Append(DOVirtual.Float(ActivePlayerCam.Lens.OrthographicSize,  initialSize, 1f,
                value => ActivePlayerCam.Lens.OrthographicSize = value).SetEase(Ease.OutBack));
        }
        
    }
}
