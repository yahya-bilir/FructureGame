using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "GameDatabase", menuName = "Scriptable Objects/Gamedatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        [field: SerializeField] public WeaponDatabase WeaponDatabase { get; private set; }
        [field: SerializeField] public EnhanceButtonDatabase EnhanceButtonDatabase { get; private set; }
    }
}