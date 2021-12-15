using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Leveling : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lvlProgressTxt;
    [SerializeField] private TextMeshProUGUI lvlTxt;
    [SerializeField] private Image progressImg;

    public int Level { get; private set; } = 1;
    public int LevelExp { get; private set; } = 10;
    public int CurrentExp { get; private set; } = 0;

    public void AddExp(int value)
    {
        CurrentExp += value;
        if (CurrentExp > LevelExp)
        {
            Level++;
            CurrentExp = LevelExp - CurrentExp > 0 ? LevelExp - CurrentExp : 0;
            LevelExp = Level + 2;
            lvlTxt.text = "Level: " + Level;
        }
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        lvlProgressTxt.text = CurrentExp + " / " + LevelExp;
        progressImg.fillAmount = (float)CurrentExp / (float)LevelExp;
    }

    private void Awake()
    {
        lvlTxt.text = "Level: " + Level;
        UpdateInfo();
    }
}
