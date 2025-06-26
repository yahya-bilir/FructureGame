using UnityEngine;

namespace Perks.Base
{
    public abstract class ClickableActionSo : ScriptableObject
    {
        public abstract void OnDragEndedOnScene(Vector2 worldPos);
    }
}