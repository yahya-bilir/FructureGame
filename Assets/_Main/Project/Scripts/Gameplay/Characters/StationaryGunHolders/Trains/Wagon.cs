using System;
using Characters.Enemy;
using Dreamteck.Splines;
using EventBusses;
using Events;
using UnityEngine;
using VContainer;

namespace Trains
{
    public class Wagon : MonoBehaviour
    {
        [Header("Spline Settings")]
        protected SplineFollower Tracer;
        private Wagon _front;

        [Header("Spacing")]
        private int _offsetIndex = 1;
        private float _spacing = 2f;

        [Header("Knockback")]
        [SerializeField] private KnockbackDataHolder knockbackData;

        protected IEventBus EventBus;
        protected IObjectResolver Resolver;

        protected virtual bool IsEngine => false;
        [SerializeField] private StationaryGunHolderCharacter gunHolder;
        
        [Inject]
        private void Inject(IEventBus eventBus, IObjectResolver resolver)
        {
            Debug.Log("Injected");
            EventBus = eventBus;
            Resolver = resolver;
            Resolver.Inject(gunHolder);
        }

        protected virtual void Awake()
        {
            Tracer = GetComponent<SplineFollower>();
            Tracer.wrapMode = SplineFollower.Wrap.Loop;
            Tracer.follow = IsEngine;
        }
        

        protected virtual void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<EnemyBehaviour>();
            if (enemy == null) return;

            //Vector3 impactPoint = other.ClosestPointOnBounds(transform.position);
            Vector3 knockbackDirection = (enemy.transform.position - transform.position).normalized;

            EventBus.Publish(new OnEnemyKnockbacked(enemy, knockbackDirection, knockbackData));
        }

        public void SetFront(Wagon frontWagon)
        {
            _front = frontWagon;
            if (_front != null)
            {
                SetSpline(_front.Tracer.spline, _front.Tracer.direction);
                SetSpeed(_front.Tracer.followSpeed);
            }
        }

        public void SetOffsetIndex(int index, float spacing)
        {
            _offsetIndex = index;
            _spacing = spacing;
        }

        public void SetSpeed(float speed)
        {
            Tracer.followSpeed = speed;
        }

        public void SetSpline(SplineComputer spline, Spline.Direction direction)
        {
            Tracer.spline = spline;
            Tracer.direction = direction;
            Tracer.RebuildImmediate();
        }

        public void UpdatePosition(Spline.Direction direction)
        {
            if (_front == null || _front.Tracer == null || Tracer.spline == null)
                return;

            double frontPercent = _front.Tracer.result.percent;
            float spacingDistance = Mathf.Abs(_offsetIndex * _spacing);
            float moved;

            // İlk Travel denemesi
            double wagonPercent = Tracer.spline.Travel(frontPercent, spacingDistance, out moved, Invert(direction));

            // Eğer tüm mesafeyi kat edemediysek ve spline kapalıysa, sar
            if (moved < spacingDistance && Tracer.spline.isClosed)
            {
                float remaining = spacingDistance - moved;
                double restartPercent = Invert(direction) == Spline.Direction.Forward ? 0.0 : 1.0;

                // Travel kalan mesafeyi baştan (ya da sondan) başlat
                wagonPercent = Tracer.spline.Travel(restartPercent, remaining, out _, Invert(direction));
            }

            Tracer.direction = direction;
            Tracer.SetPercent(Tracer.ClipPercent(wagonPercent));
        }

        
        private Spline.Direction Invert(Spline.Direction dir)
        {
            return dir == Spline.Direction.Forward ? Spline.Direction.Backward : Spline.Direction.Forward;
        }
    }
}
