using Pathfinding;
using UnityEngine;

namespace AI.EnemyBehaviour
{
    public class EnemySeeker : MonoBehaviour
    {
        private AIDestinationSetter _aiDestinationSetter;

        private void Awake()
        {
            _aiDestinationSetter = GetComponent<AIDestinationSetter>();
        }

        private void Start()
        {
        }
    }
}