using UnityEngine;

namespace Characters
{
    public class CharacterAnimationController
    {
        private readonly Animator _animator;
        private static readonly int RunHash = Animator.StringToHash("Run");
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int JumpHash = Animator.StringToHash("Jump");
        private static readonly int HitHash = Animator.StringToHash("Hit");
        private static readonly int DeadHash = Animator.StringToHash("Dead");
        private static readonly int LeglessHash = Animator.StringToHash("Legless");

        public CharacterAnimationController(Animator animator)
        {
            _animator = animator;
        }
        public void Walk()
        {
            _animator.SetBool(WalkHash, true);
            _animator.SetBool(RunHash, false);
        }

        public void Run()
        {
            _animator.SetBool(WalkHash, false);
            _animator.SetBool(RunHash, true);
        }

        public void Idle()
        {
            _animator.SetBool(AttackHash, false);
            _animator.SetBool(RunHash, false);
            _animator.SetBool(JumpHash, false);
            _animator.SetBool(WalkHash, false);
        }


        public void Attack()
        {
            _animator.SetBool(AttackHash, true);
            _animator.SetBool(RunHash, false);
        }                
        
        public void Legless()
        {
            _animator.SetBool(LeglessHash, true);
        }            
        
        public void Headless()
        {
        }

        public void Armless()
        {
        }
        
        public void Dead()
        {
            _animator.SetTrigger(DeadHash);
        }
        

        public void Spawn()
        {
            Idle();
        }

        public void DisableAnimator() => _animator.enabled = false;
        public void EnableAnimator() => _animator.enabled = true;

        public void GetHit()
        {
            _animator.SetTrigger(HitHash);
        }
    }
}