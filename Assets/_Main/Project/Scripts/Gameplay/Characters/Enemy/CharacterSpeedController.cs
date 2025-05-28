using System;
using Cysharp.Threading.Tasks;
using PropertySystem;
using System.Threading;
using EventBusses;
using Events;
using VContainer;

namespace Characters.Enemy
{
    public class CharacterSpeedController
    {
        private readonly CharacterPropertyManager _characterPropertyManager;
        private readonly CharacterDataHolder _dataHolder;
        private readonly Character _connectedCharacter;

        private bool _isInAttackedState;
        private CancellationTokenSource _attackStateCts;

        public CharacterSpeedController(CharacterPropertyManager characterPropertyManager, CharacterDataHolder dataHolder, Character connectedCharacter)
        {
            _characterPropertyManager = characterPropertyManager;
            _dataHolder = dataHolder;
            _connectedCharacter = connectedCharacter;
        }
        
        [Inject]
        private void Inject(IEventBus eventBus)
        {
            eventBus.Subscribe<OnEnemyBeingAttacked>(OnEnemyBeingAttacked);
        }

        private void OnEnemyBeingAttacked(OnEnemyBeingAttacked eventData)
        {
            if (eventData.AttackedEnemy != _connectedCharacter) return;
            OnEnemyBeingAttackedAsync().Forget();
        }

        private async UniTaskVoid OnEnemyBeingAttackedAsync()
        {
            if (_isInAttackedState)
                return;

            _isInAttackedState = true;
            _attackStateCts = new CancellationTokenSource();

            var speed = _characterPropertyManager.GetProperty(PropertyQuery.Speed);
            var originalSpeed = speed.TemporaryValue;

            var newSpeedValue = originalSpeed / _dataHolder.OnAttackedSpeedDivider;
            _characterPropertyManager.SetProperty(PropertyQuery.Speed, newSpeedValue);

            try
            {
                await UniTask.WaitForSeconds(
                    _dataHolder.OnAttackedSpeedRecoverTime,
                    cancellationToken: _attackStateCts.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            _characterPropertyManager.SetProperty(PropertyQuery.Speed, originalSpeed);
            _isInAttackedState = false;
        }

        public void CancelAttackState()
        {
            _attackStateCts?.Cancel();
            _isInAttackedState = false;
        }

        public void OnEnemyFleeing()
        {
            // Kaçma sırasında farklı bir hız uygulanacaksa burada yönetebilirsin.
        }
    }
}
