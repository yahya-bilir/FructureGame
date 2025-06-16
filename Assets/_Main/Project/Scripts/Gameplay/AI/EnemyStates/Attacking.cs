using AI.Base.Interfaces;
using Characters;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Attacking : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly float _attackingInterval;
        private readonly CharacterCombatManager _characterCombatManager;
        private float _attackingTimer;

        public Attacking(CharacterAnimationController animationController, float attackingInterval)
        {
            _animationController = animationController;
            _attackingInterval = attackingInterval;
        }

        public void Tick()
        {
            //Debug.Log("Attacking");
            
            if (_attackingTimer < _attackingInterval)
            {
                _attackingTimer += Time.deltaTime;  
                return;
            }

            Debug.Log("Attacking");
            _attackingTimer = 0f;
            _animationController.Attack();

        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}