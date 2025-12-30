using UnityEngine;
using UnityEngine.AI;

public class CustomerNavFix : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) return;

        // 회전 자동 갱신 끄기
        agent.updateRotation = false;

        agent.updateUpAxis = false;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity; 
    }
}
