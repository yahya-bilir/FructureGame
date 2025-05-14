using System.Collections;
using Interfaces;
using UnityEngine;

namespace CommonComponents
{
    public class BasicInterractor : MonoBehaviour
    {
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactions = other.GetComponentsInChildren<IInteractionReceiver>();
            foreach (var interaction in interactions)
            {
                interaction.Interact(true, _collider);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var interactions = other.GetComponentsInChildren<IInteractionReceiverInUpdate>();
            foreach (var interaction in interactions)
            {
                interaction.InteractInUpdate(_collider);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactions = other.GetComponentsInChildren<IInteractionReceiver>();
            foreach (var interaction in interactions)
            {
                interaction.Interact(false, _collider);
                StopAllCoroutines();
            }
        }
    }
}