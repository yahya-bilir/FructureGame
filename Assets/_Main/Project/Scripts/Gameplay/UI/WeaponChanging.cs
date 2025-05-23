using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class WeaponChanging : MonoBehaviour
    {
        [SerializeField] private Transform topPoint;
        [SerializeField] private Transform middlePoint;
        [SerializeField] private Transform spawnPoint;
        private GameObject _alreadyCreatedObject;
        
        [Button]
        public void CreateObject(GameObject prefab)
        {
            var obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            var tween = DOTween.Sequence();
            if (_alreadyCreatedObject != null)
            {
                tween.Append(_alreadyCreatedObject.transform.DOMove(topPoint.position, 0.5f));
                //Destroy(_alreadyCreatedObject);
            }
            
            tween.Join(obj.transform.DOMove(middlePoint.position, 0.5f));

            
            tween.OnComplete(() =>
            {
                if(_alreadyCreatedObject != null) Destroy(_alreadyCreatedObject);
                _alreadyCreatedObject = obj;
            });

            
        }
    }
}
