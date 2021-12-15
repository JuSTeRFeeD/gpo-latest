using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryTooltip _tooltip;
    public InventoryTooltip Tooltip => _tooltip; 
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private List<InvetoryItemUI> cells = new List<InvetoryItemUI>();

    private InvetoryItemUI dragCell;
    
    private void Awake()
    {
        int id = 0;
        foreach (var cell in cells)
        {
            cell.inventoryUI = this;
            cell.id = id++;
        }
    }

    public void SetDragCell(InvetoryItemUI cell)
    {
        dragCell = cell;
    }

    public void HandleDropToCell(InvetoryItemUI cell)
    {
        if (!dragCell || dragCell.id == cell.id) return;
        if (cell.HasItem)
        {
            // cell.ClearItem();
            // dragCell.ClearItem();
            // TODO: generate random item   
            return;
        }
        cell.SetItem(dragCell.Item);
        dragCell.ClearItem();
    }
    
    public bool HasEmptySlots()
    {
        return cells.Any(cell => !cell.HasItem);
    }

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
