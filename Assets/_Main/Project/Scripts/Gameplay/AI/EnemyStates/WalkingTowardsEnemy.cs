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
            //Debug.Log(_aiPath.remainingDistance);
            Vector3 velocity = _aiPath.desiredVelocity;

            if (velocity.x > 0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 0, 0); // Sağa bak
            }
            else if (velocity.x < -0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 180, 0); // Sola bak
            }
            
            Debug.Log("Walking");
        }

        public void OnEnter()
        {
            var enemy = _characterCombatManager.LastFoundEnemy;
            if (enemy == null) return;

            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;
            var enemyPosition = enemy.transform.position;

            float currentDistance = Vector3.Distance(selfPosition, enemyPosition);

            if (currentDistance > minimumRange + 0.1f) // Ufak tolerans payı ekliyoruz
            {
                // Yeterince yakın değilse: doğru pozisyona yürü
                var direction = (enemyPosition - selfPosition).normalized;
                var targetPosition = enemyPosition - direction * minimumRange;
                _aiPath.destination = targetPosition;
                _aiPath.canMove = true;
            }
            else
            {
                // Zaten yeterince yakınsa: hiç hareket etme
                _aiPath.canMove = false;
                _aiPath.destination = selfPosition; // Hedef kendin olsun, yerinde kal
            }

            _animationController.Run();
            _aiPath.maxSpeed = _speedPropertyData.TemporaryValue;
        }


        public void OnExit()
        {
            _aiPath.canMove = false;
        }
    }
}