using UnityEngine;

namespace CollectionSystem
{
    [CreateAssetMenu(fileName = "CollectionAreaDataHolder", menuName = "Scriptable Objects/Collection Area Data Holder", order = 0)]
    public class CollectionSystemDataHolder : ScriptableObject
    {
        [field: SerializeField] public float ApproachMaxSpeed { get; private set; } = 10f;
        [field: SerializeField] public float ConveyorSpeed { get; private set; } = 6f;
        [field: SerializeField] public int FragmentCountToCreateAmmo { get; private set; } = 5;

        [field: SerializeField] public float CreatedAmmoSplineSpeed { get; private set; }
        [field: SerializeField] public GameObject FireAmmo { get; private set; }
        [field: SerializeField] public GameObject IceAmmo { get; private set; }
        [field: SerializeField] public GameObject NormalAmmo { get; private set; }
        
    }
}