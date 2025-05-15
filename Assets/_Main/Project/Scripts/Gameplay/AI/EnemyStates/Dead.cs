using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Dead : IState
    {
        private readonly EnemyAnimationController _animationController;

        public Dead(EnemyAnimationController animationController)
        {
            _animationController = animationController;
        }

        public void Tick()
        {
            Debug.Log("Dead");

        }

        public void OnEnter()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}