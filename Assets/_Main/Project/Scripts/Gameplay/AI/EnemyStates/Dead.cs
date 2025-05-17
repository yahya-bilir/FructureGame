using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Dead : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly Collider2D _collider2D;

        public Dead(CharacterAnimationController animationController, Collider2D collider2D)
        {
            _animationController = animationController;
            _collider2D = collider2D;
        }

        public void Tick()
        {
            Debug.Log("Dead");

        }

        public void OnEnter()
        {
            _collider2D.enabled = false;
        }

        public void OnExit()
        {
            
        }
    }
}