using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        private readonly CharacterMovementController characterMovementController;

        public SearchingForEnemy(CharacterMovementController characterMovementController)
        {
            this.characterMovementController = characterMovementController;
        }            

        public void Tick()
        {
            Debug.Log("Searcing for enemy");
            //_enemyMovementController.StopCharacter(true);

        }

        public void OnEnter()
        {
            //_enemyMovementController.StopCharacter(true);
        }

        public void OnExit()
        {
        }
    }
}