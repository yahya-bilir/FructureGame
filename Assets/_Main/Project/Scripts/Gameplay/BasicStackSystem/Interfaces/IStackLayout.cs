using UnityEngine;

namespace BasicStackSystem
{
    public interface IStackLayout
    {
        Vector3 GetLocalPosition(int index);
        int GetRow(int index);
        int GetColumn(int index);
        int GetVerticalLayer(int index);
    }
}