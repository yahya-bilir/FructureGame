using Perks.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using VContainer;

namespace UI.PerksAndDraggables
{
    public abstract class Clickable : MonoBehaviour, IPointerClickHandler
    {
        [FormerlySerializedAs("draggableActionSo")] [SerializeField] protected ClickableActionSo clickableActionSo;
        protected IObjectResolver Resolver;
        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        protected virtual void Start()
        {
            Resolver.Inject(clickableActionSo);
        }

        protected bool IsClickable;
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsClickable) return;
            OnClickedSuccessfully();
        }
        
        public abstract void OnClickedSuccessfully();
    }
}