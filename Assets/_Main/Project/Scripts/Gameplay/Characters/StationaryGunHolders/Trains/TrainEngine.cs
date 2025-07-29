using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Trains
{
    public class TrainEngine : Wagon
    {
        [SerializeField] private List<Wagon> wagons = new();
        [SerializeField] private float wagonSpacing = 2f;
        [SerializeField] private Wagon wagonPrefab;

        protected override bool IsEngine => true;

        protected override void Start()
        {
            base.Start();
            ApplyOffsets();
            SetSharedSpeed(tracer.followSpeed);

            // Örnek: başlangıçta 5 vagon spawn et
            for (int i = 0; i < 5; i++)
            {
                SpawnWagon();
            }
        }

        private void LateUpdate()
        {
            UpdateOffsets();
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

        public void RemoveWagon(Wagon wagon)
        {
            if (wagons.Remove(wagon))
            {
                wagon.gameObject.SetActive(false);
                ApplyOffsets();
            }
        }

        public void SetSharedSpeed(float speed)
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
    }
}
