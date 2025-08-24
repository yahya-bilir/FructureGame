using UnityEngine;

namespace CollectionSystem
{
    [CreateAssetMenu(fileName = "CollectionAreaDataHolder", menuName = "Scriptable Objects/Collection Area Data Holder", order = 0)]
    public class CollectionAreaDataHolder : ScriptableObject
    {
        [field: SerializeField] public float ApproachMaxSpeed { get; private set; } = 10f;
        [field: SerializeField] public float ConveyorSpeed { get; private set; } = 6f;
        [field: SerializeField] public float StopDistance { get; private set; } = 0.05f;
        [field: SerializeField] public int FragmentCountToCreateAmmo { get; private set; } = 5;
    }
}