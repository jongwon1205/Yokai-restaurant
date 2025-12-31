using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public CustomerController[] customerPrefabs;
    public Seat[] seats;

    public Transform spawnPoint;
    public Transform exitPoint;

    public List<FoodDataSO> menuFoods = new List<FoodDataSO>();

    public float spawnInterval = 3f;
    public int maxCustomers = 5;

    public float patienceTime = 12f;
    public float eatTime = 6f;

    private List<CustomerController> aliveCustomers = new List<CustomerController>();
    private bool isRunning;

    private void Start()
    {
        isRunning = true;
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (aliveCustomers.Count >= maxCustomers)
                continue;

            Seat seat = FindFreeSeat();
            if (seat == null)
                continue;

            SpawnCustomer(seat);
        }
    }

    private void SpawnCustomer(Seat seat)
    {
        if (customerPrefabs == null || customerPrefabs.Length == 0) return;
        if (spawnPoint == null || exitPoint == null) return;

        CustomerController prefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        if (prefab == null) return;

        CustomerController customer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        CustomerDataSO data = null;
        CustomerDataHolder holder = customer.GetComponent<CustomerDataHolder>();
        if (holder != null)
            data = holder.data;

        customer.Init(
            data,
            this,
            seat,
            patienceTime,
            eatTime,
            exitPoint
        );

        aliveCustomers.Add(customer);
    }

    private Seat FindFreeSeat()
    {
        if (seats == null) return null;

        for (int i = 0; i < seats.Length; i++)
        {
            if (seats[i] == null) continue;
            if (!seats[i].isOccupied && seats[i].seatPoint != null)
                return seats[i];
        }

        return null;
    }

    public FoodDataSO GetRandomFoodForCustomer(CustomerDataSO customerData)
    {
        if (menuFoods == null || menuFoods.Count == 0)
            return null;

        return menuFoods[Random.Range(0, menuFoods.Count)];
    }

    public void OnCustomerLeft(CustomerController customer, bool isHappy)
    {
        if (customer != null)
            aliveCustomers.Remove(customer);

        StageProgressManager spm = FindObjectOfType<StageProgressManager>();
        if (spm != null)
        {
            spm.AddClearedCustomer(1);
        }
    }

    public void StopSpawning()
    {
        isRunning = false;
    }
}