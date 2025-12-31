using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Seat : MonoBehaviour
{
    [Header("최종 자리 도착 지점")]
    public Transform seatPoint;

    [Header("입장 경유 포인트(순서대로 방문)")]
    [SerializeField] private Transform[] enterWaypoints;

    [Header("퇴장 경유 포인트(순서대로 방문)")]
    [SerializeField] private Transform[] exitWaypoints;

    public bool isOccupied;

    public Transform[] GetEnterWaypoints()
    {
        return enterWaypoints;
    }

    public Transform[] GetExitWaypoints()
    {
        return exitWaypoints;
    }

    public void Occupy()
    {
        isOccupied = true;
    }

    public void Release()
    {
        isOccupied = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 입장 경로: (Seat 위치) -> enterWaypoints... -> seatPoint
        Gizmos.color = Color.yellow;
        Vector3 last = transform.position;

        if (enterWaypoints != null)
        {
            for (int i = 0; i < enterWaypoints.Length; i++)
            {
                if (enterWaypoints[i] == null) continue;
                Gizmos.DrawLine(last, enterWaypoints[i].position);
                Gizmos.DrawSphere(enterWaypoints[i].position, 0.05f);
                last = enterWaypoints[i].position;
            }
        }

        if (seatPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(last, seatPoint.position);
            Gizmos.DrawSphere(seatPoint.position, 0.06f);
        }

        // 퇴장 경로: (seatPoint) -> exitWaypoints...
        if (seatPoint == null) return;

        Gizmos.color = Color.cyan;
        last = seatPoint.position;

        if (exitWaypoints != null)
        {
            for (int i = 0; i < exitWaypoints.Length; i++)
            {
                if (exitWaypoints[i] == null) continue;
                Gizmos.DrawLine(last, exitWaypoints[i].position);
                Gizmos.DrawSphere(exitWaypoints[i].position, 0.05f);
                last = exitWaypoints[i].position;
            }
        }
    }
#endif
}
