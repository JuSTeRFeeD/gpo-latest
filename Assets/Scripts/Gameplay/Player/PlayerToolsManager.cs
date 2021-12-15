using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToolsManager : MonoBehaviour
{
    [SerializeField] private Transform playerToolsHandlePoint;
    private Camera cam;

    private void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        RotateTool();     
    }

    private void RotateTool()
    {
        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        playerToolsHandlePoint.rotation = Quaternion.LookRotation(transform.forward, playerToolsHandlePoint.position - mousePos) 
                                          * Quaternion.Euler(0, 0, -90);
    }
}
