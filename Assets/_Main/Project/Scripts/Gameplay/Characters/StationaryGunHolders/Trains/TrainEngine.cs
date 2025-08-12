using System;
using System.Collections.Generic;
using Characters.Enemy;
using Dreamteck.Splines;
using Events;
using PropertySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Trains
{
    public class TrainEngine : Wagon
    {
        [Header("Wagon Settings")]
        [field: SerializeField] public List<Wagon> Wagons { get; private set; }= new();
        [SerializeField] private float wagonSpacing = 2f;
        [SerializeField] private Wagon wagonPrefab;

        [Header("Combat Settings")]
        [SerializeField] private RagdollDataHolder ragdollData;

        [Header("Character Properties")]
        [field: SerializeField] public CharacterPropertiesSO CharacterPropertiesSO { get; private set; }

        public CharacterPropertyManager CharacterPropertyManager { get; private set; }
        private Spline.Direction _direction;
        protected override bool IsEngine => true;

    
        protected override void Awake()
        {
            base.Awake();
            CharacterPropertyManager = new CharacterPropertyManager(CharacterPropertiesSO);
        }

        private void OnPropertyUpgraded(OnPropertyUpgraded obj)
        {
            if(obj.CharacterPropertyManager != CharacterPropertyManager) return;
            SetSharedSpeed(CharacterPropertyManager.GetProperty(PropertyQuery.Speed).TemporaryValue);
        }


        protected void Start()
        {
            EventBus.Subscribe<OnPropertyUpgraded>(OnPropertyUpgraded);
            
            ApplyOffsets();
            SetSharedSpeed(Tracer.followSpeed);
            Resolver.Inject(CharacterPropertyManager);
            var speed = CharacterPropertyManager.GetProperty(PropertyQuery.Speed).TemporaryValue;
            Tracer.followSpeed = speed;
        }

        private void LateUpdate()
        {
            Tracer.direction = _direction;
            UpdateOffsets();
        }

        [Button]
        public void SpawnWagon()
        {
            var wagon = Instantiate(wagonPrefab, transform.parent);
            Resolver.Inject(wagon);

            Wagons.Add(wagon);
            wagon.SetFront(this);
            wagon.SetSpline(Tracer.spline, Tracer.direction);
            ApplyOffsets();
        }

        [Button]
        public void RemoveWagon()
        {
            if (Wagons.Count == 0) return;

            var wagon = Wagons[^1];
            if (Wagons.Remove(wagon))
            {
                wagon.gameObject.SetActive(false);
                ApplyOffsets();
            }
        }

        private void ApplyOffsets()
        {
            for (int i = 0; i < Wagons.Count; i++)
            {
                Wagons[i].SetOffsetIndex(i + 1, wagonSpacing);
            }
        }

        private void UpdateOffsets()
        {
            foreach (var wagon in Wagons)
            {
                wagon.UpdatePosition(_direction);
            }
        }

        private void SetSharedSpeed(float speed)
        {
            Tracer.followSpeed = speed;
            foreach (var wagon in Wagons)
            {
                wagon.SetSpeed(speed);
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<EnemyBehaviour>();
            if (enemy == null) return;

            Vector3 impactPoint = other.ClosestPointOnBounds(transform.position);
            EventBus.Publish(new OnEnemyCrushed(enemy, impactPoint, ragdollData));
        }

        public void SetSplineComputer(SplineComputer spline, bool isReversed, double? startPercent = null)
        {
            Tracer.spline = spline;
            _direction = isReversed ? Spline.Direction.Backward : Spline.Direction.Forward;
            Tracer.direction = _direction;
            Tracer.RebuildImmediate();

            if (startPercent.HasValue)
                Tracer.SetPercent(startPercent.Value);
            else
                Tracer.SetPercent(isReversed ? 1.0 : 0.0);

            wagonSpacing = Mathf.Abs(wagonSpacing) * (isReversed ? -1f : 1f);
        }
        
        private void OnDisable()
        {
            EventBus.Subscribe<OnPropertyUpgraded>(OnPropertyUpgraded);
        }

    }
}
