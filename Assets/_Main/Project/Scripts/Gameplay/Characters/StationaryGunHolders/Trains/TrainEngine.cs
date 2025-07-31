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
                wagon.UpdatePosition();
            }
        }
        
        public void SetSplineComputer(SplineComputer spline)
        {
            if (tracer == null) tracer = GetComponent<SplineFollower>();
            tracer.spline = spline;
            tracer.RebuildImmediate();
        }

    }
}
