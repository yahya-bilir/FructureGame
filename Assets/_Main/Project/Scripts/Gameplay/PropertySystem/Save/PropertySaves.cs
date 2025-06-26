using System;
using System.Collections.Generic;
using UnityEngine;

namespace PropertySystem.Save
{    
    [Serializable]
    public class PropertySaves
    {
        [field: SerializeField] public List<PropertySaveableData> PropertySaveDatas { get; private set; }

        public void SaveProperty(PropertySaveableData saveData)
        {
            var existing = PropertySaveDatas.Find(p =>
                p.Id == saveData.Id && p.PropertyData.PropertyQuery == saveData.PropertyData.PropertyQuery);

            if (existing != null)
            {
                existing.PropertyData.SetDataInternally(
                    saveData.PropertyData.PermanentValue,
                    saveData.PropertyData.TemporaryValue);
            }
            else
            {
                PropertySaveDatas.Add(saveData);
//                Debug.Log(saveData);
            }
        }

        public PropertySaveableData GetProperty(PropertyData data, string id)
        {
            var collectedData = PropertySaveDatas.Find(i => i.PropertyData.PropertyQuery == data.PropertyQuery && i.Id == id);
            
            if (collectedData == null)
            {
                collectedData = new PropertySaveableData(id, data);
                SaveProperty(collectedData);
            }

            return collectedData;
        }
    }
}