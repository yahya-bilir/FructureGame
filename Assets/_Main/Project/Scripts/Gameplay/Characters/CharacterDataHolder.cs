using DataSave.Runtime;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu(fileName = "CharacterDataHolder", menuName = "Scriptable Objects/Character Data Holder", order = 0)]
    public class CharacterDataHolder : ScriptableObject
    {
        [field: SerializeField] public CharacterProperties CharacterProperties { get; private set; }
        [field: SerializeField] public int Worth { get; private set; }
        [field: SerializeField, ColorUsage(true, true)] public Color ShineColor { get; private set; }
        //[field: SerializeField] public CoinSplash CoinSplash { get; private set; }
    }
}