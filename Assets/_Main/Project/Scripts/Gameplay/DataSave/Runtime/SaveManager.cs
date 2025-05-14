using System;
using System.Diagnostics;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DataSave.Runtime
{
    public static class SaveManager
    {
        // public static async UniTask SaveDataAsync<T>(T data, string path, string fileName)
        //     where T : ScriptableObject
        // {
        //     CreateFolder(path);
        //     string fullPath = path + fileName;
        //     var json = JsonUtility.ToJson(data);
        //      File.WriteAllTextAsync(fullPath, json);
        //     // return UniTask.Run(() =>
        //     // {
        //     //   
        //     // });
        // }


        public static async UniTask SaveDataAsync<T>(T data, string path, string fileName) where T : ScriptableObject
        {
            try
            {
                CreateFolder(path);
                string fullPath = Path.Combine(path, fileName);
                string json = JsonUtility.ToJson(data, true);
                Debug.LogWarning($"Saved Data {json}");
                await File.WriteAllTextAsync(fullPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed: {e}");
                throw;
            }
        }

        public static GameData LoadData(GameData gameData, string path, string fileName)
        {
            Debug.Log($"Data save path={path}");
            var fullPath = Path.Combine(path, fileName);
            if (!File.Exists(fullPath))
            {
                Debug.Log($"Data save path not exists={path}");
                SaveDataNormal(gameData, path, fileName);
            }

            var txt = File.ReadAllText(fullPath); // async okuma
            JsonUtility.FromJsonOverwrite(txt, gameData);
            return gameData;
        }

        public static void SaveDataNormal(GameData gameData, string path, string fileName)
        {
            CreateFolder(path);
            string fullPath = Path.Combine(path, fileName);
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(fullPath, json);
        }

        public static async UniTask LoadDataAsync<T>(T data, string path, string fileName)
            where T : ScriptableObject
        {
            var fullPath = Path.Combine(path, fileName);

            try
            {
                if (!File.Exists(fullPath))
                {
                    await SaveDataAsync(data, path, fileName); // Eğer yoksa save et
                }

                var txt = await File.ReadAllTextAsync(fullPath); // async okuma
                JsonUtility.FromJsonOverwrite(txt, data);
              
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadDataAsync Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static async UniTask DeleteDataAsync(string path, string fileName)
        {
            var fullPath = Path.Combine(path, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    // Run the file deletion on a background thread to make it asynchronous
                    await UniTask.RunOnThreadPool(() =>
                    {
                        File.Delete(fullPath);
                        Debug.Log($"File deleted: {fullPath}");
                    });
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to delete file: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"File does not exist: {fullPath}");
            }

            // Return a completed task to satisfy the method signature
            await UniTask.CompletedTask;
        }

        static void CreateFolder(string pathIn)
        {
            if (Directory.Exists(pathIn) == false)
            {
                Debug.Log($"Data save Created Path {pathIn}");
                Directory.CreateDirectory(pathIn);
                Debug.Log("Created Path" + pathIn);
            }
// #if UNITY_EDITOR
//             ShowExplorer(pathIn);
// #endif
        }


        private static void ShowExplorer(string path)
        {
            path = path.Replace(@"/", @"\"); // Ensure the path uses backslashes for Windows

            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer)
            {
                Process.Start("explorer.exe", path);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor ||
                     Application.platform == RuntimePlatform.OSXPlayer)
            {
                Process.Start("open", path);
            }
            else if (Application.platform == RuntimePlatform.LinuxEditor ||
                     Application.platform == RuntimePlatform.LinuxPlayer)
            {
                Process.Start("xdg-open", path);
            }
            else
            {
                Debug.LogWarning("Platform not supported for opening file explorer.");
            }
        }
    }
}