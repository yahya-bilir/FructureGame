using Cysharp.Threading.Tasks;
using DataSave.Runtime;
using FlingTamplate.DataSave;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class GameDataEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTreeAsset;
    [SerializeField] private EditorDataHolder editorDataHolder;
    [SerializeField] private GameData gameData;

    private VisualElement _root;
    private DefaultDataController _defaultDataController;

    [MenuItem("Window/UI Toolkit/GameDataEditor")]
    public static void ShowExample()
    {
        GameDataEditor wnd = GetWindow<GameDataEditor>();
        wnd.titleContent = new GUIContent("GameDataEditor");
    }

    public void CreateGUI()
    {
        _root = rootVisualElement;
        _root.style.paddingLeft = 10;
        _root.style.paddingBottom = 10;
        _root.style.paddingRight = 10;
        _root.style.paddingTop = 10;
        //_root.style.backgroundColor = new StyleColor(new Color(56, 56, 56, 255));
        VisualElement ve = visualTreeAsset.Instantiate();
        _root.Add(ve);
        CreateDataElements();
        _defaultDataController = new DefaultDataController(editorDataHolder, gameData, _root);

        var objectField = new ObjectField();
        objectField.RegisterValueChangedCallback((calback) =>
        {
            string targetPath = AssetDatabase.GetAssetPath(objectField.value);
            Debug.Log(targetPath);
        });
        objectField.tooltip = "Select Default Data Folder";
        _root.Add(objectField);


        ForDebug();
        //LooadData();
        //  BindFields();
    }

    private void ForDebug()
    {
        var saveGD = new Button();
        saveGD.text = "SaveGD";
        saveGD.RegisterCallback<ClickEvent>((EventCallbackArgs) =>
        {
            GameDataSaveController.SaveNormal(gameData);
            //GameDataSaveController.SaveGameData(gameData).Forget();
        });
        _root.Add(saveGD);

        var loadGD = new Button();
        loadGD.text = "LoadGD";
        loadGD.RegisterCallback<ClickEvent>((EventCallbackArgs) =>
        {
            GameDataSaveController.LoadGameDataNormal(gameData);
            // GameDataSaveController.LoadGameData(gameData).Forget();
        });
        _root.Add(loadGD);
    }

    private void CreateDataElements()
    {
        var fieldsContainer = new ScrollView(); // ScrollView oluşturuluyor
        fieldsContainer.style.flexGrow = 1; // Görünümün tamamını kaplaması için stil ayarı
        _root.Add(fieldsContainer); // ScrollView kök eleman olarak ekleniyor

        SerializedObject serializedObject = new SerializedObject(gameData);
        SerializedProperty property = serializedObject.GetIterator();

        // Move to the first property
        bool hasNext = property.NextVisible(true);
        while (hasNext)
        {
            if (property.name == "m_Script")
            {
                hasNext = property.NextVisible(false);
                continue;
            }

            var field = new PropertyField(property.Copy());
            field.Bind(serializedObject);
            fieldsContainer.Add(field);

            hasNext = property.NextVisible(false);
        }
    }
}