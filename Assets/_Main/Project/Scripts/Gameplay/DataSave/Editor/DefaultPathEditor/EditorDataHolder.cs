using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlingTamplate.DataSave
{
    [CreateAssetMenu(fileName = "Data", menuName = "Data Save/EditorDataHolder", order = 1)]
    public class EditorDataHolder : ScriptableObject
    {
        public string ProjectFilePath = "/FlingTamplate/DataSave/DefaultDatas/";
        public List<string> DefaultDataPathes;

        public void AddNewDefaultData(string fileName)
        {
            if (DefaultDataPathes.Contains(fileName)) return;
            DefaultDataPathes.Add(fileName);
            EditorUtility.SetDirty(this);
        }

        public string GetFileName(int index)
        {
            return DefaultDataPathes[index];
        }

        public bool RemoveFileName(int index)
        {
            if (index >= DefaultDataPathes.Count) return false;
            DefaultDataPathes.RemoveAt(index);
            EditorUtility.SetDirty(this);
            return true;
        }
    }
}