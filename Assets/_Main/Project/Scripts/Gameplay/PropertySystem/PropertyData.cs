using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PropertySystem
{
    [Serializable]
    public class PropertyData
    {
        [FoldoutGroup("Property")]
        [field: SerializeField] public PropertyQuery PropertyQuery { get; private set; }
        
        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public float PermanentValue { get; private set; }

        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public float TemporaryValue { get; private set; }

        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public int PermanentValueLevel { get; private set; }

        public PropertyData(PropertyQuery propertyQuery, float permanentValue, float temporalValue)
        {
            PropertyQuery = propertyQuery; SetDataInternally(permanentValue, temporalValue);
        }

        public PropertyData SetDataInternally(float permanentValue, float temporalValue)
        {
            PermanentValue = permanentValue;
            TemporaryValue = temporalValue;
            return this;
        }
        
        public PropertyData Clone()
        {
            var copy = new PropertyData(PropertyQuery, PermanentValue, TemporaryValue);

            // Private set olduğu için reflection yerine constructor sonrası setmek en iyisi
            typeof(PropertyData).GetProperty(nameof(PermanentValueLevel))
                ?.SetValue(copy, this.PermanentValueLevel);

            return copy;
        }
    }
}