using Cysharp.Threading.Tasks;
using Events.ClickableEvents;
using UnityEngine;

namespace UI.PerksAndDraggables.PerkManagers
{
    public class MiddlePerkManager : PerkManager
    {
        private void OnEnable()
        {
            EventBus.Subscribe<OnClickableCreated>(OnClickableCreated);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnClickableCreated>(OnClickableCreated);
        }

        private void OnClickableCreated(OnClickableCreated eventData)
        {
            PlaceClickableToAppropriatePosition(eventData.Clickable);
        }

        private void PlaceClickableToAppropriatePosition(Clickable clickable)
        {
            var newHolder = Instantiate(holder, transform);
            var draggable = clickable as Draggable;

            var clickableTransformPair = new ClickableAndConnectedTransform(newHolder.transform, clickable);
            ClickableAndConnectedTransforms.Add(clickableTransformPair);

            draggable.SetConnectedTransform(newHolder.transform);
            draggable.SendDraggableToConnectedTransform().Forget();
            
            foreach (var clickableAndConnectedTransform in ClickableAndConnectedTransforms)
            {
                if(clickableAndConnectedTransform.Clickable == draggable) continue;
                var drg = clickableAndConnectedTransform.Clickable as Draggable;
                drg.SendDraggableToConnectedTransform().Forget();
            }
        }

        public void RemoveClickableFromList(Clickable clickable)
        {
            var elementToRemove = ClickableAndConnectedTransforms.Find(i => i.Clickable == clickable);
            ClickableAndConnectedTransforms.Remove(elementToRemove);
            Destroy(elementToRemove.ParentTransform.gameObject);
        }
    }
}