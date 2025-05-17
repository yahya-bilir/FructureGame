using UnityEngine;

namespace Characters
{
    public class CharacterAnimationController
    {
        private readonly Animator _animator;
        private static readonly int RunHash = Animator.StringToHash("Run");
        private static readonly int WalkHash = Animator.StringToHash("Walk");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

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
            _animator.SetBool(WalkHash, false);
            _animator.SetBool(RunHash, false);
        }


        public void Attack()
        {
            _animator.SetBool(AttackHash, true);
            _animator.SetBool(RunHash, false);
        }

        public void DisableAnimator() => _animator.enabled = false;
        public void EnableAnimator() => _animator.enabled = true;
    }
}