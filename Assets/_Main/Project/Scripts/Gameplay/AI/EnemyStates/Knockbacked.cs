using AI.Base.Interfaces;
using Characters.Enemy;
using TMPro;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Knockbacked : IState
    {
        private readonly TextMeshPro _aiText;
        private readonly EnemyMovementController _movementController;
        private readonly Rigidbody _rigidbody;
        private readonly EnemyBehaviour _enemyBehaviour;
        public float KnockbackTimer { get; private set; }
        public Knockbacked(TextMeshPro aiText, EnemyMovementController movementController, Rigidbody rigidbody,
            EnemyBehaviour enemyBehaviour)
        {
            _aiText = aiText;
            _movementController = movementController;
            _rigidbody = rigidbody;
            _enemyBehaviour = enemyBehaviour;
        }

        public void OnEnter()
        {
            _aiText.text = "Knockbacked";
            _movementController.StopCharacter(true); // physics on
        }

        public void Tick()
        {
            KnockbackTimer += Time.deltaTime;
        }

        public void OnExit()
        {
            KnockbackTimer = 0f;
            _rigidbody.isKinematic = true;
            _enemyBehaviour.SetKnockbacked(false);
        }
    }
}