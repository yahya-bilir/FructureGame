using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Trains
{
    public class TrainEngine : Wagon
    {
        [SerializeField] private List<Wagon> wagons = new();
        [SerializeField] private float wagonSpacing = 2f;
        [SerializeField] private Wagon wagonPrefab;
        
        private void Awake()
        {
            tracer = GetComponent<SplineFollower>();
            tracer.follow = true;
            tracer.wrapMode = SplineFollower.Wrap.Loop;
        }

        private void Start()
        {
            ApplyOffsets();
            SetSharedSpeed(tracer.followSpeed);
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
            wagons.Add(wagon);
            wagon.SetFront(this);
            ApplyOffsets();
            wagon.SetSpline(tracer.spline, tracer.direction);
        }

        public void RemoveWagon(Wagon wagon)
        {
            if (wagons.Remove(wagon))
            {
                wagon.gameObject.SetActive(false); // veya Destroy
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
                wagons[i].SetOffsetIndex(i + 1, wagonSpacing); // 1, 2, 3 diye offset index veriliyor
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