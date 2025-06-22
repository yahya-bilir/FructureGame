using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.PerksAndDraggables
{
    public abstract class Clickable : MonoBehaviour, IPointerClickHandler
    {
        protected bool IsClickable;
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsClickable) return;
        }
        
        public abstract void OnClickedSuccessfully();
    }
}