using AI.Base.Interfaces;
using Characters;
using Pathfinding;
using PropertySystem;
using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace AI.EnemyStates
{
    public class WalkingTowardsEnemy : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly AIPath _aiPath;
        private readonly Transform _modelTransform;
        private readonly PropertyData _speedPropertyData;
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly CharacterDataHolder _characterDataHolder;
        private readonly Collider2D _collider;
        private readonly AIDestinationSetter _aiDestinationSetter;
        private readonly Rigidbody2D _rigidbody2D;

        public WalkingTowardsEnemy(CharacterAnimationController animationController,
            AIPath aiPath, Transform model, PropertyData speedPropertyData,
            CharacterCombatManager characterCombatManager, CharacterDataHolder characterDataHolder, Collider2D collider,
            AIDestinationSetter aiDestinationSetter, Rigidbody2D rigidbody2D)
        {
            _animationController = animationController;
            _aiPath = aiPath;
            _modelTransform = model;
            _speedPropertyData = speedPropertyData;
            _characterCombatManager = characterCombatManager;
            _characterDataHolder = characterDataHolder;
            _collider = collider;
            _aiDestinationSetter = aiDestinationSetter;
            _rigidbody2D = rigidbody2D;
        }

        public void Tick()
        {
            var enemy = _characterCombatManager.LastFoundEnemy;
            if (enemy == null || enemy.IsCharacterDead)
            {
                Debug.Log("Enemy is null or dead. Stopping movement.");
                _aiDestinationSetter.target = null;
                _aiPath.canMove = false;
                return;
            }
            
            Debug.Log("Walking towards enemy| Remaining Distance: " + _aiPath.remainingDistance);
            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;
            var enemyPosition = enemy.transform.position;

            float currentDistance = Vector3.Distance(selfPosition, enemyPosition);

            if (currentDistance > minimumRange + 0.1f)
            {
                // var direction = (enemyPosition - selfPosition).normalized;
                // var targetPosition = enemyPosition - direction * minimumRange;
                _aiDestinationSetter.target = enemy.transform;
                _aiPath.canMove = true;
                //_aiPath.destination = enemyPosition;
                //_aiPath.canMove = true;
            }
            else
            {
                _aiDestinationSetter.target = null;
                _aiPath.destination = selfPosition;
                _aiPath.canMove = false;
            }

            Vector3 velocity = _aiPath.desiredVelocity;
            if (velocity.x > 0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (velocity.x < -0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }


        public void OnEnter()
        {
            _animationController.Run();
            _aiPath.maxSpeed = _speedPropertyData.TemporaryValue;
            _collider.isTrigger = true;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }


        public void OnExit()
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;

            //_aiPath.canMove = false;
        }
    }
}