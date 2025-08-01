using System.Collections.Generic;
using Characters.Enemy;
using Dreamteck.Splines;
using EventBusses;
using Events;
using PropertySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Trains
{
    public class TrainEngine : Wagon
    {
        [SerializeField] private RagdollDataHolder ragdollData;
        
        [Header("Wagon Settings")]
        [SerializeField] private List<Wagon> wagons = new();
        [SerializeField] private float wagonSpacing = 2f;
        [SerializeField] private Wagon wagonPrefab;
        private IEventBus _eventBus;
        private Spline.Direction _direction;

        protected override bool IsEngine => true;

        [Inject]
        private void Inject(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        protected override void Start()
        {
            base.Start();
            ApplyOffsets();
            var speed = CharacterPropertyManager.GetProperty(PropertyQuery.Speed).TemporaryValue;
            tracer.followSpeed = speed;
            // for (int i = 0; i < 5; i++)
            // {
            //     SpawnWagon();
            // }
            SetSharedSpeed(speed);
            
        }

        private void LateUpdate()
        {
            tracer.direction = _direction;
            UpdateOffsets();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<EnemyBehaviour>();
            if (enemy == null) return;

            Vector3 impactPoint = other.ClosestPointOnBounds(transform.position);
            _eventBus.Publish(new OnEnemyCrushed(enemy, impactPoint, ragdollData));
        }

        [Button]
        public void SpawnWagon()
        {
            var wagon = Instantiate(wagonPrefab, transform.parent);
            Resolver.Inject(wagon);

            wagons.Add(wagon);
            wagon.SetFront(this);
            ApplyOffsets();
            wagon.SetSpline(tracer.spline, tracer.direction);
        }

        [Button]
        public void RemoveWagon()
        {
            var wagon = wagons[^1];
            if (wagons.Remove(wagon))
            {
                wagon.gameObject.SetActive(false);
                ApplyOffsets();
            }
        }

        private void SetSharedSpeed(float speed)
        {
            tracer.followSpeed = speed;
            foreach (var wagon in wagons)
            {
                wagon.SetSpeed(speed);
            }
        }

        private void ApplyOffsets()
        {
            for (int i = 0; i < wagons.Count; i++)
            {
                wagons[i].SetOffsetIndex(i + 1, wagonSpacing);
            }
        }

        private void UpdateOffsets()
        {
            foreach (var wagon in wagons)
            {
                wagon.UpdatePosition(_direction);
            }
        }
        
        public void SetSplineComputer(SplineComputer spline, bool isReversed)
        {
            if (tracer == null) tracer = GetComponent<SplineFollower>();

            //tracer.enabled = false; // BÜTÜN OTOMATİK AYARLARI ENGELLER

            tracer.spline = spline;
            _direction = isReversed ? Spline.Direction.Backward : Spline.Direction.Forward;
            tracer.direction = _direction;
            tracer.RebuildImmediate();
            tracer.SetPercent(isReversed ? 1.0 : 0.0);
            //tracer.follow = true;

            wagonSpacing = Mathf.Abs(wagonSpacing) * (isReversed ? -1f : 1f);

            //tracer.enabled = true; // SONRA AÇ
        }

    }
}
