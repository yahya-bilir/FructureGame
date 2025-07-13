using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class WalkingTowardsJumpingPosition : IState
    {
        private readonly CharacterIslandController _characterIslandController;
        private readonly EnemyMovementController _enemyMovementController;
        private readonly Transform _modelTransform;

        public WalkingTowardsJumpingPosition(CharacterIslandController characterIslandController,
            EnemyMovementController enemyMovementController, Transform modelTransform)
        {
            _characterIslandController = characterIslandController;
            _enemyMovementController = enemyMovementController;
            _modelTransform = modelTransform;
        }


        public void Tick()
        {
            Debug.Log("Walking Towards Jumping Position");
        }

        public void OnEnter()
        {
            _enemyMovementController.ToggleRVO(false);
            _characterIslandController.StartWalkingToJumpingPosition();
            //var dest = (Vector2) _modelTransform.position +  new Vector2(0, 4.5f);
            var dest = _characterIslandController.GetJumpingPosition();
            _enemyMovementController.MoveCharacter(dest, false);
            
        }
        public void OnExit()
        {
            _characterIslandController.StopWalkingToJumpingPosition();
        }
        
    }
}