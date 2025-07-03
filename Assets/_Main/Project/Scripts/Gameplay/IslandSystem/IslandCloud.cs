using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IslandSystem
{
    public class IslandCloud : MonoBehaviour
    {
        [SerializeField] private Transform cloudInnerPosition;
        private Vector3 _cloudOuterPosition;
        // private IEventBus _eventBus;
        // public int İ { get; private set; }
        //
        // [Inject]
        // private void Inject(IEventBus eventBus)
        // {
        //     _eventBus = eventBus;
        // }
        //
        private void Awake()
        {
            _cloudOuterPosition = transform.position;
            transform.position = cloudInnerPosition.position;

        }
        
        //
        // private void OnEnable()
        // {
        //     _eventBus.Subscribe<OnIslandSelected>(OnIslandSelected);   
        // }
        //
        // private void OnDisable()
        // {
        //     _eventBus.Unsubscribe<OnIslandSelected>(OnIslandSelected);
        // }
        //
        // private void OnIslandSelected(OnIslandSelected eventData)
        // {
        //     PerformCloudActions().Forget();
        // }

        public async UniTask PerformCloudActions()
        {
            await CloseCloud();
           // await UniTask.Delay();
            await OpenCloud();
        }

        private async UniTask CloseCloud()
        {
            if(transform.position == cloudInnerPosition.position) return;
            transform.DOMove(cloudInnerPosition.position, 0.6f);
            await UniTask.Delay(600 + 750);
        }        
        
        private async UniTask OpenCloud()
        {
            transform.DOMove(_cloudOuterPosition, 0.6f);
            //await UniTask.Delay(200);
        }
    }
}