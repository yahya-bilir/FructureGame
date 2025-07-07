using AI.Base.Interfaces;
using Characters;
using UnityEngine;

public abstract class BaseAttacking : IState
{
    protected readonly CharacterAnimationController _animationController;
    protected readonly float _attackingInterval;
    private readonly CharacterCombatManager _combatManager;
    private readonly GameObject _model;
    private readonly Transform _modelTransform; // Yeni ekledik
    protected float _attackingTimer;

    protected BaseAttacking(CharacterAnimationController animationController, float attackingInterval,
        CharacterCombatManager combatManager, GameObject model)
    {
        _animationController = animationController;
        _attackingInterval = attackingInterval;
        _combatManager = combatManager;
        _model = model;
        _modelTransform = model.transform; // Transform referansını al
    }

    public virtual void Tick()
    {
        if (_attackingTimer < _attackingInterval)
        {
            _attackingTimer += Time.deltaTime;
            return;
        }

        _attackingTimer = 0f;

        var lastFoundEnemy = _combatManager.LastFoundEnemy;
        if (lastFoundEnemy != null)
        {
            Vector3 dir = lastFoundEnemy.transform.position - _modelTransform.position;

            if (dir.x > 0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (dir.x < -0.1f)
            {
                _modelTransform.localEulerAngles = new Vector3(0, 180, 0);
            }
        }

        _animationController.Attack();
    }

    protected abstract void OnAttack();

    public virtual void OnEnter() { }

    public virtual void OnExit() { }
}