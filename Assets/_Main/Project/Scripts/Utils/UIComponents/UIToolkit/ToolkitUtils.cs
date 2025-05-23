using UnityEngine.UIElements;

namespace Utils.UIComponents.UIToolkit
{
    public static class ToolkitUtils
    {
        #region Class Change

        public static void ChangeClasses(VisualElement visualElement, string[] classesToAdd, string[] classesToRemove)
        {
            foreach (var classToRemove in classesToRemove)
            {
                visualElement.RemoveFromClassList(classToRemove);
            }
            foreach (var classToAdd in classesToAdd)
            {
                visualElement.AddToClassList(classToAdd);
            }
        }        
        
        public static void ChangeClasses(VisualElement visualElement, string classToAdd, string[] classesToRemove)
        {
            foreach (var classToRemove in classesToRemove)
            {
                visualElement.RemoveFromClassList(classToRemove);
            }
    
            visualElement.AddToClassList(classToAdd);
        }
        
        public static void ChangeClasses(VisualElement visualElement, string[] classesToAdd, string classToRemove)
        {
            visualElement.RemoveFromClassList(classToRemove);
            
            foreach (var classToAdd in classesToAdd)
            {
                visualElement.AddToClassList(classToAdd);
            }
        }           
        
        public static void ChangeClasses(VisualElement visualElement, string classToAdd, string classToRemove)
        {
            visualElement.RemoveFromClassList(classToRemove);
            visualElement.AddToClassList(classToAdd);
        }     

        #endregion
        
    }
}