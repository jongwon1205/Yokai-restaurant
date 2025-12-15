using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomerData", menuName = "YokaiDiner/Customer Data")]
public class CustomerDataSO : ScriptableObject
{
    [Header("이름")]
    public string displayName;

    [Header("스프라이트")]
    public Sprite sprite;

    [Header("행동 수치")]
    public float moveSpeed = 2f;       // 움직이는 스피드
    public float basePatience = 20f;   // 대기 시간
    public float eatTime = 8f;         // 기본 식사 시간

    [Header("점수/평판 수치")]
    public int successScore = 10;      // 만족 퇴장 시 점수
    public int failScore = -5;         // 불만족 퇴장 시 점수 or 평점 감소량
}
