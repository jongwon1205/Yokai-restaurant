using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public StageManager stageManager;

    [Header("Customer Tag")]
    public string customerTag = "Customer";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (stageManager == null) return;
        if (!other.CompareTag(customerTag)) return;

        CustomerController customer = other.GetComponent<CustomerController>();
        if (customer == null) return;

        // hasFood == true면 정상 서빙 받고 나가는 것(성공)
        // hasFood == false면 못 받고 나가는 것(놓침)
        if (customer.HasFood)
        {
            stageManager.AddPass();
        }
        else
        {
            stageManager.AddMiss();
        }
    }
}
