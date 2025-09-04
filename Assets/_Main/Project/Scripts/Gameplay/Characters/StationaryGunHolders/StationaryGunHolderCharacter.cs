using AI.Base;
using AI.StationaryGunHolderStates;
using BasicStackSystem;
using Characters.CarrierAI;
using CollectionSystem;
using Cysharp.Threading.Tasks;
using EventBusses;
using UnityEngine;
using VContainer;
using WeaponSystem.RangedWeapons;

namespace Characters.StationaryGunHolders
{
    public class StationaryGunHolderCharacter : Character
    {
        protected StateMachine _stateMachine;
        private SearchingForEnemy _searchingState;
        private Attacking _attackingState;

        protected RangedWeaponWithExternalAmmo _rangedWeapon;
        protected Transform _weaponTransform;
        protected IEventBus _eventBus;
        private AmmoCreator _ammoCreator;
        private GunHolderEventHandler _gunHolderEventHandler;

        [SerializeField] private CarrierAIBehaviour[] carriers;
        [SerializeField] private Transform loadingPos;

        private PhysicsStack _stack;

        [Inject]
        private void Inject(IEventBus eventBus, AmmoCreator ammoCreator)
        {
            _eventBus = eventBus;
            _ammoCreator = ammoCreator;
        }

        protected override void Start()
        {
            base.Start();

            _stateMachine = new StateMachine();

            _rangedWeapon = CharacterWeaponManager.SpawnedWeapon as RangedWeaponWithExternalAmmo;
            _weaponTransform = _rangedWeapon?.transform;

            if (_rangedWeapon == null || _weaponTransform == null)
            {
                Debug.LogError("Weapon not properly initialized.");
                return;
            }

            _rangedWeapon.SetLoadingPos(loadingPos);
            _ammoCreator.OnRangedWeaponCreated(this, _rangedWeapon.AmmoLogicType).Forget();

            _gunHolderEventHandler = new GunHolderEventHandler(this, CharacterPropertyManager);
            Resolver.Inject(_gunHolderEventHandler);
            
            foreach (var carrierAIBehaviour in carriers)
            {
                carrierAIBehaviour.Initialize(_rangedWeapon);
                Resolver.Inject(carrierAIBehaviour);
            }

            SetStates();
        }

        protected virtual void SetStates()
        {
            _searchingState = new SearchingForEnemy(CharacterCombatManager, AIText, _rangedWeapon.Animator, name);
            _attackingState = new Attacking(CharacterCombatManager, _rangedWeapon, _weaponTransform, AIText, _rangedWeapon.Animator, _eventBus, _stack);

            var waitingForWeaponToBeLoaded = new WaitingForWeaponToBeLoaded(_rangedWeapon, AIText);

            _stateMachine.AddTransition(_searchingState, waitingForWeaponToBeLoaded, () => LastEnemyIsValid());
            _stateMachine.AddTransition(waitingForWeaponToBeLoaded, _attackingState, () => _rangedWeapon.IsLoaded && LastEnemyIsValid());
            _stateMachine.AddTransition(_attackingState, waitingForWeaponToBeLoaded, () => !_rangedWeapon.IsLoaded && LastEnemyIsValid());
            _stateMachine.AddAnyTransition(_searchingState, () => !LastEnemyIsValid());

            _stateMachine.SetState(_searchingState);
        }

        private void Update() => _stateMachine.Tick();

        private bool LastEnemyIsValid()
        {
            var enemy = CharacterCombatManager.LastFoundEnemy;
            return enemy != null && !enemy.IsCharacterDead && !IsCharacterDead;
        }
    }
}
