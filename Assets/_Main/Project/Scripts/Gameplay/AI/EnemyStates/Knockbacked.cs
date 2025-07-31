using AI.Base.Interfaces;
using Characters;
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
        private readonly CharacterAnimationController _animationController;
        private readonly CharacterCombatManager _characterCombatManager;
        public float KnockbackTimer { get; private set; }
        public Knockbacked(TextMeshPro aiText, EnemyMovementController movementController, Rigidbody rigidbody,
            EnemyBehaviour enemyBehaviour, CharacterAnimationController animationController,
            CharacterCombatManager characterCombatManager)
        {
            _aiText = aiText;
            _movementController = movementController;
            _rigidbody = rigidbody;
            _enemyBehaviour = enemyBehaviour;
            _animationController = animationController;
            _characterCombatManager = characterCombatManager;
        }

        public void OnEnter()
        {
            _aiText.text = "Knockbacked";
            _movementController.StopCharacter(true); // physics on
            //_movementController.SetSpeedToZero();
            _animationController.GetHit();
            KnockbackTimer = 0f;
            _characterCombatManager.GetDamage(1f);
        }

        public void Tick()
        {
            KnockbackTimer += Time.deltaTime;
        }

        public void OnExit()
        {
            _rigidbody.isKinematic = true;
            _enemyBehaviour.SetKnockbacked(false);
        }
    }
}