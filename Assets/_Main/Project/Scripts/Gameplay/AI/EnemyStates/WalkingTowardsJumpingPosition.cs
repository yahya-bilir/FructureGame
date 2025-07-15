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
        public float Timer { get; private set; }

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
            Timer += Time.deltaTime;
        }

        public void OnEnter()
        {
            _enemyMovementController.ToggleRVO(false);
            var dest = _characterIslandController.GetJumpingPosition();
            _enemyMovementController.MoveCharacter(dest, false, 3);
            _characterIslandController.StartWalkingToJumpingPosition();
            //var dest = (Vector2) _modelTransform.position +  new Vector2(0, 4.5f);
        }
        public void OnExit()
        {
            _characterIslandController.StopWalkingToJumpingPosition();
            Debug.Log("exited");
            Timer = 0f;
        }
        
    }
}