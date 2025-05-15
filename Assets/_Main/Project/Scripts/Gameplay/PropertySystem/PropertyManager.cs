using DataSave.Runtime;

namespace PropertySystem
{
    public class PropertyManager
    {
        private readonly CharacterProperties _characterProperties;
        public PropertyManager(CharacterProperties characterProperties)
        {
            _characterProperties = characterProperties;
        }
        
        public PropertyData GetPropertySaveData(PropertyQuery query, string id = "PlayerID", float permanentValue = 0,
            float temporaryValue = 0)
        {
            var data = _characterProperties.PropertySaveDatas.Find(i => i.EntityId == id && i.PropertyQuery == query);
            if (data != null) return data;
            
            return SetPropertySaveData(query, permanentValue, temporaryValue, id);
        }

        public PropertyData SetPropertySaveData(PropertyQuery query, float permanentValue, float temporaryValue,
            string id = "PlayerID")
        {
            var data = propertySaveDatas.Find(i => i.PropertyQuery == query);
            if (data == null)
            {
                data = new PropertyData(query, permanentValue, temporaryValue, id);
                propertySaveDatas.Add(data);
            }

            data.SetDataInternally(permanentValue, temporaryValue);
            return data;
        }

        public PropertyData SetPropertyTempSaveData(PropertyQuery query, float temporaryValue,
            string id = "PlayerID")
        {
            var data = propertySaveDatas.Find(i => i.PropertyQuery == query);
            if (data == null)
            {
                data = new PropertyData(query, 0, temporaryValue, id);
                propertySaveDatas.Add(data);
            }

            data.SetTempValueLevel(temporaryValue);
            return data;
        }
    }
}