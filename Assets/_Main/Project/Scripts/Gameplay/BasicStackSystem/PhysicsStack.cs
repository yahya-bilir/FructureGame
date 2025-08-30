using DG.Tweening;
using Events;
using UnityEngine;

namespace BasicStackSystem
{
    public class PhysicsStack : BasicStack
    {
        protected override void Awake()
        {
            base.Awake();
            // Yerleşim nesneleri yine oluşturulabilir ama kullanılmayacak
        }

        // Override: yerleştirme yapılmasın
        public override bool TryAddFromOutside(IStackable stackable)
        {
            if (stackable == null || !_buffer.TryPush(stackable)) return false;

            stackable.OnObjectStartedBeingCarried();
            // ❌ _mover.Place çağrısı yok
            stackable.OnObjectCollected();

            _eventBus?.Publish(new OnStackObjectReceived(this, stackable));
            return true;
        }

        // Reflow hiçbir zaman yapılmasın
        protected override void ReflowFrom(int startIndex)
        {
            // ❌ intentionally left empty
        }

        // EjectLastTo: Reflow çağırmasın
        public override IStackable EjectLastTo(Transform targetParent, Vector3 targetLocalPos, bool instant = true)
        {
            var item = _buffer.Pop();
            if (item == null) return null;

            EjectWithoutReflow(item, targetParent, targetLocalPos, instant);
            return item;
        }

        // EjectSpecificTo: Reflow çağırmasın
        public override bool EjectSpecificTo(IStackable stackable, Transform targetParent, Vector3 targetLocalPos, bool instant = true)
        {
            int idx = _buffer.IndexOf(stackable);
            if (idx < 0) return false;

            _buffer.TryRemove(stackable);
            EjectWithoutReflow(stackable, targetParent, targetLocalPos, instant);
            return true;
        }

        private void EjectWithoutReflow(IStackable item, Transform targetParent, Vector3 targetLocalPos, bool instant)
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

            // ❌ ReflowFrom yok!
        }
    }
}
