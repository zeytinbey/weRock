using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Walkthrough.ClimbingLadders
{
    public class MyLadder : MonoBehaviour
    {
        [Header("Ladder Segment Settings")]

        [Tooltip("Ladder segment's local-space bottom offset from object origin.")]
        [SerializeField] private Vector3 ladderSegmentBottom = Vector3.zero;

        [Tooltip("Length of the ladder segment.")]
        [SerializeField] private float ladderSegmentLength = 2f;

        [Tooltip("Direction of the ladder segment in local space (will be normalized).")]
        [SerializeField] private Vector3 ladderDirection = Vector3.up;

        [Header("Release Points")]
        public Transform BottomReleasePoint;
        public Transform TopReleasePoint;

        // Exposed properties
        public Vector3 LadderSegmentBottom => ladderSegmentBottom;
        public float LadderSegmentLength => ladderSegmentLength;
        public Vector3 LadderDirection => ladderDirection.normalized;

        /// <summary>
        /// World-space tırmanma yönü (merdivenin yönü)
        /// </summary>
        public Vector3 WorldClimbDirection => transform.TransformDirection(LadderDirection);

        /// <summary>
        /// Ladder segmentinin dünya koordinatındaki alt noktası
        /// </summary>
        public Vector3 BottomAnchorPoint
        {
            get
            {
                return transform.position + transform.TransformVector(ladderSegmentBottom);
            }
        }

        /// <summary>
        /// Ladder segmentinin dünya koordinatındaki üst noktası
        /// </summary>
        public Vector3 TopAnchorPoint
        {
            get
            {
                return BottomAnchorPoint + WorldClimbDirection * ladderSegmentLength;
            }
        }

        /// <summary>
        /// Dış noktadan ladder segmentine en yakın noktayı ve segment üzerindeki pozisyonu verir
        /// </summary>
        public Vector3 ClosestPointOnLadderSegment(Vector3 fromPoint, out float onSegmentState)
        {
            Vector3 segment = TopAnchorPoint - BottomAnchorPoint;
            Vector3 segmentPoint1ToPoint = fromPoint - BottomAnchorPoint;
            float pointProjectionLength = Vector3.Dot(segmentPoint1ToPoint, segment.normalized);

            if (pointProjectionLength > 0)
            {
                if (pointProjectionLength <= segment.magnitude)
                {
                    onSegmentState = 0;
                    return BottomAnchorPoint + (segment.normalized * pointProjectionLength);
                }
                else
                {
                    onSegmentState = pointProjectionLength - segment.magnitude;
                    return TopAnchorPoint;
                }
            }
            else
            {
                onSegmentState = pointProjectionLength;
                return BottomAnchorPoint;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(BottomAnchorPoint, TopAnchorPoint);
        }
    }
}
