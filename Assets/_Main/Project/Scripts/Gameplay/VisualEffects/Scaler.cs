using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VisualEffects
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private List<TransformHolder> transforms;

        private void Start()
        {
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

        [Button]
        public void ScaleUpInOrder()
        {
            ScaleUp().Forget();
        }

        private async UniTask ScaleUp()
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
        }
    }

    [Serializable]
    public class TransformHolder
    {
        [field: SerializeField] public List<Transform> Transforms { get; private set; }
        public List<Vector3> InitialScales { get; set; }
        [field: SerializeField] public float WaitForSeconds { get; private set; }
    }
}