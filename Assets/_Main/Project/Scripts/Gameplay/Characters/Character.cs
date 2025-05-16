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

        [Inject]
        private void Inject(GameData gameData)
        {
            _gameData = gameData;
        }
        protected virtual void Awake()
        {
            animator = model.GetComponent<Animator>();
            CharacterPropertyManager = new CharacterPropertyManager(characterProperties, _gameData);
        }
    }
}