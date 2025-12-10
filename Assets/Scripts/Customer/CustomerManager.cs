using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class StageConfig
{
    [Header("스폰 간격 (초)")]
    public float spawnInterval = 3f;

    [Header("동시에 존재할 수 있는 최대 손님 수")]
    public int maxCustomers = 3;

    [Header("인내심 배율 (손님 SO의 basePatience * 이 값)")]
    public float patienceMultiplier = 1f;

    [Header("식사 시간 배율 (손님 SO의 eatTime * 이 값)")]
    public float eatTimeMultiplier = 1f;
}

[System.Serializable]
public class Seat
{
    public Transform seatPoint;               // 의자/테이블 위치
    [HideInInspector] public bool isOccupied; // 현재 사용 중인지
}

public class CustomerManager : MonoBehaviour
{
    [Header("스테이지 설정들 (예: 3개)")]
    public StageConfig[] stageConfigs;

    [Header("현재 스테이지 (0=1스테이지, 1=2스테이지, 2=3스테이지)")]
    public int currentStageIndex = 0;

    [Header("손님 프리팹 (CustomerController 포함)")]
    public GameObject customerPrefab;

    [Header("스폰/퇴장 위치")]
    public Transform spawnPoint;
    public Transform exitPoint;

    [Header("좌석 리스트")]
    public Seat[] seats;

    [Header("손님 타입 리스트 (SO들)")]
    public CustomerDataSO[] customerTypes;

    private StageConfig currentStage;
    private List<CustomerController> liveCustomers = new List<CustomerController>();
    private Coroutine spawnRoutine;
    private bool isStageRunning = false;

    private void Start()
    {
        SetStage(0);
        StartStage();
    }

    public void SetStage(int index)
    {
        if (stageConfigs == null || stageConfigs.Length == 0)
        {
            Debug.LogWarning("StageConfigs가 비어있습니다.");
            return;
        }

        currentStageIndex = Mathf.Clamp(index, 0, stageConfigs.Length - 1);
        currentStage = stageConfigs[currentStageIndex];

        Debug.Log($"스테이지 변경: {currentStageIndex + 1} 스테이지");
    }

    public void StartStage()
    {
        if (isStageRunning) return;
        isStageRunning = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopStage()
    {
        isStageRunning = false;
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (isStageRunning)
        {
            if (liveCustomers.Count < currentStage.maxCustomers)
            {
                Seat freeSeat = FindFreeSeat();
                if (freeSeat != null)
                {
                    SpawnCustomer(freeSeat);
                }
            }

            yield return new WaitForSeconds(currentStage.spawnInterval);
        }
    }

    private Seat FindFreeSeat()
    {
        for (int i = 0; i < seats.Length; i++)
        {
            if (!seats[i].isOccupied && seats[i].seatPoint != null)
            {
                return seats[i];
            }
        }

        return null;
    }

    private void SpawnCustomer(Seat seat)
    {
        if (customerPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("CustomerPrefab 또는 SpawnPoint가 설정되지 않았습니다.");
            return;
        }

        if (customerTypes == null || customerTypes.Length == 0)
        {
            Debug.LogWarning("CustomerTypes(SO)가 설정되지 않았습니다.");
            return;
        }

        // 손님 타입 랜덤 선택 (원하면 VIP 가중치 등으로 바꿀 수 있음)
        CustomerDataSO randomData = customerTypes[Random.Range(0, customerTypes.Length)];

        GameObject obj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        CustomerController customer = obj.GetComponent<CustomerController>();

        if (customer == null)
        {
            Debug.LogWarning("CustomerPrefab에 CustomerController가 없습니다.");
            Destroy(obj);
            return;
        }

        seat.isOccupied = true;

        // 스테이지 배율 적용
        float patienceTime = randomData.basePatience * currentStage.patienceMultiplier;
        float eatTime = randomData.eatTime * currentStage.eatTimeMultiplier;

        customer.Init(
            randomData,
            this,
            seat,
            patienceTime,
            eatTime,
            exitPoint
        );

        liveCustomers.Add(customer);
    }

    public void OnCustomerExit(CustomerController customer, Seat seat, bool isHappy, CustomerDataSO data)
    {
        if (seat != null)
        {
            seat.isOccupied = false;
        }

        if (liveCustomers.Contains(customer))
        {
            liveCustomers.Remove(customer);
        }

        // 여기서 점수/평판 처리
        if (isHappy)
        {
            Debug.Log($"[손님 퇴장] 만족 ({data.displayName}) / 점수 +{data.successScore}");
            // TODO: GameScoreManager.Instance.AddScore(data.successScore);
        }
        else
        {
            Debug.Log($"[손님 퇴장] 불만족 ({data.displayName}) / 점수 {data.failScore}");
            // TODO: GameScoreManager.Instance.AddScore(data.failScore);
        }
    }

    // 예시: 주방에서 호출
    public void ServeFoodToCustomer(CustomerController customer)
    {
        if (customer != null)
        {
            customer.OnFoodServed();
        }
    }

    public void GoToNextStage()
    {
        StopStage();
        int nextIndex = Mathf.Clamp(currentStageIndex + 1, 0, stageConfigs.Length - 1);
        SetStage(nextIndex);
        StartStage();
    }
}
