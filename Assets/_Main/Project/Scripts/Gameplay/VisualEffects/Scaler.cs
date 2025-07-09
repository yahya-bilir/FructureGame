using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using Water2D;

namespace VisualEffects
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private Transform islandDropStartPos;
        [SerializeField] private Ease openingEase;
        
        [SerializeField] private List<IslandObj> islandObjects;

        [SerializeField] private List<GameObject> objectsToOpenDirectly;

        [SerializeField] private List<TransformHolder> transforms;
        //private ModernWater2D _moderWater2D;

        [SerializeField] private ModernWater2D _moderWater2D;

        
        // [Inject]
        // private void Inject(ModernWater2D modernWater2D)
        // {
        //     _moderWater2D = modernWater2D;
        //     Debug.Log("injected");
        // }

        private void Awake()
        {
            //_moderWater2D.enableSimulation.value = true;
            foreach (var islandObject in islandObjects)
            {
                islandObject.obj.SetActive(false);
            }
        }

        [Button]
        private void Start()
        {
            foreach (var objectToOpen in objectsToOpenDirectly)
            {
                objectToOpen.SetActive(false);
            }
            
            foreach (var transformHolder in transforms)
            {
                transformHolder.InitialScales = new List<Vector3>();
                foreach (var trf in transformHolder.Transforms)
                {
                    transformHolder.InitialScales.Add(trf.localScale);
                    trf.localScale = Vector3.zero;
                }
            }
            
            //_moderWater2D.enableSimulation.value = false;

        }

        public void ActivateObjects()
        {
            foreach (var objectToOpen in objectsToOpenDirectly)
            {
                objectToOpen.SetActive(true);
            }
        }

        public async UniTask ScaleUp()
        {

            foreach (var transformHolder in transforms)
            {
                for (var i = 0; i < transformHolder.Transforms.Count; i++)
                {
                    var trf = transformHolder.Transforms[i];
                    trf.DOScale(transformHolder.InitialScales[i], 0.5f).SetEase(Ease.OutBack);
                }

                await UniTask.WaitForSeconds(transformHolder.WaitForSeconds);
            }

            //await UniTask.WaitForSeconds(0.25f);
        }

        [Button]
        public void Reset()
        {
            foreach (var islandObject in islandObjects)
            {
                islandObject.obj.SetActive(false);
            }
        }

        [Button]
        public async UniTask SpawnIslandObjects()
        {
            foreach (var islandObject in islandObjects)
            {
                islandObject.obj.SetActive(false);
            }

            // for (var i = 0; i < objectsToOpenDirectly.Count; i++)
            // {
            //     if(i == 0 || i== 1) continue;
            //     
            //     var obj = objectsToOpenDirectly[i];
            //     obj.SetActive(true);
            //     var localScale = obj.transform.localScale;
            //     obj.transform.localScale = Vector3.zero;
            //     obj.transform.DOScale(localScale, 0.5f).SetEase(Ease.InOutBounce).OnComplete(() => { });
            // }

            //await UniTask.WaitForSeconds(0.45f);

            Debug.Log(_moderWater2D);
            //_moderWater2D.enableSimulation.value = true;

            for (var i = 0; i < islandObjects.Count; i++)
            {
                var islandObject = islandObjects[i];
                islandObject.obj.SetActive(true);
                // islandObject.obj.transform.position = islandDropStartPos.position;
                // islandObject.obj.transform.DOMove(islandObject.endPos.position, 0.33f).SetEase(Ease.OutBounce);
                // await UniTask.WaitForSeconds(0.2f);
                var obj = islandObject.obj;
                var localScale = obj.transform.localScale;
                obj.transform.localScale = Vector3.one * 0.15f;
                islandObject.obj.transform.DOScale(localScale, 0.75f).SetEase(Ease.OutBounce);
                await UniTask.WaitForSeconds(0.1f);
            }

            await UniTask.WaitForSeconds(islandObjects.Count * 0.05f);

            for (var i = 0; i < objectsToOpenDirectly.Count; i++)
            {
               // if(i == 0) continue;
                
                var obj = objectsToOpenDirectly[i];
                obj.SetActive(true);
                var localScale = obj.transform.localScale;
                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(localScale, 0.5f).SetEase(Ease.Linear).OnComplete(() => { });
            }
            
            await UniTask.WaitForSeconds(0.1f);

            await ScaleUp();
            //_moderWater2D.enableSimulation.value = false;


            //await UniTask.WaitForSeconds(0.5f);
            //
            // foreach (var islandObject in islandObjects)
            // {
            //     islandObject.obj.SetActive(false);
            // }

            //ActivateObjects();
        }
    }

    [Serializable]
    public struct IslandObj
    {
        public GameObject obj;
        public Transform endPos;
    }
}