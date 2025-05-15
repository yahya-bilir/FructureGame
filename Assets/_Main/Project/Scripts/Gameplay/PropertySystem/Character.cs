using DataSave.Runtime;
using UnityEngine;

namespace PropertySystem
{
    public class Character : MonoBehaviour
    {
        [SerializeField] protected GameObject model;
        [SerializeField] protected CharacterProperties characterProperties;
        [SerializeField] protected Animator animator;
        
        protected virtual void Awake()
        {
            animator = model.GetComponent<Animator>();
        }
    }
}