using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace AI.EnemyStates
{
    public class WalkingTowardsEnemy : IState
    {
        private readonly CharacterCombatManager _characterCombatManager;
        private readonly CharacterDataHolder _characterDataHolder;
        private readonly EnemyMovementController _enemyMovementController;
        private readonly Transform _modelTransform;

        public WalkingTowardsEnemy(CharacterCombatManager characterCombatManager,
            CharacterDataHolder characterDataHolder, EnemyMovementController enemyMovementController,
            Transform modelTransform)
        {
            _characterCombatManager = characterCombatManager;
            _characterDataHolder = characterDataHolder;
            _enemyMovementController = enemyMovementController;
            _modelTransform = modelTransform;
        }

        public void Tick()
        {
            var enemy = _characterCombatManager.LastFoundEnemy;
            if (enemy == null || enemy.IsCharacterDead)
            {
                Debug.Log("Enemy is null or dead. Stopping movement.");
                _enemyMovementController.StopCharacter(true);
                
                return;
            }
            
            //Debug.Log("Walking towards enemy| Remaining Distance: " + _aiPath.remainingDistance);
            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;
            var enemyPosition = enemy.transform.position;

            float currentDistance = Vector3.Distance(selfPosition, enemyPosition);

            if (currentDistance > minimumRange + 0.1f)
            {
                _enemyMovementController.MoveCharacter(enemy.transform, false);
  
            }
            else
            {
                _enemyMovementController.StopCharacter(false);
            }
        }


        public void OnEnter()
        {
            _enemyMovementController.ToggleRVO(true);
        }


        public void OnExit()
        {
            //_rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}