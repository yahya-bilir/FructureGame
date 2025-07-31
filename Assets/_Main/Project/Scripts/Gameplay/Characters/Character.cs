using Factions;
using MoreMountains.Feedbacks;
using PropertySystem;
using TMPro;
using UI;
using UnityEngine;
using VContainer;
using WeaponSystem.Managers;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        [Header("Debug")]
        [field: SerializeField] public TextMeshPro AIText { get; private set; }

        [Header("References")]
        [SerializeField] protected GameObject model;
        [SerializeField] private Transform weaponEquippingField;
        [SerializeField] protected UIPercentageFiller healthBar;
        [SerializeField] protected ParticleSystem onDeathVfx;
        [SerializeField] protected ParticleSystem hitVfx;

        [Header("Data")]
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [field: SerializeField] protected CharacterPropertiesSO CharacterPropertiesSo { get; private set; }
        [field: SerializeField] public Faction Faction { get; private set; }

        [Header("Runtime")]
        public CharacterCombatManager CharacterCombatManager { get; protected set; }
        public CharacterPropertyManager CharacterPropertyManager { get; private set; }
        public CharacterVisualEffects CharacterVisualEffects { get; protected set; }
        protected CharacterSpeedController CharacterSpeedController;
        protected CharacterWeaponManager CharacterWeaponManager;
        protected CharacterAnimationController AnimationController;
        private Animator _animator;
        protected MMF_Player Feedback;
        protected IObjectResolver Resolver;

        public bool IsCharacterDead => CharacterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue <= 0;

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            Resolver = resolver;
        }

        protected virtual void Awake()
        {
            GetComponents();
            CharacterPropertyManager = new CharacterPropertyManager(CharacterPropertiesSo);
            AnimationController = new CharacterAnimationController(_animator);
            CharacterVisualEffects = new CharacterVisualEffects(healthBar, onDeathVfx, this, AnimationController, hitVfx, Feedback);
            CharacterSpeedController = new CharacterSpeedController(CharacterPropertyManager, CharacterDataHolder, this);
        }

        protected virtual void Start()
        {
            CharacterCombatManager = new CharacterCombatManager(CharacterPropertyManager, CharacterVisualEffects, this);
            CharacterWeaponManager = new CharacterWeaponManager(weaponEquippingField, CharacterPropertyManager, CharacterCombatManager, CharacterDataHolder.Weapon, this);

            ResolveOrInitializeCreatedObjects();
        }

        private void ResolveOrInitializeCreatedObjects()
        {
            Resolver.Inject(CharacterPropertyManager);
            Resolver.Inject(CharacterCombatManager);
            Resolver.Inject(CharacterSpeedController);
            Resolver.Inject(CharacterWeaponManager);
            Resolver.Inject(CharacterVisualEffects);
        }

        protected virtual void GetComponents()
        {
            _animator = model.GetComponent<Animator>();
            Feedback = GetComponent<MMF_Player>();
        }

        public void InitializeOnSpawn(Faction faction)
        {
            Faction = faction;
        }
    }
    
}