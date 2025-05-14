using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PropertySystem
{
    [Serializable]
    public class PropertySaveData
    {
        [field: SerializeField] public string EntityId { get; private set; }

        [FoldoutGroup("Property")]
        [field: SerializeField] public PropertyQuery PropertyQuery { get; private set; }
        
        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public float PermanentValue { get; private set; }

        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public float TemporaryValue { get; private set; }

        [FoldoutGroup("Property/Values")]
        [field: SerializeField] public int PermanentValueLevel { get; private set; }

        public PropertySaveData(PropertyQuery propertyQuery, float permanentValue, float temporalValue, string entityId)
        {
            PropertyQuery = propertyQuery;
            EntityId = entityId;
            SetDataInternally(permanentValue, temporalValue);
        }

        public PropertySaveData SetDataInternally(float permanentValue, float temporalValue)
        {
            PermanentValue = permanentValue;
            TemporaryValue = temporalValue;
            return this;
        }

        public PropertySaveData SetPermanentValueLevel(int permanentValueLevel, float permanentValue)
        {
            PermanentValueLevel = permanentValueLevel;
            PermanentValue = permanentValue;
            return this;
        }

        public PropertySaveData SetTempValueLevel(float tempValue)
        {
            TemporaryValue = tempValue;
            return this;
        }
    }
}