using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<InvetoryItemUI> cells = new List<InvetoryItemUI>();

    public void AddNewItem(ResourceData item)
    {
        foreach (var cell in cells)
        {
            if (!cell.HasItem)
            {
                cell.SetItem(item);
                return;
            }
        }
        
        Debug.Log("Inventory is full!");
    }
}
