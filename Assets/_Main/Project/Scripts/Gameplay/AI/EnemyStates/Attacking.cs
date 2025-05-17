using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using Characters.Player;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Attacking : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly float _attackingInterval;
        private readonly CharacterCombatManager _playerCombatManager;
        private readonly float _attackDamage;
        private float _attackingTimer;

        public Attacking(CharacterAnimationController animationController, float attackingInterval,
            CharacterCombatManager playerCombatManager, float attackDamage)
        {
            _animationController = animationController;
            _attackingInterval = attackingInterval;
            _playerCombatManager = playerCombatManager;
            _attackDamage = attackDamage;
        }

        public void Tick()
        {
            Debug.Log("Attacking");
            
            if (_attackingTimer < _attackingInterval)
            {
                _attackingTimer += Time.deltaTime;  
                return;
            }

            _attackingTimer = 0f;
            _animationController.Attack();
            _playerCombatManager.GetDamage(_attackDamage);
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}