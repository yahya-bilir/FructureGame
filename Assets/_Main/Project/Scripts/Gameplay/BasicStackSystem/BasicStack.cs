using DG.Tweening;
using UnityEngine;
using VContainer;
using EventBusses;
using Events;

namespace BasicStackSystem
{
    public class BasicStack : MonoBehaviour
    {
        [field: SerializeField] public StackAreaSO StackArea { get; private set; }
        [field: SerializeField] public Transform StackParent { get; private set; }
        [field: SerializeField] public int Capacity { get; private set; } = 6;
        [field: SerializeField] public MoveStyle PlacementStyle { get; private set; } = MoveStyle.Instant;
        [field: SerializeField] public float TweenDuration { get; private set; } = 0.2f;

        private StackBuffer _buffer;
        private IStackLayout _layout;
        private IStackMover _mover;

        private IEventBus _eventBus; // <-- EventBus referansı

        public bool IsThereAnySpace => !_buffer.IsFull;
        public bool IsThereAnyObject => !_buffer.IsEmpty;
        public int Count => _buffer.Count;

        [Inject] // Projedeki örneklerle aynı injection paterni (method injection)  :contentReference[oaicite:3]{index=3}
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        private void Awake()
        {
            if (StackParent == null) StackParent = transform;

            _buffer = new StackBuffer(Capacity);
            _layout = new GridStackLayout(StackArea);
            _mover = PlacementStyle == MoveStyle.Instant
                ? new InstantMover()
                : new TweenMover(PlacementStyle, StackArea != null ? StackArea.JumpSpeedInSeconds : TweenDuration);
        }

        public void SetCapacity(int newCapacity)
        {
            Capacity = Mathf.Max(0, newCapacity);
            _buffer.SetCapacity(Capacity);
        }

        // Dışarıdan obje ekleme
        public bool TryAddFromOutside(IStackable stackable)
        {
            if (stackable == null || !_buffer.TryPush(stackable)) return false;

            stackable.OnObjectStartedBeingCarried();
            var tr = stackable.GameObject.transform;
            var targetPos = _layout.GetLocalPosition(_buffer.Count - 1);
            _mover.Place(tr, StackParent, targetPos);
            stackable.OnObjectCollected();

            // -> OLAY: Stack obje aldı (OnStackObjectReceived)
            _eventBus?.Publish(new OnStackObjectReceived(this, stackable)); // Publish<T> kullanımı senin EventBus ile aynı  :contentReference[oaicite:4]{index=4}

            return true;
        }

        // Son objeyi dışarı çıkar (hedef parent/pozisyona)
        public IStackable EjectLastTo(Transform targetParent, Vector3 targetLocalPos, bool instant = true)
        {
            var s = _buffer.Pop();
            if (s == null) return null;

            var tr = s.GameObject.transform;
            tr.DOKill();
            if (instant)
            {
                if (tr.parent != targetParent) tr.SetParent(targetParent, false);
                tr.localPosition = targetLocalPos;
            }
            else
            {
                var mover = new TweenMover(MoveStyle.Move, StackArea != null ? StackArea.JumpSpeedInSeconds : TweenDuration);
                mover.Place(tr, targetParent, targetLocalPos);
            }

            s.OnObjectDropped();

            // -> OLAY: Stack objeyi dışarı verdi (OnStackObjectEjected)
            _eventBus?.Publish(new OnStackObjectEjected(this, s)); // Publish tetiklemesi  :contentReference[oaicite:5]{index=5}

            ReflowFrom(0);
            return s;
        }

        public bool EjectSpecificTo(IStackable stackable, Transform targetParent, Vector3 targetLocalPos, bool instant = true)
        {
            int idx = _buffer.IndexOf(stackable);
            if (idx < 0) return false;

            _buffer.TryRemove(stackable);
            var tr = stackable.GameObject.transform;
            tr.DOKill();

            if (instant)
            {
                if (tr.parent != targetParent) tr.SetParent(targetParent, false);
                tr.localPosition = targetLocalPos;
            }
            else
            {
                var mover = new TweenMover(MoveStyle.Move, StackArea != null ? StackArea.JumpSpeedInSeconds : TweenDuration);
                mover.Place(tr, targetParent, targetLocalPos);
            }

            stackable.OnObjectDropped();

            _eventBus?.Publish(new OnStackObjectEjected(this, stackable)); //  :contentReference[oaicite:6]{index=6}

            ReflowFrom(idx);
            return true;
        }

        public void ReflowFrom(int startIndex)
        {
            for (int i = Mathf.Max(0, startIndex); i < _buffer.Count; i++)
            {
                var tr = _buffer[i].GameObject.transform;
                var targetPos = _layout.GetLocalPosition(i);
                _mover.Place(tr, StackParent, targetPos);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (StackArea == null) return;
            var layout = new GridStackLayout(StackArea);
            var parent = StackParent == null ? transform : StackParent;
            Gizmos.color = new Color(1f, 1f, 0f, 0.75f);
            int preview = Mathf.Max(1, Capacity);
            for (int i = 0; i < preview; i++)
            {
                var local = layout.GetLocalPosition(i);
                Gizmos.DrawSphere(parent.TransformPoint(local), 0.075f);
            }
        }
#endif
    }
}
