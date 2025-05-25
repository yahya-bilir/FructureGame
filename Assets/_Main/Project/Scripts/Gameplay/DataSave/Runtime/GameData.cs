using System;
using System.Collections.Generic;
using EventBusses;
using Events;
using PropertySystem.Save;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace DataSave.Runtime
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [Button]
        public void Save() => GameDataSaveController.SaveNormal(this);
        
        public CharacterResource CharacterResource;
        public SettingsData SettingsData;
        public PlayerProgressData PlayerProgressData;
        public TutorialData TutorialData;
        public PropertySaves PropertySaves;
        public EnhanceButtonData EnhanceButtonData;
    }

    [System.Serializable]
    public class TutorialData
    {
        public bool IsCompleted;
        public int Section;
    }

    [System.Serializable]
    public class PlayerProgressData
    {
        public int mapCount = 2;
        public int Level;

        public List<LevelInfo> LevelInfos;

        public LevelInfo GetInfo(int lvl)
        {
            var result = LevelInfos.Find(x => x.Level == lvl);
            if (result == null)
            {
                LevelInfo lvlInfo = new LevelInfo();
                lvlInfo.IsOpen = lvl == 0;
                lvlInfo.Level = lvl;
                LevelInfos.Add(lvlInfo);
                result = lvlInfo;
            }

            return result;
        }

        public void LevelCompleted()
        {
            int nextLevel = Level + 1;
            if (nextLevel >= mapCount) return;
            GetInfo(nextLevel).IsOpen = true;
            Level = nextLevel;
        }
    }

    [System.Serializable]
    public class LevelInfo
    {
        public int Level;
        public bool IsOpen;
    }

    [System.Serializable]
    public class SettingsData
    {
        public bool IsCloseVibration;
        public bool IsCloseSound;
    }
    

    [Serializable]
    public class CharacterResource
    {
        [Inject] private IEventBus _eventBus;
        private int _coinCount;
        public int CoinCount
        {
            get => _coinCount;
            set
            {
                _coinCount = value;
                _eventBus.Publish(new OnCoinCountChanged(_coinCount));
            }
        }
    }

    [Serializable]
    public class EnhanceButtonData
    {
        public int ButtonClickedCount;
        public int TemporaryButtonClickedCount;
    }
}