using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/Gamedatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [field: SerializeField] public Material DissolveMaterial { get; private set; }
    }
}