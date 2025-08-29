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
        private readonly CharacterMovementController characterMovementController;
        private readonly Transform _modelTransform;
        private readonly TextMeshPro _aiText;
        private Character _enemy;
        public WalkingTowardsEnemy(MainBase mainBase,
            CharacterDataHolder characterDataHolder, CharacterMovementController characterMovementController,
            Transform modelTransform, TextMeshPro aiText)
        {
            _mainBase = mainBase;
            _characterDataHolder = characterDataHolder;
            this.characterMovementController = characterMovementController;
            _modelTransform = modelTransform;
            _aiText = aiText;
        }

        public void Tick()
        {
            var castedWeapon = (WeaponSO)_characterDataHolder.Weapon.ObjectUIIdentifierSo;
            float minimumRange = castedWeapon.MinimumRange;

            var selfPosition = _modelTransform.position;

            // MainBase içerisindeki Collider'dan en yakın noktayı bul
            Vector3 targetPosition = _mainBase.Collider.ClosestPoint(selfPosition);

            float currentDistance = Vector3.Distance(selfPosition, targetPosition);

            if (currentDistance > minimumRange + 0.1f)
            {
                characterMovementController.MoveCharacter(targetPosition, false, 0f);
                characterMovementController.IncreaseSpeedSmoothly(2f);
            }
            else
            {
                characterMovementController.StopCharacter(false);
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