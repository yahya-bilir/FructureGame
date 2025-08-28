using Characters;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace WeaponSystem.AmmoSystem.CustomAmmos
{
    public class CatapultAmmo : AmmoHomingBase
    {
        private IEventBus _eventBus;
        private MeshRenderer _renderer;

        protected override void Awake()
        {
            base.Awake();
            _renderer = GetComponent<MeshRenderer>();
        }

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _eventBus.Subscribe<OnCharacterAttacked>(OnCharacterAttacked);
        }

        protected override void TryProcessTrigger(Collider other, bool isEntering)
        {
            
        }

        protected override void PlayTween(Character target)
        {
            
        }

        private void OnCharacterAttacked(OnCharacterAttacked eventData)
        {
            if(eventData.AttackedCharacter != ConnectedCombatManager.Character) return;
            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _eventBus.Unsubscribe<OnCharacterAttacked>(OnCharacterAttacked);
        }
    }
}