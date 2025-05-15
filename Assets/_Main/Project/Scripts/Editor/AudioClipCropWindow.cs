using UnityEngine;
using UnityEditor;
using System.IO;

public static class AudioClipCropper
{
    [MenuItem("Assets/Crop Audio Clip", false, 20)]
    private static void CropSelectedAudioClip()
    {
        AudioClip selectedClip = Selection.activeObject as AudioClip;
        if (selectedClip == null)
        {
            Debug.LogWarning("Lütfen bir AudioClip seçin!");
            return;
        }

        // Pencere açarak başlangıç ve bitiş sürelerini al
        AudioClipCropWindow.ShowWindow(selectedClip);
    }

    [MenuItem("Assets/Crop Audio Clip", true)]
    private static bool ValidateCropSelectedAudioClip()
    {
        return Selection.activeObject is AudioClip;
    }
}

public class AudioClipCropWindow : EditorWindow
{
    private AudioClip audioClip;
    private float startTime = 0f;
    private float endTime = 1f;

    public static void ShowWindow(AudioClip clip)
    {
        AudioClipCropWindow window = GetWindow<AudioClipCropWindow>(true, "AudioClip Kırpma Aracı");
        window.audioClip = clip;
        window.endTime = clip.length;
        window.minSize = new Vector2(350, 150);
    }

    private void OnGUI()
    {
        GUILayout.Label($"Seçilen Clip: <b>{audioClip.name}</b> (Toplam Süre: {audioClip.length:F2}s)", new GUIStyle(EditorStyles.label) { richText = true });

        EditorGUILayout.Space();
        startTime = EditorGUILayout.FloatField("Başlangıç Zamanı (s)", startTime);
        endTime = EditorGUILayout.FloatField("Bitiş Zamanı (s)", endTime);

        // Değerleri sınırla
        if (startTime < 0) startTime = 0;
        if (endTime > audioClip.length) endTime = audioClip.length;
        if (startTime >= endTime) endTime = startTime + 0.1f;

        EditorGUILayout.Space();
        if (GUILayout.Button("Kırp ve Kaydet", GUILayout.Height(30)))
        {
            CropAndSave();
            Close();
        }
    }

    private void CropAndSave()
    {
        string originalPath = AssetDatabase.GetAssetPath(audioClip);
        string directory = Path.GetDirectoryName(originalPath);
        string filename = Path.GetFileNameWithoutExtension(originalPath);
        string extension = Path.GetExtension(originalPath);
        string newPath = Path.Combine(directory, $"{filename}_Cropped{extension}");

        // Ses verisini kırp
        AudioClip croppedClip = CropAudioClip(audioClip, startTime, endTime);

        // Yeni dosyayı oluştur ve kaydet
        SaveAudioClip(croppedClip, newPath);
        AssetDatabase.Refresh();

        Debug.Log($"Kırpılmış ses kaydedildi: <b>{newPath}</b>", AssetDatabase.LoadAssetAtPath<AudioClip>(newPath));
    }

    private AudioClip CropAudioClip(AudioClip originalClip, float startTime, float endTime)
    {
        int sampleRate = originalClip.frequency;
        int channels = originalClip.channels;
        int startSample = (int)(startTime * sampleRate);
        int endSample = (int)(endTime * sampleRate);
        int sampleLength = endSample - startSample;

        float[] originalData = new float[originalClip.samples * channels];
        originalClip.GetData(originalData, 0);

        float[] croppedData = new float[sampleLength * channels];
        System.Array.Copy(originalData, startSample * channels, croppedData, 0, croppedData.Length);

        AudioClip croppedClip = AudioClip.Create(
            $"{originalClip.name}_Cropped",
            sampleLength,
            channels,
            sampleRate,
            false
        );
        croppedClip.SetData(croppedData, 0);

        return croppedClip;
    }

    private void SaveAudioClip(AudioClip clip, string path)
    {
        // WAV formatında kaydetmek için (Unity'nin yerel desteği yok, bu yüzden dışarı aktarım gerekir)
        // Bu örnekte, Unity'nin AssetDatabase sistemi kullanılıyor.
        // Gerçek bir WAV kaydetmek için ek bir kütüphane gerekebilir.
        // Bu kısım, Unity'nin AudioClip'i doğrudan kaydetmesine izin vermez, bu yüzden geçici bir çözüm:
        UnityEditor.EditorUtility.CopySerialized(clip, AssetDatabase.LoadMainAssetAtPath(path));
    }
}