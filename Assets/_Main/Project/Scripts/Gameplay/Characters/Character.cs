using System.Linq;
using Characters.Enemy;
using DataSave.Runtime;
using Factions;
using PropertySystem;
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
        [SerializeField] private CharacterProperties characterProperties;
        [SerializeField] protected UIPercentageFiller healthBar;
        [SerializeField] protected ParticleSystem onDeathVfx;
        [SerializeField] private Transform weaponEquippingField;
        
        [Header("Debug")]
        [field: SerializeField] public Faction Faction { get; private set; }

        
        private Animator _animator;
        protected CharacterPropertyManager CharacterPropertyManager;
        protected CharacterVisualEffects CharacterVisualEffects;
        protected CharacterSpeedController CharacterSpeedController;
        protected CharacterWeaponManager CharacterWeaponManager;
        protected SpriteRenderer[] _childrenSpriteRenderers;
        private ShineEffect _shineEffect;
        protected CharacterAnimationController AnimationController;
        public bool IsCharacterDead => CharacterPropertyManager.GetProperty(PropertyQuery.Health).TemporaryValue <= 0;
        
        private IObjectResolver _resolver;

        
        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        protected virtual void Awake()
        {
            GetComponents();
            AnimationController = new CharacterAnimationController(_animator);
            CharacterVisualEffects = new CharacterVisualEffects(_childrenSpriteRenderers.ToList(), CharacterDataHolder, healthBar, onDeathVfx);
            CharacterPropertyManager = new CharacterPropertyManager(characterProperties);
            CharacterCombatManager = new CharacterCombatManager(CharacterPropertyManager, CharacterVisualEffects, this);
            CharacterSpeedController = new CharacterSpeedController(CharacterPropertyManager, CharacterDataHolder, this);
            CharacterWeaponManager = new CharacterWeaponManager(weaponEquippingField, CharacterPropertyManager, CharacterCombatManager, CharacterDataHolder.Weapon);
        }
        
        protected virtual void Start()
        {
            ResolveOrInitializeCreatedObjects();
        }

        private void ResolveOrInitializeCreatedObjects()
        {
            _resolver.Inject(CharacterPropertyManager);
            _resolver.Inject(CharacterCombatManager);
            _resolver.Inject(CharacterSpeedController);
            _resolver.Inject(CharacterWeaponManager);
        }

        protected virtual void GetComponents()
        {
            _animator = model.GetComponent<Animator>();
            _childrenSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
        
        public void InitializeOnSpawn(Faction currentFaction)
        {
            Faction = currentFaction;
        }
    }
}