using Characters.Enemy;
using Dreamteck.Splines;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Trains
{
    public class Wagon : StationaryGunHolderCharacter
    {
        [Header("Spline Settings")]
        protected SplineFollower tracer;
        protected Wagon front;

        [Header("Spacing")]
        [SerializeField] private int offsetIndex = 1;
        [SerializeField] private float spacing = 2f;

        [Header("Knockback")]
        [SerializeField] private KnockbackDataHolder knockbackData;

        private IEventBus _eventBus;

        protected virtual bool IsEngine => false;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override void Awake()
        {
            base.Awake();

            tracer = GetComponent<SplineFollower>();
            tracer.wrapMode = SplineFollower.Wrap.Loop;
            tracer.follow = IsEngine;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<EnemyBehaviour>();
            if (enemy == null) return;

            Vector3 impactPoint = other.ClosestPointOnBounds(transform.position);
            Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;

            _eventBus.Publish(new OnEnemyKnockbacked(enemy, knockbackDirection, knockbackData));
        }

        public void SetFront(Wagon frontWagon)
        {
            front = frontWagon;
            if (front != null)
            {
                SetSpline(front.tracer.spline, front.tracer.direction);
                SetSpeed(front.tracer.followSpeed);
            }
        }

        public void SetOffsetIndex(int index, float spacing)
        {
            offsetIndex = index;
            this.spacing = spacing;
        }

        public void SetSpeed(float speed)
        {
            tracer.followSpeed = speed;
        }

        public void SetSpline(SplineComputer spline, Spline.Direction direction)
        {
            tracer.spline = spline;
            tracer.direction = direction;
            tracer.RebuildImmediate();
        }

        public void UpdatePosition(Spline.Direction direction)
        {
            if (front == null || front.tracer == null || tracer.spline == null)
                return;

            double frontPercent = front.tracer.result.percent;
            float spacingDistance = Mathf.Abs(offsetIndex * spacing);
            float moved;

            // İlk Travel denemesi
            double wagonPercent = tracer.spline.Travel(frontPercent, spacingDistance, out moved, Invert(direction));

            // Eğer tüm mesafeyi kat edemediysek ve spline kapalıysa, sar
            if (moved < spacingDistance && tracer.spline.isClosed)
            {
                float remaining = spacingDistance - moved;
                double restartPercent = Invert(direction) == Spline.Direction.Forward ? 0.0 : 1.0;

                // Travel kalan mesafeyi baştan (ya da sondan) başlat
                wagonPercent = tracer.spline.Travel(restartPercent, remaining, out _, Invert(direction));
            }

            tracer.direction = direction;
            tracer.SetPercent(tracer.ClipPercent(wagonPercent));
        }

        
        private Spline.Direction Invert(Spline.Direction dir)
        {
            return dir == Spline.Direction.Forward ? Spline.Direction.Backward : Spline.Direction.Forward;
        }
    }
}
