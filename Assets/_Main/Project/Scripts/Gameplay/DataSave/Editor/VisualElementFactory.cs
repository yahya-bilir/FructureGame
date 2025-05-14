using UnityEngine;
using UnityEngine.UIElements;

public static class VisualElementFactory
{
    public static VisualElement CreateSaveAsDefaultDataContainer()
    {
        var fieldsContainer = new VisualElement();
        fieldsContainer.style.flexDirection = FlexDirection.Row;
        fieldsContainer.style.unityTextAlign = TextAnchor.MiddleCenter;

        var textField = new TextField();
        textField.style.width = 200;
        textField.name = "FileName";
        //   textField.value = "Data info";

        var addBtn = new Button();
        addBtn.text = "Add";
        addBtn.style.width = 50;
        addBtn.style.color = Color.green;
        addBtn.name = "AddBtn";
        // var openBtn = new Button();
        // openBtn.text = "O";
        // openBtn.style.width = 50;
        // openBtn.style.color = Color.blue;


        fieldsContainer.Add(textField);
        fieldsContainer.Add(addBtn);
        //fieldsContainer.Add(openBtn);


        return fieldsContainer;
    }
}