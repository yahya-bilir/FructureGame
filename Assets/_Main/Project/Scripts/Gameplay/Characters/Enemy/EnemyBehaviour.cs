using AI.Base;
using DataSave.Runtime;
using Pathfinding;
using UnityEngine;

namespace Characters.Enemy
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject model;
        [SerializeField] private CharacterProperties characterProperties;
        
        private StateMachine _stateMachine;
        private Animator _animator;
        
        private AIPath _aiPath;
        private EnemyAnimationController _animationController;
        public void Awake()
        {
            _animator = model.GetComponent<Animator>();
            _animationController = new EnemyAnimationController(_animator);
        }

        private void Start()
        {
            SetStates();
        }

        public void InitializeOnSpawn()
        {
            
        }
        
        private void SetStates()
        {
            _stateMachine = new StateMachine();

            #region States

            

            #endregion

            #region State Changing Conditions

            

            #endregion

            #region Transitions

            

            #endregion
        }
        
        private void Update()
        {
            _stateMachine.Tick();
        }
    }
}