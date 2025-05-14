using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Initialization
{
    public class AddressableStartupLoader : IStartable
    {
        private readonly string _addressKey = "default";
        private readonly float _waitAfterDownload = 1f;

        public async void Start()
        {
            Debug.Log("Checking for addressable asset size...");

            var sizeHandle = Addressables.GetDownloadSizeAsync(_addressKey);
            await sizeHandle;

            if (sizeHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError("Failed to check download size.");
                return;
            }

            if (sizeHandle.Result == 0)
            {
                Debug.Log("Assets already up to date.");
            }
            else
            {
                Debug.Log("Downloading addressable assets...");
                var downloadHandle = Addressables.DownloadDependenciesAsync(_addressKey);

                while (!downloadHandle.IsDone)
                {
                    Debug.Log($"Progress: {(downloadHandle.PercentComplete * 100f):0.0}%");
                    await UniTask.Yield();
                }

                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Download complete.");
                    Addressables.Release(downloadHandle);
                }
                else
                {
                    Debug.LogError("Download failed.");
                    return;
                }
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_waitAfterDownload));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }
}