using AI.Base.Interfaces;
using Characters.Enemy;
using UnityEngine;

namespace AI.EnemyStates
{
    public class Waiting : IState
    {
        private readonly CharacterMovementController characterMovementController;

        public Waiting(CharacterMovementController characterMovementController)
        {
            this.characterMovementController = characterMovementController;
        }

        public void Tick()
        {
            Debug.Log("Waiting");
        }

        public void OnEnter()
        {
            characterMovementController.StopCharacter(true);
        }

        public void OnExit()
        {
        }
    }
}