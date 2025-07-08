using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace VisualEffects
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectsToOpenDirectly;
        
        [SerializeField] private List<TransformHolder> transforms;

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
    }
}