using Cysharp.Threading.Tasks;
using Events.ClickableEvents;
using UnityEngine;

namespace UI.PerksAndDraggables.PerkManagers
{
    public class MiddlePerkManager : PerkManager
    {
        private void Start()
        {
            //debug purpose
            foreach (var clickable in FindObjectsByType<Draggable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                //PlaceClickableToAppropriatePosition(clickable);
            }
        }
        
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
        }

        public void RemoveClickableFromList(Clickable clickable)
        {
            var elementToRemove = ClickableAndConnectedTransforms.Find(i => i.Clickable == clickable);
            ClickableAndConnectedTransforms.Remove(elementToRemove);
            Destroy(elementToRemove.ParentTransform.gameObject);
        }
    }
}