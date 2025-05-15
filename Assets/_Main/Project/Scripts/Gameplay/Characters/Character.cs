using DataSave.Runtime;
using UnityEngine;

namespace PropertySystem
{
    public class Character : MonoBehaviour
    {
        [SerializeField] protected GameObject model;
        [SerializeField] private CharacterProperties characterProperties;
        [SerializeField] protected Animator animator;
        protected PropertyManager PropertyManager;
        
        protected virtual void Awake()
        {
            animator = model.GetComponent<Animator>();
            PropertyManager = new PropertyManager(characterProperties);
        }
    }
}