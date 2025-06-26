using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class CCDUploaderWindow : EditorWindow
{
    private const string UXML_PATH = "Assets/FlingTamplate/Editor/AssetUploader/CCDUploader.uxml";

    private const string PREF_KEY_ID = "CCD_KeyId";
    private const string PREF_SECRET = "CCD_SecretKey";
    private const string PREF_PROJECT = "CCD_ProjectId";
    private const string PREF_ENV = "CCD_EnvId";
    private const string PREF_BUCKET = "CCD_BucketId";

    // private string PREF_KEY_DEFAULT = "0db6e081-4edd-43ff-88fb-6121ece54740";
    // private string PREF_SECRET_DEFAULT = "pDM51SOq6ZsqmsXfO887BIMkv0MZb4zS";
    // private string PREF_PROJECT_DEFAULT = "1b7fad74-3e1d-4586-918c-30b9447321eb";
    // private string PREF_ENV_DEFAULT = "1c86c837-b303-40ec-ab33-2ae84e9d9ee7";
    //  private string PREF_BUCKET_DEFAULT = "620ad1cb-f87e-4584-81c9-2e529a96aff9";

    private TextField keyIdField, secretKeyField, projectIdField, envIdField, bucketIdField, remotePathField;
    private Label statusLabel;
    private DropdownField envDropdown;
    private Label envInfoLabel;

    private readonly List<string> environments = new() { "Development", "Production" };

    private DropdownField bucketDropdown;
    private Label bucketInfoLabel;
    private string auth;
    private bool _isValid = true;


    [MenuItem("Flickle/CCD Uploader")]
    public static void ShowWindow()
    {
        //Create Bucket Dev And Production , Get Fill the Areas
        var window = GetWindow<CCDUploaderWindow>(false, "CCD Uploader");
        window.titleContent = new GUIContent("CCD Uploader", EditorGUIUtility.IconContent("CloudConnect").image);
    }

    public void CheckValidity()
    {
        _isValid = true;
        ValidateField(keyIdField, val => !string.IsNullOrWhiteSpace(val) && val.Length > 5);
        ValidateField(secretKeyField, val => !string.IsNullOrWhiteSpace(val) && val.Length > 5);
        ValidateField(projectIdField, val => !string.IsNullOrWhiteSpace(val) && val.Length > 5);
    }

    private void ValidateField(TextField field, Func<string, bool> isValid)
    {
        if (isValid(field.value))
        {
            field.style.borderBottomColor = Color.green;
            field.style.borderTopColor = Color.green;
            field.style.borderLeftColor = Color.green;
            field.style.borderRightColor = Color.green;
            field.style.borderBottomWidth = 1;
            // field.style.borderTopWidth = 1;
            // field.style.borderLeftWidth = 1;
            // field.style.borderRightWidth = 1;
        }
        else
        {
            _isValid = false;
            field.style.borderBottomColor = Color.red;
            field.style.borderTopColor = Color.red;
            field.style.borderLeftColor = Color.red;
            field.style.borderRightColor = Color.red;
            field.style.borderBottomWidth = 1;
            // field.style.borderTopWidth = 1;
            // field.style.borderLeftWidth = 1;
            // field.style.borderRightWidth = 1;
        }
    }

    public async UniTaskVoid Initialization()
    {
        Debug.Log($"Is Valid {_isValid}");
        if (!_isValid)
        {
            envDropdown.choices.Clear();
            bucketDropdown.choices.Clear();
            envDropdown.value = "None";
            bucketDropdown.value = "None";
            envIdField.value = "None";
            bucketIdField.value = "None";
            return;
        }

        auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyIdField.value}:{secretKeyField.value}"));
        string envUrl =
            $"https://services.api.unity.com/ccd/management/v1/projects/{projectIdField.value}/environments";

        using var envReq = UnityWebRequest.Get(envUrl);
        envReq.SetRequestHeader("Authorization", $"Basic {auth}");
        await envReq.SendWebRequest();

        if (envReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Failed to get environments: {envReq.error}");
            return;
        }

        string json = "{\"environments\":" + envReq.downloadHandler.text + "}";
        var envData = JsonUtility.FromJson<CCDEnvironmentList>(json);

        // envDropdown = rootVisualElement.Q<DropdownField>("EnvDropdown");
        // envIdField = rootVisualElement.Q<TextField>("EnvIdField");
        // envInfoLabel = rootVisualElement.Q<Label>("EnvInfoLabel");
        // var bucketDropdown = rootVisualElement.Q<DropdownField>("BucketDropdown");
        // var bucketInfoLabel = rootVisualElement.Q<Label>("BucketInfoLabel");
        envDropdown.choices = envData.environments.Select(e => e.name).ToList();

        string savedEnvId = EditorPrefs.GetString("CCD_ENV_ID", "");
        string savedEnvName = EditorPrefs.GetString("CCD_ENV_NAME", "");
        if (!string.IsNullOrEmpty(savedEnvId))
        {
            var matched = envData.environments.FirstOrDefault(e => e.id == savedEnvId);
            if (matched != null)
            {
                envDropdown.value = matched.name;
                envIdField.value = matched.id;
                envInfoLabel.text = $"Selected: {matched.name}";
                await LoadBucketsForEnvironment(matched.id);
            }
            else
            {
                envDropdown.value = envData.environments[0].name;
                envIdField.value = envData.environments[0].id;
                envInfoLabel.text = $"Selected: {envDropdown.value}";
                await LoadBucketsForEnvironment(envData.environments[0].id);
            }
        }


        envDropdown.RegisterValueChangedCallback(async evt =>
        {
            var selected = envData.environments.FirstOrDefault(e => e.name == evt.newValue);
            if (selected != null)
            {
                envIdField.value = selected.id;
                envInfoLabel.text = $"Selected: {selected.name}";

                EditorPrefs.SetString("CCD_ENV_ID", selected.id);
                EditorPrefs.SetString("CCD_ENV_NAME", selected.name);

                await LoadBucketsForEnvironment(selected.id);
            }
        });
    }

    private async UniTask LoadBucketsForEnvironment(string environmentId)
    {
        string savedBucketId = EditorPrefs.GetString("CCD_BUCKET_ID", "");
        string savedBucketName = EditorPrefs.GetString("CCD_BUCKET_NAME", "");

        string bucketUrl =
            $"https://services.api.unity.com/ccd/management/v1/projects/{projectIdField.value}/environments/{environmentId}/buckets";
        using var bucketReq = UnityWebRequest.Get(bucketUrl);
        bucketReq.SetRequestHeader("Authorization", $"Basic {auth}");
        await bucketReq.SendWebRequest();

        if (bucketReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Failed to get buckets: {bucketReq.error}");
            bucketDropdown.choices = new List<string> { "Failed to load" };
            return;
        }

        string bucketJson = "{\"buckets\":" + bucketReq.downloadHandler.text + "}";
        var bucketData = JsonUtility.FromJson<CCDBucketList>(bucketJson);

        bucketDropdown.choices = bucketData.buckets.Select(b => b.name).ToList();

        if (!string.IsNullOrEmpty(savedBucketId))
        {
            var matchedBucket = bucketData.buckets.FirstOrDefault(b => b.id == savedBucketId);
            if (matchedBucket != null)
            {
                bucketDropdown.value = matchedBucket.name;
                bucketIdField.value = matchedBucket.id;
                bucketInfoLabel.text = $"Selected: {matchedBucket.name}";
            }
            else
            {
                bucketDropdown.value = bucketData.buckets.Count > 0 ? bucketData.buckets[0].name : "None";
                bucketInfoLabel.text = $"Selected: {bucketDropdown.value}";
            }
        }


        bucketDropdown.RegisterValueChangedCallback(bucketEvt =>
        {
            var selectedBucket = bucketData.buckets.FirstOrDefault(b => b.name == bucketEvt.newValue);
            if (selectedBucket != null)
            {
                // var bucketIdField = rootVisualElement.Q<TextField>("BucketIdField");
                bucketIdField.value = selectedBucket.id;
                bucketInfoLabel.text = $"Selected: {selectedBucket.name}";

                EditorPrefs.SetString("CCD_BUCKET_ID", selectedBucket.id);
                EditorPrefs.SetString("CCD_BUCKET_NAME", selectedBucket.name);
            }
        });
    }

    private void OnEnvironmentSelected(string selectedEnv)
    {
        switch (selectedEnv)
        {
            case "Development":
                envIdField.value = "DEV_ENV_ID";
                break;
            case "Production":
                envIdField.value = "PROD_ENV_ID";
                break;
        }
    }

    private VisualElement _root;

    public void CreateGUI()
    {
        _isValid = true;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        visualTree.CloneTree(rootVisualElement);

        keyIdField = rootVisualElement.Q<TextField>("KeyIdField");
        secretKeyField = rootVisualElement.Q<TextField>("SecretKeyField");
        projectIdField = rootVisualElement.Q<TextField>("ProjectIdField");
        envIdField = rootVisualElement.Q<TextField>("EnvIdField");
        bucketIdField = rootVisualElement.Q<TextField>("BucketIdField");
        remotePathField = rootVisualElement.Q<TextField>("RemotePathField");
        statusLabel = rootVisualElement.Q<Label>("StatusLabel");
        rootVisualElement.Q<Button>("SetRemotePathButton").clicked += () => { SetRemotePath(); };

        envDropdown = rootVisualElement.Q<DropdownField>("EnvDropdown");
        envInfoLabel = rootVisualElement.Q<Label>("EnvInfoLabel");

        bucketDropdown = rootVisualElement.Q<DropdownField>("BucketDropdown");
        bucketInfoLabel = rootVisualElement.Q<Label>("BucketInfoLabel");

        LoadPrefs();
        CheckValidity();

        keyIdField.RegisterValueChangedCallback(evt =>
        {
            CheckValidity();
            SavePrefs();
            Initialization().Forget();
        });
        secretKeyField.RegisterValueChangedCallback(evt =>
        {
            CheckValidity();
            SavePrefs();
            Initialization().Forget();
        });
        projectIdField.RegisterValueChangedCallback(evt =>
        {
            CheckValidity();
            SavePrefs();
            Initialization().Forget();
        });
        envIdField.RegisterValueChangedCallback(evt => SavePrefs());
        bucketIdField.RegisterValueChangedCallback(evt => SavePrefs());
        statusLabel.RegisterValueChangedCallback(evt => SavePrefs());


        rootVisualElement.Q<Button>("UploadButton").clicked += () => Upload().Forget();
        ShowRemotePath();
        Initialization().Forget();
    }


    private void SetRemotePath()
    {
        string profileName = "Default";
        string variableName = "Remote.LoadPath";
        string variableValue = "";
        string savedBucketId = EditorPrefs.GetString("CCD_BUCKET_ID", "");
        string envName = EditorPrefs.GetString("CCD_ENV_NAME", "");

        variableValue =
            $"https //{projectIdField.value}.client-api.unity3dusercontent.com/client_api/v1/environments/{envName}/buckets/{savedBucketId}/release_by_badge/latest/entry_by_path/content/?path=";

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var profileSettings = settings.profileSettings;
        var profileId =
            profileSettings
                .GetProfileId(
                    profileName);
        profileSettings.SetValue(profileId, variableName, variableValue);

        ShowRemotePath();
    }

    private void ShowRemotePath()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var remoteLoadPathVar = settings.profileSettings.GetAllProfileNames();
        remoteLoadPathVar.ForEach(x => Debug.Log($"Profile: {x}"));
        var defPath = settings.RemoteCatalogLoadPath.GetValue(settings);
        remotePathField.value = defPath;
    }

    private void LoadPrefs()
    {
        // EditorPrefs.DeleteKey(PREF_KEY_ID);
        // EditorPrefs.DeleteKey(PREF_SECRET);
        // EditorPrefs.DeleteKey(PREF_PROJECT);
        // EditorPrefs.DeleteKey(PREF_ENV);
        // EditorPrefs.DeleteKey(PREF_BUCKET);

        keyIdField.value = GetPrefs(PREF_KEY_ID, "");
        secretKeyField.value = GetPrefs(PREF_SECRET, "");
        projectIdField.value = GetPrefs(PREF_PROJECT, "");
        envIdField.value = GetPrefs(PREF_ENV, "");
        bucketIdField.value = GetPrefs(PREF_BUCKET, "");
    }

    private string GetPrefs(string key, string defaultValue)
    {
        var result = EditorPrefs.GetString(key, "");
        if (result.Length <= 2)
        {
            result = defaultValue;
            EditorPrefs.SetString(key, defaultValue);
        }

        return result;
    }

    private void SavePrefs()
    {
        EditorPrefs.SetString(PREF_KEY_ID, keyIdField.value);
        EditorPrefs.SetString(PREF_SECRET, secretKeyField.value);
        EditorPrefs.SetString(PREF_PROJECT, projectIdField.value);
        EditorPrefs.SetString(PREF_ENV, envIdField.value);
        EditorPrefs.SetString(PREF_BUCKET, bucketIdField.value);
    }

    private async UniTaskVoid Upload()
    {
        if (!_isValid)
        {
            Debug.LogError("Setup is not valid !");
            return;
        }

        string startPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string folderPath = EditorUtility.OpenFolderPanel("Select folder", startPath, "");
        List<string> files = new();
        if (!string.IsNullOrEmpty(folderPath))
        {
            files = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
        }

        // await DeleteAllEntries();

        for (int i = 0; i < files.Count; i++)
        {
            var filePath = files[i];
            if (string.IsNullOrEmpty(filePath)) return;

            statusLabel.text = $"{i} Uploading...";

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            string contentHash = GetMD5(fileBytes);
            string fileName = Path.GetFileName(filePath);

            string projectId = projectIdField.value;
            string envId = envIdField.value;
            string bucketId = bucketIdField.value;
            string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyIdField.value}:{secretKeyField.value}"));


            // https://services.api.unity.com/ccd/management/v1/projects/{projectid}/environments/{environmentid}/buckets/{bucketid}/entry_by_path
            // string baseUrl =
            //     $"https://services.api.unity.com/ccd/management/v1/projects/{projectId}/environments/{envId}/buckets/{bucketId}/entries/";  
            string path = UnityWebRequest.EscapeURL(fileName);
            string url =
                $"https://services.api.unity.com/ccd/management/v1/projects/{projectId}/environments/{envId}/buckets/{bucketId}/entry_by_path?path={path}&updateIfExists=true";


            var entryPayload = new CCDEntry
            {
                content_hash = contentHash,
                content_size = fileBytes.Length,
                content_type = "application/octet-stream",
                signed_url = true
            };

            string json = JsonUtility.ToJson(entryPayload);

            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Authorization", $"Basic {auth}");
            req.SetRequestHeader("Content-Type", "application/json");

            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"❌ Upload failed: {req.responseCode} - {req.downloadHandler.text}");
            }
            else
            {
                var response = JsonUtility.FromJson<CCDEntryResponse>(req.downloadHandler.text);
                await UploadToSignedUrl(response.signed_url, fileBytes, response.content_type);
            }
        }

        await CreateRelease();
        statusLabel.text = "✅ Upload and Release successful!";
    }


    private async UniTask UploadToSignedUrl(string signedUrl, byte[] data, string contentType)
    {
        using var uploadReq = UnityWebRequest.Put(signedUrl, data);
        uploadReq.SetRequestHeader("Content-Type", contentType);
        await uploadReq.SendWebRequest();
    }

    private async UniTask CreateRelease()
    {
        string url =
            $"https://services.api.unity.com/ccd/management/v1/projects/{projectIdField.value}/environments/{envIdField.value}/buckets/{bucketIdField.value}/releases";

        string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyIdField.value}:{secretKeyField.value}"));

        var body = new
        {
            notes = $"Automated release on {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"
        };
        string json = JsonUtility.ToJson(body);

        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", $"Basic {auth}");
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ Release creation failed: {req.error}");
        }
        else
        {
            Debug.Log("✅ Release created successfully.");
        }
    }

    private string GetMD5(byte[] bytes)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    private async UniTask DeleteAllEntries()
    {
        string projectId = projectIdField.value;
        string envId = envIdField.value;
        string bucketId = bucketIdField.value;
        string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyIdField.value}:{secretKeyField.value}"));

        string listUrl =
            $"https://services.api.unity.com/ccd/management/v1/projects/{projectId}/environments/{envId}/buckets/{bucketId}/entries";

        using var listReq = UnityWebRequest.Get(listUrl);
        listReq.SetRequestHeader("Authorization", $"Basic {auth}");
        await listReq.SendWebRequest();

        if (listReq.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ Failed to fetch entries: " + listReq.downloadHandler.text);
            statusLabel.text = "❌ Failed to list entries.";
            return;
        }

        var listResponse =
            JsonUtility.FromJson<CCDEntryListResponse>("{\"entries\":" + listReq.downloadHandler.text + "}");
        foreach (var VARIABLE in listResponse.entries)
        {
            Debug.Log($"Entire {VARIABLE.entryid}  /n path= {VARIABLE.path}");
        }

        foreach (var entry in listResponse.entries)
        {
            string deleteUrl = $"{listUrl}/{entry.entryid}";
            using var deleteReq = UnityWebRequest.Delete(deleteUrl);
            deleteReq.SetRequestHeader("Authorization", $"Basic {auth}");
            await deleteReq.SendWebRequest();

            if (deleteReq.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Deleted {entry.path}");
            }
            else
            {
                Debug.LogError($"❌ Failed to delete {entry.path}: {deleteReq.downloadHandler.text}");
            }
        }

        statusLabel.text = "✅ All entries deleted.";
    }

    [Serializable]
    private class CCDEntry
    {
        public string content_hash;
        public long content_size;
        public string content_type;
        public bool signed_url;
    }

    [Serializable]
    private class CCDEntryResponse
    {
        public string signed_url;
        public string content_type;
    }

    [Serializable]
    private class CCDEntryUpdateResponse
    {
        public string entry_id;
    }

    [Serializable]
    private class CCDEntryListResponse
    {
        public CCDEntryInfo[] entries;
    }

    [Serializable]
    private class CCDEntryInfo
    {
        public string entryid;
        public string current_versionid;
        public string path;
        public string content_hash;
        public int content_size;
        public string content_type;
        public string[] labels;
        public string content_link;
        public string link;
        public bool complete;
        public string last_modified;
        public string last_modified_by;
        public string updated_at;
    }


    //--Fon initialize 

    [Serializable]
    public class EnvironmentInfo
    {
        public string id;
        public string projectid;
        public string name;
    }

    [Serializable]
    public class EnvironmentListWrapper
    {
        public List<EnvironmentInfo> environments;
    }


    [Serializable]
    public class CCDEnvironment
    {
        public string id;
        public string name;
    }

    [Serializable]
    public class CCDEnvironmentList
    {
        public List<CCDEnvironment> environments;
    }

    [Serializable]
    public class CCDBucket
    {
        public string id;
        public string name;
    }

    [Serializable]
    public class CCDBucketList
    {
        public List<CCDBucket> buckets;
    }
}


/*using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public class CCDUploaderWindow : EditorWindow
{
    private const string UXML_PATH = "Assets/FlingTamplate/Editor/AssetUploader/CCDUploader.uxml";

    private const string PREF_KEY_ID = "CCD_KeyId";
    private const string PREF_SECRET = "CCD_SecretKey";
    private const string PREF_PROJECT = "CCD_ProjectId";
    private const string PREF_ENV = "CCD_EnvId";
    private const string PREF_BUCKET = "CCD_BucketId";

    private string PREF_KEY_DEFAULT = "0db6e081-4edd-43ff-88fb-6121ece54740";
    private string PREF_SECRET_DEFAULT = "pDM51SOq6ZsqmsXfO887BIMkv0MZb4zS";
    private string PREF_PROJECT_DEFAULT = "1b7fad74-3e1d-4586-918c-30b9447321eb";
    private string PREF_ENV_DEFAULT = "1c86c837-b303-40ec-ab33-2ae84e9d9ee7";
    private string PREF_BUCKET_DEFAULT = "620ad1cb-f87e-4584-81c9-2e529a96aff9";


    [MenuItem("Flickle/CCDUploader")]
    public static void ShowWindow()
    {
        var w = GetWindow<CCDUploaderWindow>("CCD Uploader");
        //var window = GetWindow<CCDUploaderWindow>();
        // var window = GetWindow<CCDUploaderWindow>();
        w.titleContent = new GUIContent("CCD Uploader", EditorGUIUtility.IconContent("CloudConnect").image);
    }

    private TextField keyIdField, secretKeyField, projectIdField, envIdField, bucketIdField;


    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        visualTree.CloneTree(rootVisualElement);

        keyIdField = rootVisualElement.Q<TextField>("KeyIdField");
        secretKeyField = rootVisualElement.Q<TextField>("SecretKeyField");
        projectIdField = rootVisualElement.Q<TextField>("ProjectIdField");
        envIdField = rootVisualElement.Q<TextField>("EnvIdField");
        bucketIdField = rootVisualElement.Q<TextField>("BucketIdField");

        LoadPrefs();

        rootVisualElement.Q<Button>("UploadButton").clicked += () => Upload().Forget();
    }

    private void LoadPrefs()
    {
        EditorPrefs.DeleteKey(PREF_KEY_ID);
        EditorPrefs.DeleteKey(PREF_SECRET);
        EditorPrefs.DeleteKey(PREF_PROJECT);
        EditorPrefs.DeleteKey(PREF_ENV);
        EditorPrefs.DeleteKey(PREF_BUCKET);
        keyIdField.value = EditorPrefs.GetString(PREF_KEY_ID, PREF_KEY_DEFAULT);
        secretKeyField.value = EditorPrefs.GetString(PREF_SECRET, PREF_SECRET_DEFAULT);
        projectIdField.value = EditorPrefs.GetString(PREF_PROJECT, PREF_PROJECT_DEFAULT);
        envIdField.value = EditorPrefs.GetString(PREF_ENV, PREF_ENV_DEFAULT);
        bucketIdField.value = EditorPrefs.GetString(PREF_BUCKET, PREF_BUCKET_DEFAULT);
    }

    private void SavePrefs()
    {
        EditorPrefs.SetString(PREF_KEY_ID, keyIdField.value);
        EditorPrefs.SetString(PREF_SECRET, secretKeyField.value);
        EditorPrefs.SetString(PREF_PROJECT, projectIdField.value);
        EditorPrefs.SetString(PREF_ENV, envIdField.value);
        EditorPrefs.SetString(PREF_BUCKET, bucketIdField.value);
    }

    private async UniTaskVoid Upload()
    {
        SavePrefs();

        //string filePath = EditorUtility.OpenFilePanel("Select file to upload", "Assets", "*.*");
        string startPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

        string filePath = EditorUtility.OpenFilePanel("Select file to upload", startPath, "*.*");
        if (string.IsNullOrEmpty(filePath)) return;

        byte[] fileBytes = File.ReadAllBytes(filePath);
        string contentHash = GetMD5(fileBytes);
        string fileName = Path.GetFileName(filePath);

        string url =
            $"https://services.api.unity.com/ccd/management/v1/projects/{projectIdField.value}/environments/{envIdField.value}/buckets/{bucketIdField.value}/entries/";
        string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{keyIdField.value}:{secretKeyField.value}"));

        var json = JsonUtility.ToJson(new CCDEntry
        {
            path = fileName,
            content_hash = contentHash,
            content_size = fileBytes.Length,
            content_type = "application/octet-stream",
            signed_url = true
        });

        using var req = UnityWebRequest.PostWwwForm(url, "");
        req.method = UnityWebRequest.kHttpVerbPOST;
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Authorization", $"Basic {auth}");
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Entry creation failed: {req.error}\n{req.downloadHandler.text}");
            return;
        }

        var response = JsonUtility.FromJson<CCDEntryResponse>(req.downloadHandler.text);
        await UploadToSignedUrl(response.signed_url, fileBytes, response.content_type);
    }

    private async UniTask UploadToSignedUrl(string signedUrl, byte[] data, string contentType)
    {
        using var uploadReq = UnityWebRequest.Put(signedUrl, data);
        uploadReq.SetRequestHeader("Content-Type", contentType);
        await uploadReq.SendWebRequest();

        if (uploadReq.result == UnityWebRequest.Result.Success)
            Debug.Log("Upload successful!");
        else
            Debug.LogError("Upload failed: " + uploadReq.error);
    }

    private string GetMD5(byte[] bytes)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    [Serializable]
    private class CCDEntry
    {
        public string path;
        public string content_hash;
        public long content_size;
        public string content_type;
        public bool signed_url;
    }

    [Serializable]
    private class CCDEntryResponse
    {
        public string signed_url;
        public string content_type;
    }
}*/