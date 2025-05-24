using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EventBusses;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace UI
{
    public class WeaponChanging : MonoBehaviour
    {
        [SerializeField] private Transform topPoint;
        [SerializeField] private Transform middlePoint;
        [SerializeField] private Transform spawnPoint;
        private List<SpriteRenderer> _renderers;
        private IEventBus _eventBus;
        private int _outlineColorId;
        private void Awake()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
            _outlineColorId = Shader.PropertyToID("_OuterOutlineColor");
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnWeaponUpgraded>(ChangeWeaponSpriteWithAnim);   
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnWeaponUpgraded>(ChangeWeaponSpriteWithAnim);
        }

        private void ChangeWeaponSpriteWithAnim(OnWeaponUpgraded eventData)
        {
            var emptySpriteRenderer = _renderers.Find(i => i.sprite == null);
            var fullSpriteRenderer = _renderers.Find(i => i.sprite != null);
            emptySpriteRenderer.transform.position = spawnPoint.position;
            emptySpriteRenderer.sprite = eventData.ObjectUIIdentifierSo.ObjectSprite;
            emptySpriteRenderer.material.SetColor(_outlineColorId, eventData.Stage.OutlineColor);
            
            CreateObject(fullSpriteRenderer, emptySpriteRenderer);
        }

        private void CreateObject(SpriteRenderer fullSpriteRenderer, SpriteRenderer emptySpriteRenderer)
        {
            var tween = DOTween.Sequence();
            tween.SetId(GetHashCode());
            tween.OnKill(() =>
            {
                //todo eğer butona birden fazla basılırsa olacak aksiyon gerçekleşsin.
            });
            tween.Append(fullSpriteRenderer.transform.DOMove(topPoint.position, 0.5f));
            tween.Join(emptySpriteRenderer.transform.DOMove(middlePoint.position, 0.5f));
            tween.OnComplete(() =>
            {
                fullSpriteRenderer.sprite = null;
            });
        }
    }
}
