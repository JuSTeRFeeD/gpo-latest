using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Serialization;

public class DaySystem : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color dayColor = new Color(240, 238, 245);
    [SerializeField]private Color nightColor = new Color(70, 70, 70);
    [SerializeField] private float secondsInHour = 5f;
    
    private const float NightHours = 7f;

    public float _curTime = 7f;

    public bool IsNight => _curTime < NightHours;

    private void Awake()
    {
        globalLight.color = dayColor;
    }

    private void Update()
    {
        _curTime += Time.deltaTime / secondsInHour;
        if (_curTime > 24f) _curTime = 0;
        if (_curTime > 22f) globalLight.color = Color.Lerp(globalLight.color, nightColor, Time.deltaTime / secondsInHour);
        else if (_curTime > NightHours) globalLight.color = Color.Lerp(globalLight.color, dayColor, Time.deltaTime / secondsInHour);
    }
}
