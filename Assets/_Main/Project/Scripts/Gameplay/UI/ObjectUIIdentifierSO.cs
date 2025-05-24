using UnityEngine;

namespace UI
{
    public abstract class ObjectUIIdentifierSO : ScriptableObject
    {
        [field: SerializeField] public string ObjectName { get; private set; }
        [field: SerializeField] public Sprite ObjectSprite { get; private set; }
    }
}