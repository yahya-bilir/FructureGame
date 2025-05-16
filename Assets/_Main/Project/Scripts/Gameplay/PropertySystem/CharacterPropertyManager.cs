using System.Collections.Generic;
using System.Linq;
using DataSave.Runtime;
using UnityEngine;

namespace PropertySystem
{
    public class CharacterPropertyManager
    {
        private readonly CharacterProperties _characterProperties;
        private List<PropertyData> _propertySaveDatas = new();
        private readonly GameData _gameData;

        public CharacterPropertyManager(CharacterProperties characterProperties, GameData gameData)
        {
            _characterProperties = characterProperties;
            _gameData = gameData;

            Initialize();
        }

        private void Initialize()
        {

            if (_characterProperties.IsSaveable)
            {
                ReadFromGameData();
                return;
            }
            
            _propertySaveDatas = _characterProperties.PropertySaveDatas
                .Select(p => new PropertyData(p.PropertyQuery, p.PermanentValue, p.TemporaryValue))
                .ToList();

        }

        public PropertyData GetProperty(PropertyQuery query)
        {
            var data = _propertySaveDatas.Find(i => i.PropertyQuery == query);
            return data;
        }

        public void SetProperty(PropertyQuery query, float permanentValue, float temporaryValue)
        {
            var data = GetProperty(query);
            data.SetDataInternally(permanentValue, temporaryValue);
            if (!_characterProperties.IsSaveable) return;
            SaveToGameData(data);
        }
        
        private void SaveToGameData(PropertyData data)
        {
            var saveData = _gameData.PropertySaves.GetProperty(data, _characterProperties.EntityId);
            saveData.PropertyData.SetDataInternally(data.PermanentValue, data.TemporaryValue);
            _gameData.PropertySaves.SaveProperty(saveData);
        }

        private void ReadFromGameData()
        {
            _propertySaveDatas.Clear();

            foreach (var t in _characterProperties.PropertySaveDatas)
            {
                var propertyData = _gameData.PropertySaves
                    .GetProperty(t, _characterProperties.EntityId)?
                    .PropertyData;

                if (propertyData != null)
                {
                    var copy = new PropertyData(
                        propertyData.PropertyQuery,
                        propertyData.PermanentValue,
                        propertyData.TemporaryValue
                    );

                    // Seviye bilgisi de kopyalansın istiyorsan:
                    copy.SetDataInternally(propertyData.PermanentValue, propertyData.TemporaryValue);
                    typeof(PropertyData).GetProperty("PermanentValueLevel")
                        ?.SetValue(copy, propertyData.PermanentValueLevel);

                    _propertySaveDatas.Add(copy);
                    Debug.Log(copy.TemporaryValue);
                }
            }
        }
    }
}