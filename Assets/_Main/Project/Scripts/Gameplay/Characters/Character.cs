using System.Linq;
using Characters.Enemy;
using Characters.Transforming;
using Factions;
using IslandSystem;
using PropertySystem;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Utils;
using VContainer;
using WeaponSystem.Managers;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        public CharacterCombatManager CharacterCombatManager { get; protected set; }
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [SerializeField] protected GameObject model;
        [field: SerializeField] public CharacterPropertiesSO CharacterPropertiesSo { get; private set; }
        [SerializeField] protected UIPercentageFiller healthBar;
        [SerializeField] protected ParticleSystem onDeathVfx;
        [SerializeField] private ParticleSystem hitVfx;
        
        [SerializeField] private Transform weaponEquippingField;
        
        [Header("Debug")]
        [field: SerializeField] public Faction Faction { get; private set; }

        
        private Animator _animator;
        protected CharacterPropertyManager CharacterPropertyManager;
        public CharacterVisualEffects CharacterVisualEffects { get; protected set; }
        protected CharacterSpeedController CharacterSpeedController;
        protected CharacterWeaponManager CharacterWeaponManager;
        public CharacterIslandController CharacterIslandController { get; private set; }
        protected SpriteRenderer[] ChildrenSpriteRenderers;
        private ShineEffect _shineEffect;
        protected CharacterAnimationController AnimationController;
        public bool IsCharacterDead => CharacterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue <= 0;

        protected IObjectResolver Resolver;
        private CharacterTransformManager _characterTransformManager;
        private AttackAnimationCaller _attackAnimationCaller;

        [Inject]
        private void Inject(IObjectResolver resolver, CharacterTransformManager characterTransformManager)
        {
            Resolver = resolver;
            _characterTransformManager = characterTransformManager;
            
        }

        protected virtual void Awake()
        {
            GetComponents();
            AnimationController = new CharacterAnimationController(_animator);
            CharacterVisualEffects = new CharacterVisualEffects(ChildrenSpriteRenderers.ToList(), CharacterDataHolder, healthBar, onDeathVfx, this, AnimationController, hitVfx);
            CharacterPropertyManager = new CharacterPropertyManager(CharacterPropertiesSo);
            CharacterCombatManager = new CharacterCombatManager(CharacterPropertyManager, CharacterVisualEffects, this);
            CharacterSpeedController = new CharacterSpeedController(CharacterPropertyManager, CharacterDataHolder, this);
            CharacterWeaponManager = new CharacterWeaponManager(weaponEquippingField, CharacterPropertyManager, CharacterCombatManager, CharacterDataHolder.Weapon, this);
            CharacterIslandController = new CharacterIslandController(this);
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
            Resolver.Inject(_attackAnimationCaller);
        }

        protected virtual void GetComponents()
        {
            _animator = model.GetComponent<Animator>();
            ChildrenSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _attackAnimationCaller = GetComponentInChildren<AttackAnimationCaller>();
        }

        public void InitializeOnSpawn(Faction currentFaction)
        {
            Faction = currentFaction;
        }
        
        [Button(ButtonSizes.Medium)]
        private void TryUpgradeFromEditor()
        {
#if UNITY_EDITOR
            if (_characterTransformManager != null)
            {
                _characterTransformManager.TryUpgradeCharacter(this);
            }
            else
            {
                Debug.LogWarning("CharacterTransformManager not injected.");
            }
#endif
        }
    }
    
}