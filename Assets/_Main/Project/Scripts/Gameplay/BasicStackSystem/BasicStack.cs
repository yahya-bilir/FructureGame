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

        protected StackBuffer _buffer;
        protected IStackLayout _layout;
        protected IStackMover _mover;
        protected IEventBus _eventBus;

        public bool IsThereAnySpace => !_buffer.IsFull;
        public bool IsThereAnyObject => !_buffer.IsEmpty;
        public int Count => _buffer.Count;

        [Inject]
        private void Inject(IEventBus eventBus) => _eventBus = eventBus;

        protected virtual void Awake()
        {
            if (StackParent == null) StackParent = transform;

            _buffer = new StackBuffer(Capacity);
            _layout = new GridStackLayout(StackArea);
            _mover = PlacementStyle == MoveStyle.Instant
                ? new InstantMover()
                : new TweenMover(PlacementStyle, StackArea.TweenDuration);
        }

        public void SetCapacity(int newCapacity)
        {
            Capacity = Mathf.Max(0, newCapacity);
            _buffer.SetCapacity(Capacity);
        }

        public virtual bool TryAddFromOutside(IStackable stackable)
        {
            if (stackable == null || !_buffer.TryPush(stackable)) return false;

            stackable.OnObjectStartedBeingCarried();
            var tr = stackable.GameObject.transform;
            var targetPos = _layout.GetLocalPosition(_buffer.Count - 1);
            _mover.Place(tr, StackParent, targetPos);
            stackable.OnObjectCollected();

            _eventBus?.Publish(new OnStackObjectReceived(this, stackable));
            return true;
        }

        public virtual IStackable EjectLastTo(Transform targetParent, Vector3 targetLocalPos, bool instant = true)
        {
            var item = _buffer.Pop();
            if (item == null) return null;

            Eject(item, reflowStartIndex: 0, targetParent, targetLocalPos, instant);
            return item;
        }

        public virtual bool EjectSpecificTo(IStackable stackable, Transform targetParent, Vector3 targetLocalPos,
            bool instant = true)
        {
            int idx = _buffer.IndexOf(stackable);
            if (idx < 0) return false;

            _buffer.TryRemove(stackable);
            Eject(stackable, reflowStartIndex: idx, targetParent, targetLocalPos, instant);
            return true;
        }

        protected virtual void ReflowFrom(int startIndex)
        {
            for (int i = Mathf.Max(0, startIndex); i < _buffer.Count; i++)
            {
                var tr = _buffer[i].GameObject.transform;
                var targetPos = _layout.GetLocalPosition(i);
                PlaceInstant(tr, StackParent, targetPos);
            }
        }

        private void PlaceInstant(Transform tr, Transform parent, Vector3 localPos)
        {
            tr.DOKill();
            if (tr.parent != parent) tr.SetParent(parent, false);
            tr.localPosition = localPos;
        }

        protected virtual void Eject(IStackable item, int reflowStartIndex, Transform targetParent,
            Vector3 targetLocalPos, bool instant)
        {
            var tr = item.GameObject.transform;
            tr.DOKill();

            if (instant)
            {
                if (tr.parent != targetParent) tr.SetParent(targetParent, false);
                tr.localPosition = targetLocalPos;
            }
            else
            {
                var mover = new TweenMover(MoveStyle.Move, StackArea.TweenDuration);
                mover.Place(tr, targetParent, targetLocalPos);
            }

            item.OnObjectDropped();
            _eventBus?.Publish(new OnStackObjectEjected(this, item));

            // ✨ Türevin iptal edebileceği nokta
            ReflowFrom(reflowStartIndex);
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