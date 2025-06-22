using Characters;
using EventBusses;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace UI.PerksAndDraggables
{
    public class DragDropCircleHandler : MonoBehaviour, IEndDragHandler, IDragHandler
    {
        [SerializeField] private float radius = 5f;
        [SerializeField] private Camera cam;
        private IEventBus _eventBus;
        private Image _image;
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
        
        private void Awake()
        {
            cam = Camera.main;
            _image = GetComponent<Image>();

            _image.enabled = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;

        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            Vector3 screenPos = eventData.position;
            screenPos.z = 10f;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f; 
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius);
            foreach (var hit in hits)
            {
                var character = hit.GetComponent<Character>();
                if (character != null)
                {
                    Debug.Log($"Character found: {character.name}");
                }
            }
        }
    }
}