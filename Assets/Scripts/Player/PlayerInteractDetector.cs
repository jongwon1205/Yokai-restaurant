using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractDetector : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] private float detectRadius = 0.8f;
    [SerializeField] private LayerMask interactableMask;

    [Header("Refs")]
    [SerializeField] private PlayerCarry carry;
    [SerializeField] private EPromptUI promptUi;

    private IInteractable currentTarget;

    private void Awake()
    {
        if (carry == null)
            carry = GetComponent<PlayerCarry>();
    }

    private void Update()
    {
        UpdateTarget();
        UpdateUIAndHighlight();
    }

    private void UpdateTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, interactableMask);

        IInteractable nearest = null;
        float bestDistSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            IInteractable interactable = hits[i].GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distSqr = ((Vector2)hits[i].transform.position - (Vector2)transform.position).sqrMagnitude;
            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                nearest = interactable;
            }
        }

        if (nearest == currentTarget) return;

        // 이전 타겟 OFF
        if (currentTarget != null)
            currentTarget.SetHighlighted(false);

        currentTarget = nearest;

        // 새 타겟 ON
        if (currentTarget != null)
            currentTarget.SetHighlighted(true);
    }

    private void UpdateUIAndHighlight()
    {
        if (promptUi == null) return;

        if (currentTarget == null)
        {
            promptUi.Hide();
            return;
        }

        Transform anchor = currentTarget.PromptAnchor != null ? currentTarget.PromptAnchor : ((MonoBehaviour)currentTarget).transform;
        promptUi.Show(anchor);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
