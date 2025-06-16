using AI.Base.Interfaces;
using UnityEngine;

namespace AI.EnemyStates
{
    public class SearchingForEnemy : IState
    {
        public void Tick()
        {
            Debug.Log("Searching for enemy");
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }
    }
}