using System.Collections.Generic;
using PropertySystem;
using UnityEngine;

namespace DataSave.Runtime
{
    [CreateAssetMenu(fileName = "CharacterProperties", menuName = "Scriptable Objects/Character Properties")]
    public class CharacterProperties : ScriptableObject
    {
        [SerializeField] private List<PropertySaveData> propertySaveDatas;

        public PropertySaveData GetPropertySaveData(PropertyQuery query, string id = "PlayerID", float permanentValue = 0,
            float temporaryValue = 0)
        {
            var data = propertySaveDatas.Find(i => i.EntityId == id && i.PropertyQuery == query);
            if (data != null) return data;
            
            
            return SetPropertySaveData(query, permanentValue, temporaryValue, id);
        }

        public PropertySaveData SetPropertySaveData(PropertyQuery query, float permanentValue, float temporaryValue,
            string id = "PlayerID")
        {
            var data = propertySaveDatas.Find(i => i.EntityId == id && i.PropertyQuery == query);
            if (data == null)
            {
                data = new PropertySaveData(query, permanentValue, temporaryValue, id);
                propertySaveDatas.Add(data);
            }

            data.SetDataInternally(permanentValue, temporaryValue);
            return data;
        }

        public PropertySaveData SetPropertyTempSaveData(PropertyQuery query, float temporaryValue,
            string id = "PlayerID")
        {
            var data = propertySaveDatas.Find(i => i.EntityId == id && i.PropertyQuery == query);
            if (data == null)
            {
                data = new PropertySaveData(query, 0, temporaryValue, id);
                propertySaveDatas.Add(data);
            }

            data.SetTempValueLevel(temporaryValue);
            return data;
        }
    }
}