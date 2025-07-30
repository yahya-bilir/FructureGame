using System;
using System.Collections.Generic;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Characters.Enemy
{
    public class EnemyRigidbodyEffectsController : IDisposable
    {
        private readonly GameObject _model;
        private readonly Rigidbody _mainRigidbody;
        private readonly EnemyBehaviour _enemyBehaviour;
        private IEventBus _eventBus;
        private List<Rigidbody> _ragdollRigidbodies;
        private List<Collider> _ragdollColliders;

        public EnemyRigidbodyEffectsController(GameObject model, Rigidbody mainRigidbody, EnemyBehaviour enemyBehaviour)
        {
            _model = model;
            _mainRigidbody = mainRigidbody;
            _enemyBehaviour = enemyBehaviour;
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnEnemyKnockbacked>(OnKnockbacked);
            _eventBus.Subscribe<OnEnemyCrushed>(OnCrushed);
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
                col.enabled = false;
        }

        private void OnKnockbacked(OnEnemyKnockbacked evt)
        {
            if (evt.KnockbackedEnemy != _enemyBehaviour) return;

            ApplyKnockbackForce(evt.KnockbackDirection, evt.KnockbackData);
            _enemyBehaviour.SetKnockbacked(true);
        }

        private void OnCrushed(OnEnemyCrushed evt)
        {
            if (evt.CrushedEnemy != _enemyBehaviour) return;

            _enemyBehaviour.SetCrushed();
            ActivateRagdollWithExplosion(evt.ImpactPoint, evt.RagdollData);
        }

        private void ApplyKnockbackForce(Vector3 direction, KnockbackDataHolder data)
        {
            Vector3 force = direction.normalized * data.Force;
            force += Vector3.up * data.UpwardModifier;

            _mainRigidbody.isKinematic = false;
            _mainRigidbody.AddForce(force, ForceMode.Impulse);
        }

        private void ActivateRagdollWithExplosion(Vector3 explosionOrigin, RagdollDataHolder data)
        {
            foreach (var rb in _ragdollRigidbodies)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(data.Force, explosionOrigin, data.Radius, data.UpwardsModifier, ForceMode.Impulse);
            }
            foreach (var col in _ragdollColliders)
                col.enabled = true;
            
            //_mainRigidbody.isKinematic = false;
        }

        public void Dispose()
        {
            _eventBus?.Unsubscribe<OnEnemyKnockbacked>(OnKnockbacked);
            _eventBus?.Unsubscribe<OnEnemyCrushed>(OnCrushed);
        }
    }
}
