using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using Pathfinding;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Fleeing : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly AIPath _aiPath;
        private readonly CharacterSpeedController _characterSpeedController;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly AIDestinationSetter _aiDestinationSetter;

        private bool _hasReachedFleePosition;
        private float _fleeDistanceThreshold = 0.5f;
        private float _randomFleeRadius = 3f;
        private Transform _transform;

        public Fleeing(CharacterAnimationController animationController, AIPath aiPath,
            CharacterSpeedController characterSpeedController, CharacterCombatManager characterCombatManager,
            AIDestinationSetter aiDestinationSetter, Transform transform)
        {
            _animationController = animationController;
            _aiPath = aiPath;
            _characterSpeedController = characterSpeedController;
            _characterCombatManager = characterCombatManager;
            _aiDestinationSetter = aiDestinationSetter;
            _transform = transform;
        }

        public void Tick()
        {
            if (!_hasReachedFleePosition)
            {
                _aiPath.destination = _characterCombatManager.FleePosition;

                float distance = Vector3.Distance(_transform.position, _characterCombatManager.FleePosition);
                if (distance < _fleeDistanceThreshold)
                {
                    _hasReachedFleePosition = true;
                }
            }
            else
            {
                if (_aiPath.reachedEndOfPath)
                {
                    Vector2 randomOffset = Random.insideUnitCircle * _randomFleeRadius;
                    Vector3 randomTarget = _transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                    _aiPath.destination = randomTarget;
                }
            }
        }

        public void OnEnter()
        {
            _hasReachedFleePosition = false;
            _aiPath.canMove = true;
            _aiPath.isStopped = false;
            _aiDestinationSetter.target = null;
        }

        public void OnExit()
        {
            _hasReachedFleePosition = false;
        }
    }
}
