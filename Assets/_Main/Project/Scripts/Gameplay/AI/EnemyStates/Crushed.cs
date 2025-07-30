using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using TMPro;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Crushed : IState
    {
        private readonly Collider _collider;
        private readonly TextMeshPro _aiText;
        private readonly EnemyMovementController _enemyMovementController;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly CharacterAnimationController _animationController;
        private float _deathTimer;

        public Crushed(Collider collider, TextMeshPro aiText, EnemyMovementController enemyMovementController,
            CharacterCombatManager characterCombatManager, CharacterAnimationController animationController)
        {
            _collider = collider;
            _aiText = aiText;
            _enemyMovementController = enemyMovementController;
            _characterCombatManager = characterCombatManager;
            _animationController = animationController;
        }

        public void Tick()
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= 2f)
            {
                _collider.gameObject.SetActive(false);
            }
        }

        public void OnEnter()
        {
            _aiText.text = "Crushed";
            _collider.enabled = false;
            _animationController.DisableAnimator();
            _enemyMovementController.StopCharacter(false);
            _characterCombatManager.GetDamage(500);
        }

        public void OnExit() { }
    }
}