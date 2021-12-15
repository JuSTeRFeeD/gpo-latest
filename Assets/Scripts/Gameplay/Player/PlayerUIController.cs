using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject inventory; 
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) inventory.SetActive(!inventory.activeSelf);
    }
}
