using UnityEngine;

namespace BasicStackSystem
{
    public sealed class GridStackLayout : IStackLayout
    {
        private readonly StackAreaSO _so;
        public GridStackLayout(StackAreaSO so) => _so = so;

        public Vector3 GetLocalPosition(int index)
        {
            int col = index % _so.MaxItemsInColumn;
            int vertical = index / (_so.MaxItemsInColumn * _so.MaxItemsInRow);
            int row = (index % (_so.MaxItemsInColumn * _so.MaxItemsInRow)) / _so.MaxItemsInColumn;

            return new Vector3(
                _so.ItemInitialPosition.x + (row * _so.Increments.x),
                _so.ItemInitialPosition.y + (vertical * _so.Increments.y),
                _so.ItemInitialPosition.z + (col * _so.Increments.z)
            );
        }

        public int GetRow(int index) => (index % (_so.MaxItemsInColumn * _so.MaxItemsInRow)) / _so.MaxItemsInColumn;
        public int GetColumn(int index) => index % _so.MaxItemsInColumn;
        public int GetVerticalLayer(int index) => index / (_so.MaxItemsInColumn * _so.MaxItemsInRow);
    }
}