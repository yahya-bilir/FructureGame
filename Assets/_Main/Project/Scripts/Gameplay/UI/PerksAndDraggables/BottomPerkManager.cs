using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventBusses;
using Events.ClickableEvents;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace UI.PerksAndDraggables
{
    public class BottomPerkManager : MonoBehaviour
    {
        [SerializeField] private GameObject holder;
        
        private IEventBus _eventBus;
        private List<ClickableAndConnectedTransform> _clickableAndConnectedTransforms = new();
        

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Start()
        {
            //debug purpose
            foreach (var drg in FindObjectsByType<Draggable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                TransferDraggableToBottom(drg);
            }
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<OnDraggableStartedBeingDragged>(OnDraggableStartedBeingDragged);
            _eventBus.Subscribe<OnDraggableStoppedBeingDragged>(OnDraggableStoppedBeingDragged);
            _eventBus.Subscribe<OnClickableDestroyed>(OnClickableDestroyed);
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnDraggableStartedBeingDragged>(OnDraggableStartedBeingDragged);
            _eventBus.Unsubscribe<OnDraggableStoppedBeingDragged>(OnDraggableStoppedBeingDragged);
            _eventBus.Unsubscribe<OnClickableDestroyed>(OnClickableDestroyed);
        }

        private void OnDraggableStartedBeingDragged(OnDraggableStartedBeingDragged eventData)
        {
            var draggable = eventData.Draggable;
            foreach (var connectedTransform in _clickableAndConnectedTransforms)
            {
                if (connectedTransform.Clickable == draggable)
                {
                    connectedTransform.ParentTransform.SetParent(transform.parent);
                    continue;
                }
                
                Draggable drg = connectedTransform.Clickable as Draggable;
                if (drg != null) drg.SendDraggableToConnectedTransform().Forget();
            }
            
        }        
        private void OnDraggableStoppedBeingDragged(OnDraggableStoppedBeingDragged eventData)
        {
            foreach (var connectedTransform in _clickableAndConnectedTransforms)
            {
                connectedTransform.ParentTransform.SetParent(transform);
                Draggable drg = connectedTransform.Clickable as Draggable;
                if (drg != null) drg.SendDraggableToConnectedTransform().Forget();
            }
        }

        private void OnClickableDestroyed(OnClickableDestroyed eventData)
        {
            if(eventData.Clickable is not Draggable) return;
            _clickableAndConnectedTransforms.Remove(
                _clickableAndConnectedTransforms.Find(i => i.Clickable == eventData.Clickable));
        }

        [Button(ButtonSizes.Medium)]
        public void TransferDraggableToBottom(Draggable draggable)
        {
            // 1. Yeni holder objesini Instantiate et
            GameObject newHolder = Instantiate(holder, transform);

            // 2. Draggable'ın parent'ını yeni holder olarak ayarla
            //draggable.transform.SetParent(newHolder.transform);

            // 3. ConnectedTransform olarak set et
            draggable.SetConnectedTransform(newHolder.transform);

            // 4. Draggable'ı bağlı transformuna gönder
            draggable.SendDraggableToConnectedTransform().Forget();

            // 5. Listeye ekle
            var clickableTransformPair = new ClickableAndConnectedTransform(newHolder.transform, draggable);
            _clickableAndConnectedTransforms.Add(clickableTransformPair);
        }
    }
}