using Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace CommonComponents
{
    public class BasicInteractionActions : MonoBehaviour, IInteractionReceiver
    {
        [SerializeField] private UnityEvent onInteractionHappened;
        [SerializeField] private UnityEvent onInteractionEnded;
        [SerializeField] private string interactionTag;
        
        public void Interact(bool isEntered, Collider receivedCollider)
        {
            if(!receivedCollider.CompareTag(interactionTag)) return;
            if(isEntered) onInteractionHappened?.Invoke();
            else onInteractionEnded?.Invoke();
        }
    }
}