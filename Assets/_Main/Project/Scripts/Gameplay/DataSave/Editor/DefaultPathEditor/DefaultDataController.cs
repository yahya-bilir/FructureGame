using Cysharp.Threading.Tasks;
using DataSave.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FlingTamplate.DataSave
{
    public class DefaultDataController
    {
        private EditorDataHolder _editorDataHolder;
        private VisualElement _root;

        private GameData _gameData;

//
        private ListView _listView;
        private Foldout _mainFoldout;
        private VisualElement _saveAsDefaultDataContainer;

        public DefaultDataController(
            EditorDataHolder editorDataHolder,
            GameData gameData,
            VisualElement root)
        {
            _editorDataHolder = editorDataHolder;
            _gameData = gameData;
            _root = root;
            CreateDefaultDataList();
        }

        private void CreateDefaultDataList()
        {
            CreateFoldout();
            _listView = new ListView(_editorDataHolder.DefaultDataPathes, 20,
                () =>
                {
                    var fieldsContainer = new VisualElement();
                    fieldsContainer.style.flexDirection = FlexDirection.Row;
                    fieldsContainer.style.unityTextAlign = TextAnchor.MiddleCenter;
                    var label = new Label();
                    label.style.color = Color.red;
                    label.name = "PathLabel";
                    // label.Bind(_serializedEditorData);

                    var buttons = new VisualElement();
                    buttons.style.flexDirection = FlexDirection.Row;

                    var loadButton = new Button();
                    loadButton.text = "Load";
                    loadButton.style.width = 50;
                    loadButton.name = "LoadBtn";
                    buttons.Add(loadButton);

                    var selectFolderBtn = new Button();
                    selectFolderBtn.text = "Update";
                    selectFolderBtn.style.width = 35;
                    selectFolderBtn.style.color = Color.yellow;
                    selectFolderBtn.name = "SaveBtn";
                    buttons.Add(selectFolderBtn);

                    var openFolderBtn = new Button();
                    openFolderBtn.text = "Open";
                    openFolderBtn.style.width = 30;
                    openFolderBtn.style.color = Color.green;
                    openFolderBtn.name = "OpenBtn";
                    buttons.Add(openFolderBtn);

                    var delete = new Button();
                    delete.text = "Delete";
                    delete.style.width = 40;
                    delete.style.color = Color.red;
                    delete.name = "DeleteBtn";

                    buttons.Add(delete);


                    fieldsContainer.Add(label);
                    fieldsContainer.Add(buttons);

                    return fieldsContainer;
                }, (element, index) =>
                {
                    var pathLabel = element.Q<Label>("PathLabel");
                    var loadBtn = element.Q<Button>("LoadBtn");
                    var updateBtn = element.Q<Button>("SaveBtn");
                    var openBtn = element.Q<Button>("OpenBtn");
                    var deleteBtn = element.Q<Button>("DeleteBtn");

                    pathLabel.text = GetPathName(index);
                    loadBtn.RegisterCallback<ClickEvent, int>(LoadDefaultData, index);
                    updateBtn.RegisterCallback<ClickEvent, int>(UpdateDefaultData, index);

                    deleteBtn.RegisterCallback((ClickEvent e) =>
                    {
                        var fileName = _editorDataHolder.GetFileName(index);
                        DefaultDataSaveController.DeleteDefaultData(_editorDataHolder.ProjectFilePath, fileName)
                            .Forget();
                        _editorDataHolder.RemoveFileName(index);
                        _listView.Rebuild();
                    });
                });
            _saveAsDefaultDataContainer = SetupSaveAsDefaultDataContainer();

            _mainFoldout.Add(_saveAsDefaultDataContainer);
            _mainFoldout.Add(_listView);
            _root.Add(_mainFoldout);
        }

        private void UpdateDefaultData(ClickEvent evt, int index)
        {
            var fileName = _editorDataHolder.GetFileName(index);
            DefaultDataSaveController.SaveDefaultData(_gameData, _editorDataHolder.ProjectFilePath, fileName);
            _editorDataHolder.AddNewDefaultData(fileName);
            _listView.itemsSource = _editorDataHolder.DefaultDataPathes;
            _listView.Rebuild();
        }

        private void LoadDefaultData(ClickEvent evt, int index)
        {
            Debug.Log("LoadData  Index: " + index);
            var fileName = _editorDataHolder.GetFileName(index);
            DefaultDataSaveController.LoadDefaultData(_gameData, _editorDataHolder.ProjectFilePath, fileName);
            EditorUtility.SetDirty(_gameData);
        }


        private VisualElement SetupSaveAsDefaultDataContainer()
        {
            var container = VisualElementFactory.CreateSaveAsDefaultDataContainer();

            container.Q<Button>("AddBtn").RegisterCallback<ClickEvent>((e) =>
            {
                string fileName = container.Q<TextField>("FileName").text;
                DefaultDataSaveController.SaveDefaultData(_gameData, _editorDataHolder.ProjectFilePath, fileName);
                _editorDataHolder.AddNewDefaultData(fileName);
                _listView.itemsSource = _editorDataHolder.DefaultDataPathes;
                _listView.Rebuild();
            });

            return container;
        }

        private void CreateFoldout()
        {
            _mainFoldout = new Foldout();
            _mainFoldout.text = "Default Datas";

            _mainFoldout.style.unityTextAlign = TextAnchor.MiddleCenter;
            _mainFoldout.style.unityTextOutlineColor = Color.black;
            _mainFoldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            _mainFoldout.style.marginBottom = 20;
            _mainFoldout.style.fontSize = 20;
            _mainFoldout.style.backgroundColor = new StyleColor(
                new Color(0.1f, 0.1f, 0.1f, 0.5f));
            _mainFoldout.style.minHeight = 40;
        }

        private string GetPathName(int index)
        {
            return _editorDataHolder.DefaultDataPathes[index];
        }
    }
}