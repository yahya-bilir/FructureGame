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
        private RectTransform _card;
        private Transform _connectedTransform;
        private float _radius;

        private bool _isDraggingEnabled;

        protected override void Start()
        {
            base.Start();
            _isDraggingEnabled = false;
            cardPart.SetActive(true);
            draggableCircle.SetActive(false);
            
            Vector2 sizeDelta = draggableCircle.GetComponent<RectTransform>().sizeDelta;
            float pixelsPerUnit = Screen.height / (Camera.main.orthographicSize * 2);

            float worldWidth = sizeDelta.x / pixelsPerUnit;
            float worldHeight = sizeDelta.y / pixelsPerUnit;
            _radius = Mathf.Sqrt(worldWidth * worldWidth + worldHeight * worldHeight) / 2f;
        }
        
        [Inject]
        private void Inject(RectTransform bottomHalf)
        {
            _bottomHalf = bottomHalf;
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
            if(draggableCircle.activeInHierarchy) clickableActionSo.OnDrag(GetWorldPos(eventData), _radius);
        }


        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if(!_isDraggingEnabled) return;

            if (!CheckIfInBottomHalf(eventData.position))
            {
                var screenPos = GetWorldPos(eventData);
                OnDragEndedOnScene(screenPos);
                return;
            }
            
            OnDragEndedOnBottomHalf();
        }

        private Vector3 GetWorldPos(PointerEventData eventData)
        {
            Vector3 screenPos = eventData.position;
            screenPos.z = 10f;
            
            var worldPos =  Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f;
            return worldPos;
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
            clickableActionSo.OnDragEndedOnScene(worldPos, _radius);
            
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