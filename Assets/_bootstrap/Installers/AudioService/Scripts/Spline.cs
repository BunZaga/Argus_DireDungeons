using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Update;

namespace FunZaga.Audio
{
    public class Spline : MonoBehaviour
    {

        private Vector3 v1ToPos;
        private Vector3 sectorDir;

        private Vector3 prevSegment;
        private Vector3 nextSegment;

        private Vector3 returnValue;

        private Vector3 vec1;
        private Vector3 vec2;

        private Vector3 AtoP;
        private Vector3 AtoB;

        private int splineCount;
        private Vector3[] splinePoint;

        private void Awake()
        {
            splineCount = transform.childCount;
            splinePoint = new Vector3[splineCount];

            for (int i = 0; i < splineCount; i++)
            {
                splinePoint[i] = transform.GetChild(i).position;
            }

            var eventService = Refs.Instance.Get<EventService>();
            // TODO add this to an update tick queue
            // AddFutureQueue
            // Action, Time
            eventService.Subscribe<EventGameUpdate>(OnGameUpdate);
        }

        private void OnGameUpdate()
        {
#if Debug
            if (splineCount > 1)
            {
                for (int i = 0; i < splineCount - 1; i++)
                {
                    Debug.DrawLine(splinePoint[i], splinePoint[i + 1], Color.green);
                }
            }
#endif
        }

        public Vector3 ClosestPosition(Vector3 pos)
        {
            int closestPoint = ClosestPoint(pos);

            if (closestPoint == 0)
            {
                vec1 = splinePoint[closestPoint];
                vec2 = splinePoint[closestPoint + 1];
                ClosestPositionBetweenTwoSplinePoints(vec1, vec2, pos, ref returnValue);
                return returnValue;
            }

            if (closestPoint == splineCount - 1)
            {
                vec1 = splinePoint[closestPoint];
                vec2 = splinePoint[closestPoint - 1];
                ClosestPositionBetweenTwoSplinePoints(vec1, vec2, pos, ref returnValue);
                return returnValue;
            }

            ClosestPositionBetweenTwoSplinePoints(splinePoint[closestPoint], splinePoint[closestPoint - 1], pos, ref prevSegment);
            ClosestPositionBetweenTwoSplinePoints(splinePoint[closestPoint], splinePoint[closestPoint + 1], pos, ref nextSegment);

            if ((prevSegment - pos).sqrMagnitude <= (nextSegment - pos).sqrMagnitude)
            {
                returnValue = prevSegment;
            }
            else
            {
                returnValue = nextSegment;
            }
            return returnValue;
        }

        private int ClosestPoint(Vector3 pos)
        {
            int closestPoint = -1;
            float shortestDistance = Mathf.Infinity;

            // go through each point and find the one closest
            for (int i = 0; i < splineCount; i++)
            {
                float sqrDistance = (splinePoint[i] - pos).sqrMagnitude;
                if (sqrDistance < shortestDistance)
                {
                    shortestDistance = sqrDistance;
                    closestPoint = i;
                }
            }

            return closestPoint;
        }

        private void ClosestPositionBetweenTwoSplinePoints(Vector3 a, Vector3 b, Vector3 pos, ref Vector3 vecRef)
        {
            AtoP = pos - a;
            AtoB = b - a;

            var AtoBSqrMag = AtoB.sqrMagnitude;

            var AtoPDOTAtoB = Vector3.Dot(AtoP, AtoB);

            var distance = AtoPDOTAtoB / AtoBSqrMag;

            // past the first point
            if(distance < 0)
            {
                vecRef = a;
                return;
            }
            // past the second point
            if(distance > 1.0f)
            {
                vecRef = b;
                return;
            }
            // project a onto b by the normalized distance
            vecRef = a + (AtoB * distance);
        }
    }
}