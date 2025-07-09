using AI.Base.Interfaces;
using Characters;
using Characters.Enemy;
using Cysharp.Threading.Tasks;
using Pathfinding;
using PropertySystem;
using UnityEngine;

namespace AI.EnemyStates
{
    public class WalkingTowardsJumpingPosition : IState
    {
        private readonly CharacterAnimationController _animationController;
        private readonly AIPath _aiPath;
        private readonly Transform _modelTransform;
        private readonly PropertyData _speedPropertyData;
        private readonly CharacterIslandController _characterIslandController;
        private readonly Collider2D _collider;

        public WalkingTowardsJumpingPosition(CharacterAnimationController animationController, AIPath aiPath,
            Transform modelTransform, PropertyData speedPropertyData,
            CharacterIslandController characterIslandController, Collider2D collider)
        {
            _animationController = animationController;
            _aiPath = aiPath;
            _modelTransform = modelTransform;
            _speedPropertyData = speedPropertyData;
            _characterIslandController = characterIslandController;
            _collider = collider;
        }


        public void Tick()
        {
            var velocity = _aiPath.desiredVelocity;
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
            _characterIslandController.StartWalkingToJumpingPosition();
            _aiPath.canMove = false;
            _collider.isTrigger = true;
            
            WaitForCloudActionsToComplete().Forget();
        }
        public void OnExit()
        {
            Debug.Log("exited" + " " + _aiPath.canMove);
            _characterIslandController.StopWalkingToJumpingPosition();
            //_collider.isTrigger = false;
            
        }

        private async UniTask WaitForCloudActionsToComplete()
        {
            //await UniTask.WaitForSeconds(1);
            //Debug.Log("waited");
            var dest = _characterIslandController.GetJumpingPosition();
            dest = (Vector2) _modelTransform.position +  new Vector2(0, 4.5f);
            _aiPath.destination = dest;
            _aiPath.SearchPath();
            _animationController.Run();
            _aiPath.canMove = true;
            _aiPath.maxSpeed = _speedPropertyData.TemporaryValue * 2.25f;
        }
        
    }
}