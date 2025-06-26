using Cysharp.Threading.Tasks;
using DG.Tweening;
using EventBusses;
using Events.ClickableEvents;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace UI.PerksAndDraggables
{
    public abstract class Draggable : Clickable, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        [SerializeField] private GameObject draggableCircle;
        [SerializeField] private GameObject cardPart;
        private RectTransform _bottomHalf;
        private Transform _connectedTransform;
        protected IEventBus EventBus;

        //todo sonradan false'a çek
        private bool _isDraggingEnabled = true;
        protected override void Start()
        {
            base.Start();
            cardPart.SetActive(true);
            draggableCircle.SetActive(false);
        }
        
        [Inject]
        private void Inject(RectTransform bottomHalf, IEventBus eventBus)
        {
            _bottomHalf = bottomHalf;
            EventBus = eventBus;
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!_isDraggingEnabled) return;
            var seq = DOTween.Sequence();
            seq.SetId(GetHashCode());
            seq.Append(cardPart.transform.DOScale(cardPart.transform.localScale * 1.1f, 0.15f));
            EventBus.Publish(new OnDraggableStartedBeingDragged(this));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!_isDraggingEnabled) return;
            transform.position = eventData.position;
            var isOnBottomHalf = CheckIfInBottomHalf(eventData.position);
            cardPart.SetActive(isOnBottomHalf);
            draggableCircle.SetActive(!isOnBottomHalf);
        }


        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if(!_isDraggingEnabled) return;

            if (!CheckIfInBottomHalf(eventData.position))
            {
                Vector3 screenPos = eventData.position;
                screenPos.z = 10f;

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldPos.z = 0f; 
                OnDragEndedOnScene(worldPos);
                return;
            }
            
            OnDragEndedOnBottomHalf();
        }

        private void OnDragEndedOnBottomHalf()
        {
            cardPart.transform.DOScale(Vector3.one, 0.15f);
            EventBus.Publish(new OnDraggableStoppedBeingDragged(this));
        }

        private bool CheckIfInBottomHalf(Vector2 pos)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_bottomHalf, pos, null);
        }

        protected virtual void OnDragEndedOnScene(Vector2 worldPos)
        {
            clickableActionSo.OnDragEndedOnScene(worldPos);
            
            EventBus.Publish(new OnClickableDestroyed(this));
            
            Destroy(gameObject);
        }

        public void SetConnectedTransform(Transform trf) => _connectedTransform = trf;

        public async UniTask SendDraggableToConnectedTransform()
        {
            _isDraggingEnabled = false;
            await UniTask.WaitForSeconds(0.1f);
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(_connectedTransform.position, 0.25f).OnComplete(() => _isDraggingEnabled = true));
        }
    }
}