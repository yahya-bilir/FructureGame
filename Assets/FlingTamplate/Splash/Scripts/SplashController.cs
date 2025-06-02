using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    [SerializeField] private float splashTime = 2f;

    private void Start()
    {
        Application.targetFrameRate = 60;
        LoadAssetLoaderScene().Forget();
    }

    private async UniTaskVoid LoadAssetLoaderScene()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(splashTime));
        SceneManager.LoadScene(1);
    }
}