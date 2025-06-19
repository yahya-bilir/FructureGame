using AI.Base.Interfaces;
using Characters;
using UnityEngine;

public abstract class BaseAttacking : IState
{
    protected readonly CharacterAnimationController _animationController;
    protected readonly float _attackingInterval;
    protected float _attackingTimer;

    protected BaseAttacking(CharacterAnimationController animationController, float attackingInterval)
    {
        _animationController = animationController;
        _attackingInterval = attackingInterval;
    }

    public virtual void Tick()
    {
        if (_attackingTimer < _attackingInterval)
        {
            _attackingTimer += Time.deltaTime;
            return;
        }

        _attackingTimer = 0f;
        _animationController.Attack();

        OnAttack(); // 🔁 Alt sınıfa özgü davranış
    }

    protected abstract void OnAttack();

    public virtual void OnEnter() { }

    public virtual void OnExit() { }
}