using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Waiting : IState
    {
        private readonly EnemyMovementController _enemyMovementController;

        public Waiting(EnemyMovementController enemyMovementController)
        {
            _enemyMovementController = enemyMovementController;
        }

        public void Tick()
        {
            Debug.Log("Waiting");
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