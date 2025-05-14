using UnityEngine;

namespace Interfaces
{
    public interface IInteractionReceiver
    {
        public void Interact(bool isEntered, Collider receivedCollider);
    }
}