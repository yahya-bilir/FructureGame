using UnityEngine;

namespace Perks
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Perks/Perk UI Info", order = 0)]
    public class PerkUIInfo : ScriptableObject
    {
        [field: SerializeField] public Sprite Background { get; private set; }
        [field: SerializeField] public Color NameColor { get; private set; } = Color.white;
        [field: SerializeField] public Color NewStatColor { get; private set; } = Color.white;
    }
}