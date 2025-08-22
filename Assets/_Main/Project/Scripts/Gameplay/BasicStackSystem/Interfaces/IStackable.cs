using UnityEngine;

namespace BasicStackSystem
{
    public interface IStackable
    {
        GameObject GameObject { get; }
        void OnObjectStartedBeingCarried();
        void OnObjectCollected();
        void OnObjectDropped();
    }
}