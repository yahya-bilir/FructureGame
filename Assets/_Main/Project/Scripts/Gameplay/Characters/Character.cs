using System.Collections.Generic;
using System.Linq;
using _Main.Project.Scripts.Utils;
using DataSave.Runtime;
using PropertySystem;
using UnityEngine;
using VContainer;

namespace Characters
{
    public class Character : MonoBehaviour
    {
        public CharacterCombatManager CharacterCombatManager { get; protected set; }
        [field: SerializeField] public CharacterDataHolder CharacterDataHolder { get; private set; }
        [SerializeField] protected GameObject model;
        [SerializeField] private CharacterProperties characterProperties;

        protected Animator animator;
        protected CharacterPropertyManager CharacterPropertyManager;
        private GameData _gameData;
        
        private SpriteRenderer[] _childrenSpriteRenderers;
        private ShineEffect _shineEffect;

        [Inject]
        private void Inject(GameData gameData)
        {
            _gameData = gameData;
        }
        protected virtual void Awake()
        {
            animator = model.GetComponent<Animator>();
            _childrenSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _shineEffect = new ShineEffect(_childrenSpriteRenderers.ToList(), CharacterDataHolder.ShineColor, CharacterDataHolder.ShineDuration);

            CharacterPropertyManager = new CharacterPropertyManager(characterProperties, _gameData);
            CharacterCombatManager = new CharacterCombatManager(this, CharacterPropertyManager, CharacterDataHolder, _shineEffect);
        }
    }
}