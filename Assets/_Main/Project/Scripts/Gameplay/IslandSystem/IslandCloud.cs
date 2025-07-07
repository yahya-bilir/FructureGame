using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IslandSystem
{
    public class IslandCloud : MonoBehaviour
    {
        [SerializeField] private Transform cloudInnerPosition;
        private Vector3 _cloudOuterPosition;
        private SpriteRenderer _cloudOuterSpriteRenderer;
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
            _cloudOuterSpriteRenderer = GetComponent<SpriteRenderer>();
            _cloudOuterSpriteRenderer.enabled = true;
            //_cloudOuterPosition = transform.position;
            //transform.position = cloudInnerPosition.position;

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
            if(Vector3.Distance(transform.position, cloudInnerPosition.position) <= 5f) return;
            transform.DOMove(cloudInnerPosition.position, 0.6f);
            await UniTask.Delay(600 + 750);
        }        
        
        public async UniTask OpenCloud()
        {
            transform.DOMove(cloudInnerPosition.position, 1f);
            DOVirtual.Color(_cloudOuterSpriteRenderer.color, new Color(1, 1, 1, 0), 0.6f, (value) =>
            {
                _cloudOuterSpriteRenderer.color = value;
            });
            await UniTask.WaitForSeconds(0.75f);
        }
    }
}