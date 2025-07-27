using System.Collections.Generic;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyRagdollManager
    {
        private readonly GameObject _model;
        private readonly CharacterAnimationController _animationController;
        private List<Rigidbody> _ragdollRigidbodies;
        private List<Collider> _ragdollColliders;

        public EnemyRagdollManager(GameObject model, CharacterAnimationController animationController)
        {
            _model = model;
            _animationController = animationController;
            _ragdollRigidbodies = new List<Rigidbody>();
            _ragdollColliders = new List<Collider>();
        }

        public void Initialize()
        {
            _ragdollRigidbodies = new List<Rigidbody>(_model.GetComponentsInChildren<Rigidbody>());
            _ragdollColliders = new List<Collider>(_model.GetComponentsInChildren<Collider>());

            foreach (var rb in _ragdollRigidbodies)
            {
                rb.isKinematic = true;
            }

            foreach (var col in _ragdollColliders)
            {
                col.enabled = false;
            }
        }

        public void ActivateRagdoll()
        {
            _animationController.DisableAnimator();

            foreach (var rb in _ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }

            foreach (var col in _ragdollColliders)
            {
                col.enabled = true;
            }
        }
    }
}