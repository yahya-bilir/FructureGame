using System.Linq;
using Characters.Enemy;
using Factions;
using PropertySystem;
using TMPro;
using UI;
using UnityEngine;
using Utils;
using VContainer;
using WeaponSystem.Managers;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        [Header("AI Debugger")]
        [field: SerializeField] public TextMeshPro AIText { get; private set; }
        public CharacterCombatManager CharacterCombatManager { get; protected set; }
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [SerializeField] protected GameObject model;
        [field: SerializeField] public CharacterPropertiesSO CharacterPropertiesSo { get; private set; }
        [SerializeField] protected UIPercentageFiller healthBar;
        [SerializeField] protected ParticleSystem onDeathVfx;
        [SerializeField] private ParticleSystem hitVfx;
        
        [SerializeField] private Transform weaponEquippingField;
        [field: SerializeField] public Faction Faction { get; private set; }

        private Animator _animator;
        protected CharacterPropertyManager CharacterPropertyManager;
        public CharacterVisualEffects CharacterVisualEffects { get; protected set; }
        protected CharacterSpeedController CharacterSpeedController;
        protected CharacterWeaponManager CharacterWeaponManager;
        protected SpriteRenderer[] ChildrenSpriteRenderers;
        private ShineEffect _shineEffect;
        protected CharacterAnimationController AnimationController;
        public bool IsCharacterDead => CharacterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue <= 0;

        protected IObjectResolver Resolver;

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
            CharacterVisualEffects = new CharacterVisualEffects(ChildrenSpriteRenderers.ToList(), CharacterDataHolder, healthBar, onDeathVfx, this, AnimationController, hitVfx);
            CharacterCombatManager = new CharacterCombatManager(CharacterPropertyManager, CharacterVisualEffects, this);
            CharacterSpeedController = new CharacterSpeedController(CharacterPropertyManager, CharacterDataHolder, this);
            CharacterWeaponManager = new CharacterWeaponManager(weaponEquippingField, CharacterPropertyManager, CharacterCombatManager, CharacterDataHolder.Weapon, this);
        }

        protected virtual void Start()
        {
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
            ChildrenSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        public void InitializeOnSpawn(Faction faction)
        {
            Faction = faction;
        }
    }
    
}