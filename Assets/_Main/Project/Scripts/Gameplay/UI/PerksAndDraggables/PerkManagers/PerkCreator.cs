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
        private List<Clickable> _currentClickables = new();
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
            _currentClickables = new List<Clickable>();
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
                
                _currentClickables.Add(clickable);

                // Orta Perk Manager'a ekle
                _eventBus.Publish(new OnClickableCreated(clickable));

                // Seçilen clickable'ı pool'dan çıkar ki tekrar seçilmesin
                pool.RemoveAt(randomIndex);
            }

            _initiatedClickableCount = 1;
        }


        private void OnClickableClicked(OnClickableClicked eventData)
        {
            if (eventData.Clickable is Draggable)
            {
                //_middlePerkManager.RemoveClickableFromList(eventData.Clickable);
                _bottomPerkManager.TransferDraggableToBottom(eventData.Clickable as Draggable);
            }

            List<Clickable> clickablesToBeDestroyed = new List<Clickable>();
            foreach (var clickable in _currentClickables)
            {
                _middlePerkManager.RemoveClickableFromList(clickable);
                
                if(clickable == eventData.Clickable) continue;
                clickablesToBeDestroyed.Add(clickable);
                clickable.DestroyClickableWithAnimation();
            }

            foreach (var clickable in clickablesToBeDestroyed)
            {
                _currentClickables.Remove(clickable);
            }
                        
            _eventBus.Publish(new OnAllClickablesClicked());
            //todo burada eğer sadece clickable ise yapılacak askiyonlar
        }

        private void OnDraggableDroppedToScene(OnDraggableDroppedToScene eventData)
        {
            _initiatedPerkCount -= 1;
            Debug.Log(_initiatedPerkCount + " | Initiated Perk Count");
            if (_initiatedPerkCount <= 0) _eventBus.Publish(new OnAllPerksSelected());
        }

        private void OnDisable()
        {
            _eventBus.Unsubscribe<OnAllIslandEnemiesKilled>(CreateClickables);
            _eventBus.Unsubscribe<OnClickableClicked>(OnClickableClicked);
            _eventBus.Unsubscribe<OnDraggableDroppedToScene>(OnDraggableDroppedToScene);
            
        }
    }
}