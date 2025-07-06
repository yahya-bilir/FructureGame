using System.Collections.Generic;
using EventBusses;
using Events;
using Events.ClickableEvents;
using Events.IslandEvents;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace UI.PerksAndDraggables.PerkManagers
{
    public class PerkCreator : MonoBehaviour
    {
        [SerializeField] private List<Clickable> availableClickables;
        private int _initiatedClickableCount;
        private int _initiatedPerkCount;
        private MiddlePerkManager _middlePerkManager;
        private BottomPerkManager _bottomPerkManager;
        private IObjectResolver _objectResolver;
        private IEventBus _eventBus;

        [Inject]
        private void Inject(IEventBus eventBus, MiddlePerkManager middlePerkManager, BottomPerkManager bottomPerkManager, IObjectResolver objectResolver)
        {
            _eventBus = eventBus;
            _middlePerkManager = middlePerkManager;
            _objectResolver = objectResolver;
            _bottomPerkManager = bottomPerkManager;
        }
        private void OnEnable()
        {
            _eventBus.Subscribe<OnAllIslandEnemiesKilled>(CreateClickables);
            _eventBus.Subscribe<OnClickableClicked>(OnClickableClicked);
            _eventBus.Subscribe<OnDraggableDroppedToScene>(OnDraggableDroppedToScene);
        }

        private void CreateClickables(OnAllIslandEnemiesKilled eventData)
        {
            // Önce available listeden kopya bir liste oluştur
            List<Clickable> pool = new List<Clickable>(availableClickables);

            // Eğer liste 3'ten küçükse hata önle
            int spawnCount = Mathf.Min(3, pool.Count);

            for (int i = 0; i < spawnCount; i++)
            {
                // Rastgele bir index seç
                int randomIndex = UnityEngine.Random.Range(0, pool.Count);

                // O clickable prefabını al
                var clickablePrefab = pool[randomIndex];

                // Instantiate et
                var clickable = _objectResolver.Instantiate(clickablePrefab, transform);

                // Orta Perk Manager'a ekle
                _eventBus.Publish(new OnClickableCreated(clickable));

                // Seçilen clickable'ı pool'dan çıkar ki tekrar seçilmesin
                pool.RemoveAt(randomIndex);
            }

            _initiatedClickableCount = 3;
        }


        private void OnClickableClicked(OnClickableClicked eventData)
        {
            _initiatedClickableCount -= 1;
            if (_initiatedClickableCount == 0)
            {
                Debug.Log("remaining clickable count is 0");
                _eventBus.Publish(new OnAllClickablesClicked());
                _initiatedPerkCount = 3;
            }
                
            if (eventData.Clickable is Draggable)
            {
                _middlePerkManager.RemoveClickableFromList(eventData.Clickable);
                _bottomPerkManager.TransferDraggableToBottom(eventData.Clickable as Draggable);
                return;
            }
            
            
            //todo burada eğer sadece clickable ise yapılacak askiyonlar
        }

        private void OnDraggableDroppedToScene(OnDraggableDroppedToScene eventData)
        {
            _initiatedPerkCount -= 1;
            if (_initiatedPerkCount == 0) _eventBus.Publish(new OnAllPerksSelected());
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnAllIslandEnemiesKilled>(CreateClickables);
            _eventBus.Unsubscribe<OnClickableClicked>(OnClickableClicked);
            _eventBus.Unsubscribe<OnDraggableDroppedToScene>(OnDraggableDroppedToScene);
            
        }
    }
}