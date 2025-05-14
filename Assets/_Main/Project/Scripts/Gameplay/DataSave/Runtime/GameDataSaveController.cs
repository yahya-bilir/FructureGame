using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DataSave.Runtime
{
    public static class GameDataSaveController
    {
        private static string _path = "GameDatas";
        private static string _fileName = "GameData.json";

        public static UniTask SaveGameData(GameData gameData)
        {
            //   string fullPath = Path.Combine(Application.persistentDataPath, _path);
            string fullPath = Application.persistentDataPath;
            return SaveManager.SaveDataAsync(gameData, fullPath, _fileName);
        }

        public static UniTask LoadGameData(GameData gameData)
        {
            // string fullPath = Path.Combine(Application.persistentDataPath, _path);
            string fullPath = Application.persistentDataPath;
            return SaveManager.LoadDataAsync(gameData, fullPath, _fileName);
        }

        public static GameData LoadGameDataNormal(GameData gameData)
        {
            string fullPath = Application.persistentDataPath; // Path.Combine(Application.persistentDataPath);
            return SaveManager.LoadData(gameData, fullPath, _fileName);
        }

        public static void SaveNormal(GameData gameData)
        {
            string fullPath = Application.persistentDataPath;
            SaveManager.SaveDataNormal(gameData, fullPath, _fileName);
        }
    }
}