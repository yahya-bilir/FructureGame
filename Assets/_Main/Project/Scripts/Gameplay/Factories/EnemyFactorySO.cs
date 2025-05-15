using System.Collections.Generic;
using Characters.Enemy;
using UnityEngine;

namespace Factories
{
    [CreateAssetMenu(fileName = "EnemyFactory", menuName = "Scriptable Objects/Enemy Factory", order = 0)]
    public class EnemyFactorySO : ScriptableObject
    {
        [field: SerializeField] public List<EnemyBehaviour> SpawnableEnemies { get; private set; }
        [field: SerializeField] public int SpawnLimit { get; private set; }
        [field: SerializeField] public float SpawnInterval { get; private set; }
    }
}