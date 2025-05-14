using UnityEngine;
using UnityEngine.Events;

namespace CommonComponents
{
    public class OnUIClickedOperations : MonoBehaviour
    {
        [SerializeField] private UnityEvent actionToTrigger;
        
        private void OnMouseDown()
        {
            actionToTrigger?.Invoke();
        }
    }
}