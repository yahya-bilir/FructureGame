using System.Collections.Generic;
using System.Linq;
using DataSave.Runtime;
using UnityEngine;
using VContainer;

namespace PropertySystem
{
    public class CharacterPropertyManager
    {
        private readonly CharacterPropertiesSO _characterPropertiesSo;
        private List<PropertyData> _propertySaveDatas = new();
        private GameData _gameData;

        public CharacterPropertyManager(CharacterPropertiesSO characterPropertiesSo)
        {
            _characterPropertiesSo = characterPropertiesSo;
        }

        [Inject]
        private void Inject(GameData gameData)
        {
            _gameData = gameData;
            Initialize();
        }

        private void Initialize()
        {
            if (_characterPropertiesSo.IsSaveable)
            {
                ReadFromGameData();
                return;
            }
            
            _propertySaveDatas = _characterPropertiesSo.PropertySaveDatas
                .Select(p => new PropertyData(p.PropertyQuery, p.PermanentValue, p.TemporaryValue))
                .ToList();

        }

        public PropertyData GetProperty(PropertyQuery query)
        {
            var data = _propertySaveDatas.Find(i => i.PropertyQuery == query);
            return data;
        }

        public void SetPropertyTemporarily(PropertyQuery query,  float temporaryValue)
        {
            var data = GetProperty(query);
            data.SetDataInternally(data.PermanentValue, temporaryValue);
        }
        
        public void SetPropertyPermanently(PropertyQuery query, float newPermanentValue)
        {
            var data = GetProperty(query);
            data.SetDataInternally(newPermanentValue, newPermanentValue);
            if (!_characterPropertiesSo.IsSaveable) return;
            SaveToGameData(data); // sadece permanent veriyi kaydet
        }
        
        private void SaveToGameData(PropertyData data)
        {
            var saveData = _gameData.PropertySaves.GetProperty(data, _characterPropertiesSo.EntityId);
            saveData.PropertyData.SetDataInternally(data.PermanentValue, data.PermanentValue);
            _gameData.PropertySaves.SaveProperty(saveData);
        }

        private void ReadFromGameData()
        {
            _propertySaveDatas.Clear();

            foreach (var t in _characterPropertiesSo.PropertySaveDatas)
            {
                var propertyData = _gameData.PropertySaves
                    .GetProperty(t, _characterPropertiesSo.EntityId)?
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
                }
            }
        }
    }
}