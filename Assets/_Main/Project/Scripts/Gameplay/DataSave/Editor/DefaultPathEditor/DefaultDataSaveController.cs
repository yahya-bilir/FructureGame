using Cysharp.Threading.Tasks;
using DataSave.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlingTamplate.DataSave
{
    public static class DefaultDataSaveController
    {
        public static void SaveDefaultData(GameData gameData, string path, string fileName)
        {
            if (!fileName.Contains(".json")) fileName += ".json";
            string assetsPath = Application.dataPath;
            string fullPath = assetsPath + path;

            SaveManager.SaveDataAsync(gameData, fullPath, fileName).Forget();
            AssetDatabase.Refresh();
        }


        public static void LoadDefaultData(GameData gameData, string path, string fileName)
        {
            if (!fileName.Contains(".json")) fileName += ".json";
            string assetsPath = Application.dataPath;
            string fullPath = assetsPath + path;
            SaveManager.LoadDataAsync(gameData, fullPath, fileName).Forget();
            AssetDatabase.Refresh();
        }

        public static async UniTask DeleteDefaultData(string path, string fileName)
        {
            if (!fileName.Contains(".json")) fileName += ".json";
            string assetsPath = Application.dataPath;
            string fullPath = assetsPath + path;

            await SaveManager.DeleteDataAsync(fullPath, fileName);
            AssetDatabase.Refresh();
        }
    }
}