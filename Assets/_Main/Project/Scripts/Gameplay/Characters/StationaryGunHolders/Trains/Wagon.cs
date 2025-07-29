using Dreamteck.Splines;
using UnityEngine;

namespace Trains
{
    public class Wagon : StationaryGunHolderCharacter
    {
        protected SplineFollower tracer;
        protected Wagon front;

        private int offsetIndex = 1;
        private float spacing = 2f;

        // Engine olup olmadığını alt sınıf override edebilir
        protected virtual bool IsEngine => false;

        protected override void Awake()
        {
            base.Awake();

            tracer = GetComponent<SplineFollower>();
            tracer.wrapMode = SplineFollower.Wrap.Loop;
            tracer.follow = IsEngine;
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

        public void UpdatePosition()
        {
            if (front == null || front.tracer == null || tracer.spline == null)
                return;

            float totalLength = (float)tracer.spline.CalculateLength();
            double frontPercent = front.tracer.result.percent;
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
