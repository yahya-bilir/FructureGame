using UnityEngine;

namespace Initialization
{
    public class GameEntryCoordinator : MonoBehaviour
    {
        [SerializeField] protected MonoBehaviour[] entryPoints;

        protected virtual async void Awake()
        {
            foreach (var entry in entryPoints)
            {
                entry.TryGetComponent<IGameEntry>(out var gameEntry);
                    await gameEntry.InitializeAsync();
            }
        }
    }
}