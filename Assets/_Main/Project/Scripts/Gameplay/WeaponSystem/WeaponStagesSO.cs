using UnityEngine;

namespace WeaponSystem
{
    [CreateAssetMenu(fileName = "WeaponStage", menuName = "Scriptable Objects/Weapon Stage", order = 0)]
    public class WeaponStagesSO : ScriptableObject
    {
        [field: SerializeField] public int StarCount { get; private set; }
        [field: SerializeField] public string Prefix { get; private set; }
        [field: SerializeField] public Color PrefixColor { get; private set; }
        [field: SerializeField] public Color OutlineColor { get; private set; }
        [field: SerializeField] public Sprite BackgroundBorderSprite { get; private set; }
        [field: SerializeField] public Sprite BackgroundInnerSprite { get; private set; }
    }
}