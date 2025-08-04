using System.Collections.Generic;
using Characters.Enemy;
using Dreamteck.Splines;
using EventBusses;
using Events;
using PropertySystem;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Trains
{
    public class TrainEngine : Wagon
    {
        [Header("Wagon Settings")]
        [SerializeField] private List<Wagon> wagons = new();
        [SerializeField] private float wagonSpacing = 2f;
        [SerializeField] private Wagon wagonPrefab;

        [Header("Combat Settings")]
        [SerializeField] private RagdollDataHolder ragdollData;

        [Header("Character Properties")]
        [SerializeField] private CharacterPropertiesSO characterPropertiesSO;

        private IObjectResolver _resolver;
        private CharacterPropertyManager _characterPropertyManager;
        private Spline.Direction _direction;

        [Inject]
        private void Inject(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        protected void Awake()
        {
            _characterPropertyManager = new CharacterPropertyManager(characterPropertiesSO);
            var speed = _characterPropertyManager.GetProperty(PropertyQuery.Speed).TemporaryValue;
            Tracer.followSpeed = speed;
        }

        protected void Start()
        {
            ApplyOffsets();
            SetSharedSpeed(Tracer.followSpeed);
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
            _resolver.InjectGameObject(wagon.gameObject);

            wagons.Add(wagon);
            wagon.SetFront(this);
            wagon.SetSpline(Tracer.spline, Tracer.direction);
            ApplyOffsets();
        }

        [Button]
        public void RemoveWagon()
        {
            if (wagons.Count == 0) return;

            var wagon = wagons[^1];
            if (wagons.Remove(wagon))
            {
                wagon.gameObject.SetActive(false);
                ApplyOffsets();
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

        private void SetSharedSpeed(float speed)
        {
            Tracer.followSpeed = speed;
            foreach (var wagon in wagons)
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

        public void SetSplineComputer(SplineComputer spline, bool isReversed)
        {
            Tracer.spline = spline;
            _direction = isReversed ? Spline.Direction.Backward : Spline.Direction.Forward;
            Tracer.direction = _direction;

            Tracer.RebuildImmediate();
            Tracer.SetPercent(isReversed ? 1.0 : 0.0);

            wagonSpacing = Mathf.Abs(wagonSpacing) * (isReversed ? -1f : 1f);
        }
    }
}
