using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "YokaiDiner/Food Data")]
public class FoodDataSO : ScriptableObject
{
    [Header("메뉴 이름")]
    public string foodName;

    [Header("스프라이트")]
    public Sprite icon;

    [Header("조리 시간(초)")]
    public float cookTime;

    [Header("기본 점수")]
    public int baseScore;
}
