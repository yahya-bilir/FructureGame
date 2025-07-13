using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        private readonly EnemyMovementController _enemyMovementController;

        public SearchingForEnemy(EnemyMovementController enemyMovementController)
        {
            _enemyMovementController = enemyMovementController;
        }            

        public void Tick()
        {
            Debug.Log("Searcing for enemy");
            _enemyMovementController.StopCharacter(true);

        }

        public void OnEnter()
        {
            _enemyMovementController.StopCharacter(true);
        }

        public void OnExit()
        {
        }
    }
}