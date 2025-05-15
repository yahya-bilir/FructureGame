using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Attacking : IState
    {
        private readonly EnemyAnimationController _animationController;

        public Attacking(EnemyAnimationController animationController)
        {
            _animationController = animationController;
        }

        public void Tick()
        {
            Debug.Log("Attacking");

        }

        public void OnEnter()
        {
            _animationController.Attack();
        }

        public void OnExit()
        {
        }
    }
}