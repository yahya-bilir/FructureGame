using System;

namespace PropertySystem.Save
{
    [Serializable]
    public class PropertySaveableData
    {
        public string Id;
        public PropertyData PropertyData;
        
        public PropertySaveableData(string id, PropertyData propertyData)
        {
            Id = id;
            PropertyData = propertyData;
        }
    }
}