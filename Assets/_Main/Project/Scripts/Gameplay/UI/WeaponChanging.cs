using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EventBusses;
using Events;
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
            
            if (emptySpriteRenderer == null || fullSpriteRenderer == null)
            {
                Debug.Log("SpriteRenderer eşleşmesinde problem: null referans.");
                emptySpriteRenderer = _renderers[0];
                fullSpriteRenderer = _renderers[1];

                foreach (var rnd in _renderers)
                {
                    rnd.sprite = null;
                }
            }
            
            emptySpriteRenderer.transform.position = spawnPoint.position;
            emptySpriteRenderer.sprite = eventData.ObjectUIIdentifierSo.ObjectSprite;
            emptySpriteRenderer.material.SetColor(_outlineColorId, eventData.Stage.OutlineColor);
            
            CreateObject(fullSpriteRenderer, emptySpriteRenderer);
        }

        private void CreateObject(SpriteRenderer fullSpriteRenderer, SpriteRenderer emptySpriteRenderer)
        {
            DOTween.Kill(GetHashCode());
            
            var tween = DOTween.Sequence();
            tween.SetId(GetHashCode());
            tween.SetAutoKill(false);
            tween.OnKill(() =>
            {
                emptySpriteRenderer.transform.position = middlePoint.position;
                fullSpriteRenderer.transform.position = spawnPoint.position;
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
