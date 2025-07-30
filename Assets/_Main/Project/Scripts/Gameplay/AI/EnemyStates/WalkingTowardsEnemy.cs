using AI.Base.Interfaces;
using Characters;
using Characters.BaseSystem;
using Characters.Enemy;
using TMPro;
using UnityEngine;
using WeaponSystem.MeleeWeapons;

namespace AI.EnemyStates
{
    public class WalkingTowardsEnemy : IState
    {
        private readonly MainBase _mainBase;
        private readonly CharacterDataHolder _characterDataHolder;
        private readonly EnemyMovementController _enemyMovementController;
        private readonly Transform _modelTransform;
        private readonly TextMeshPro _aiText;
        private Character _enemy;
        public WalkingTowardsEnemy(MainBase mainBase,
            CharacterDataHolder characterDataHolder, EnemyMovementController enemyMovementController,
            Transform modelTransform, TextMeshPro aiText)
        {
            _mainBase = mainBase;
            _characterDataHolder = characterDataHolder;
            _enemyMovementController = enemyMovementController;
            _modelTransform = modelTransform;
            _aiText = aiText;
        }

        public void Tick()
        {
            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;
            var enemyPosition = _enemy.transform.position;

            float currentDistance = Vector3.Distance(selfPosition, enemyPosition);

            if (currentDistance > minimumRange + 0.1f)
            {
                _enemyMovementController.MoveCharacter(_enemy.transform.position, false, 0f);
                _enemyMovementController.IncreaseSpeedSmoothly(2f);
            }
            else
            {
                _enemyMovementController.StopCharacter(false);
            }
        }


        public void OnEnter()
        {
            _aiText.text = "Walking Towards Enemy";
            _enemy = _mainBase;
            //_enemyMovementController.MoveCharacter(_enemy.transform.position, true, 1);

        }


        public void OnExit()
        {
        }
    }
}