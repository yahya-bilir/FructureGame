using System.Linq;
using DataSave.Runtime;
using PropertySystem;
using UI;
using UnityEngine;
using Utils;
using VContainer;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        public CharacterCombatManager CharacterCombatManager { get; protected set; }
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [SerializeField] protected GameObject model;
        [SerializeField] private CharacterProperties characterProperties;
        [SerializeField] private UIPercentageFiller healthBar;

        private Animator _animator;
        protected CharacterPropertyManager CharacterPropertyManager;
        private CharacterVisualEffects _characterVisualEffects;
        
        private SpriteRenderer[] _childrenSpriteRenderers;
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
            _characterVisualEffects = new CharacterVisualEffects(_childrenSpriteRenderers.ToList(), CharacterDataHolder, healthBar);
            CharacterPropertyManager = new CharacterPropertyManager(characterProperties);
            CharacterCombatManager = new CharacterCombatManager(CharacterPropertyManager, _characterVisualEffects, this);
        }
        
        protected virtual void Start()
        {
            ResolveOrInitializeCreatedObjects();
        }

        private void ResolveOrInitializeCreatedObjects()
        {
            _resolver.Inject(CharacterCombatManager);
            _resolver.Inject(CharacterPropertyManager);
            CharacterPropertyManager.Initialize();
        }

        protected virtual void GetComponents()
        {
            _animator = model.GetComponent<Animator>();
            _childrenSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }
    }
}