using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour, IPointerClickHandler , IDragHandler ,
    IPointerEnterHandler , IPointerExitHandler ,
    IEndDragHandler , IBeginDragHandler
{
    public InvetoryItemUI cell;


    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cell.inventoryUI.SetDragCell(cell);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!cell.HasItem) return;
        cell.inventoryUI.Tooltip.Show(cell.Item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!cell.HasItem) return;
        cell.inventoryUI.Tooltip.Clear();
    }
}

