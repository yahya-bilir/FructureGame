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

        public WalkingTowardsEnemy(CharacterAnimationController animationController,
            AIPath aiPath, Transform model, PropertyData speedPropertyData,
            CharacterCombatManager characterCombatManager, CharacterDataHolder characterDataHolder)
        {
            _animationController = animationController;
            _aiPath = aiPath;
            _modelTransform = model;
            _speedPropertyData = speedPropertyData;
            _characterCombatManager = characterCombatManager;
            _characterDataHolder = characterDataHolder;
        }

        public void Tick()
        {
            var enemy = _characterCombatManager.LastFoundEnemy;
            if (enemy == null)
            {
                _aiPath.canMove = false;
                return;
            }
            Debug.Log("Walking towards enemy");

            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;
            var enemyPosition = enemy.transform.position;

            float currentDistance = Vector3.Distance(selfPosition, enemyPosition);

            if (currentDistance > minimumRange + 0.1f)
            {
                var direction = (enemyPosition - selfPosition).normalized;
                var targetPosition = enemyPosition - direction * minimumRange;
                _aiPath.destination = targetPosition;
                _aiPath.canMove = true;
            }
            else
            {
                _aiPath.canMove = false;
                _aiPath.destination = selfPosition;
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
        }


        public void OnExit()
        {
            _aiPath.canMove = false;
        }
    }
}