using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Dead : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly Collider2D _collider2D;
        private readonly AIPath _aiPath;
        private readonly AIDestinationSetter _aiDestinationSetter;
        private readonly RVOController _rvoController;
        private float _deathTimer;

        public Dead(CharacterAnimationController animationController, Collider2D collider2D, AIPath aiPath,
            AIDestinationSetter aiDestinationSetter, RVOController rvoController)
        {
            _animationController = animationController;
            _collider2D = collider2D;
            _aiPath = aiPath;
            _aiDestinationSetter = aiDestinationSetter;
            _rvoController = rvoController;
        }

        public void Tick()
        {
            _deathTimer += Time.deltaTime;
            if (_deathTimer >= 1f)
            {
                GameObject.Destroy(_collider2D.gameObject);
            }
        }

        public void OnEnter()
        {
            _collider2D.enabled = false;
            _animationController.DisableAnimator();
            _aiPath.radius = 0f;
        }

        public void OnExit()
        {
            
        }
    }
}