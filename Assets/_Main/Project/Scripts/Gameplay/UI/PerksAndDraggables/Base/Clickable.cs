using DG.Tweening;
using EventBusses;
using Events.ClickableEvents;
using Events.IslandEvents;
using Perks;
using Perks.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace UI.PerksAndDraggables
{
    public abstract class Clickable : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("draggableActionSo")] [SerializeField] protected ClickableActionSo clickableActionSo;
        protected IObjectResolver Resolver;
        protected bool IsClickable;
        protected IEventBus EventBus;

        [Header("UI")]
        [SerializeField] private Image mainImageHolder;
        [SerializeField] private TextMeshProUGUI titleTextHolder;
        [SerializeField] private TextMeshProUGUI descHolder;
        private ClickableUIManager _clickableUIManager;


        [Inject]
        private void Inject(IObjectResolver resolver, IEventBus eventBus)
        {
            Resolver = resolver;
            EventBus = eventBus;

        }

        private void Awake()
        {
            _clickableUIManager = new ClickableUIManager(clickableActionSo.ClickableActionInfo, mainImageHolder, titleTextHolder, descHolder);
        }


        protected virtual void Start()
        {
            Resolver.Inject(clickableActionSo); 
            _clickableUIManager.Initialize();
            //EventBus.Publish(new OnClickableCreated(this));
            
            var localScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(localScale, 1f).SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                IsClickable = true;
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsClickable) return;
            IsClickable = false;
            OnClickedSuccessfully();
        }

        protected virtual void OnClickedSuccessfully()
        {
            EventBus.Publish(new OnClickableClicked(this));
        }
    }
}