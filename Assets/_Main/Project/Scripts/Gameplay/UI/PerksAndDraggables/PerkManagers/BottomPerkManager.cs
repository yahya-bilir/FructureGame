using Cysharp.Threading.Tasks;
using Events.ClickableEvents;
using Sirenix.OdinInspector;

namespace UI.PerksAndDraggables.PerkManagers
{
    public class BottomPerkManager : PerkManager
    {
        private void OnEnable()
        {
            EventBus.Subscribe<OnDraggableStartedBeingDragged>(OnDraggableStartedBeingDragged);
            EventBus.Subscribe<OnDraggableStoppedBeingDragged>(OnDraggableStoppedBeingDragged);
            EventBus.Subscribe<OnClickableDestroyed>(OnClickableDestroyed);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnDraggableStartedBeingDragged>(OnDraggableStartedBeingDragged);
            EventBus.Unsubscribe<OnDraggableStoppedBeingDragged>(OnDraggableStoppedBeingDragged);
            EventBus.Unsubscribe<OnClickableDestroyed>(OnClickableDestroyed);
        }

        private void OnDraggableStartedBeingDragged(OnDraggableStartedBeingDragged eventData)
        {
            var draggable = eventData.Draggable;
            foreach (var connectedTransform in ClickableAndConnectedTransforms)
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
            foreach (var connectedTransform in ClickableAndConnectedTransforms)
            {
                connectedTransform.ParentTransform.SetParent(transform);
                Draggable drg = connectedTransform.Clickable as Draggable;
                if (drg != null) drg.SendDraggableToConnectedTransform().Forget();
            }
        }

        private void OnClickableDestroyed(OnClickableDestroyed eventData)
        {
            if(eventData.Clickable is not Draggable) return;
            ClickableAndConnectedTransforms.Remove(
                ClickableAndConnectedTransforms.Find(i => i.Clickable == eventData.Clickable));
        }

        [Button(ButtonSizes.Medium)]
        public void TransferDraggableToBottom(Draggable draggable)
        {
            // 1. Yeni holder objesini Instantiate et
            var newHolder = Instantiate(holder, transform);

            // 2. Draggable'ın parent'ını yeni holder olarak ayarla
            //draggable.transform.SetParent(newHolder.transform);

            // 3. ConnectedTransform olarak set et
            draggable.SetConnectedTransform(newHolder.transform);

            // 4. Draggable'ı bağlı transformuna gönder
            draggable.SendDraggableToConnectedTransform().Forget();

            // 5. Listeye ekle
            var clickableTransformPair = new ClickableAndConnectedTransform(newHolder.transform, draggable);
            ClickableAndConnectedTransforms.Add(clickableTransformPair);

            foreach (var clickableAndConnectedTransform in ClickableAndConnectedTransforms)
            {
                if(clickableAndConnectedTransform.Clickable == draggable) continue;
                var drg = clickableAndConnectedTransform.Clickable as Draggable;
                drg.SendDraggableToConnectedTransform().Forget();
            }
        }
    }
}