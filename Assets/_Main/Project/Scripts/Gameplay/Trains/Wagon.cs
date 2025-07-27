using Characters;
using Dreamteck.Splines;
using UnityEngine;

namespace Trains
{
    public class Wagon : Character
    {
        protected SplineFollower tracer;
        protected Wagon front;

        private int offsetIndex = 1;
        private float spacing = 2f;

        protected virtual void Awake()
        {
            tracer = GetComponent<SplineFollower>();
            tracer.follow = false;
            tracer.wrapMode = SplineFollower.Wrap.Loop;
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
            this.offsetIndex = index;
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

        public void UpdatePosition()
        {
            if (front == null || front.tracer == null || tracer.spline == null)
                return;

            float totalLength = (float)tracer.spline.CalculateLength();
            double frontPercent = front.tracer.result.percent;

            // front'un spline üzerindeki mesafe karşılığı
            float frontDistance = tracer.spline.CalculateLength(0.0, frontPercent);

            float desiredOffset = offsetIndex * spacing;
            float targetDistance = frontDistance - desiredOffset;

            if (targetDistance < 0f && tracer.spline.isClosed)
            {
                targetDistance += totalLength;
            }

            tracer.SetDistance(targetDistance);
        }
    }
}