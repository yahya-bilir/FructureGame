using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class JumpingToPosition : IState
    {
        private readonly CharacterIslandController _characterIslandController;
        private readonly EnemyBehaviour _enemyBehaviour;
        private readonly EnemyMovementController _enemyMovementController;
        private Vector2 _jumpTarget;
        private float _timer;

        public JumpingToPosition(CharacterIslandController characterIslandController, EnemyBehaviour enemyBehaviour,
            EnemyMovementController enemyMovementController)
        {
            _characterIslandController = characterIslandController;
            _enemyBehaviour = enemyBehaviour;
            _enemyMovementController = enemyMovementController;
        }

        public void Tick()
        {
            if (_timer <= 0.05f)
            {
                _timer += Time.deltaTime;
                return;
            }

            if (_enemyMovementController.GetIsReachedDistance(3.5f))
            {
                _characterIslandController.SetCanJumpDisabled();
                _characterIslandController.StopJumping();
            }
        }
        public void OnEnter()
        {
            _characterIslandController.StartJumpingActions();
            _jumpTarget = (Vector2) _enemyBehaviour.transform.position + new Vector2(0, 5f);

            _enemyMovementController.MoveCharacter(_jumpTarget, false, 3);
            _enemyMovementController.ToggleRVO(false);
        }

        public void OnExit()
        {
            _timer = 0f;
        }
    }
}