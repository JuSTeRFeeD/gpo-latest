using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("DropItem")) return;
        var dropItem = other.GetComponent<DropItem>();
        if (!dropItem) return;
        if (!inventoryUI.HasEmptySlots()) return;
        var resData = dropItem.Pickup();
        inventoryUI.AddNewItem(resData);
    }
}
