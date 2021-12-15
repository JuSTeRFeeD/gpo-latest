using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvetoryItemUI : MonoBehaviour
{
    private ResourceData _item;
    public ResourceData Item => _item;
    public bool HasItem => _item != null;

    [SerializeField] private Image itemImage;
    
    private void Start()
    {
        var uiItem = itemImage.GetComponent<UIItem>();
        uiItem.cell = this;
    }
    public void SetItem(ResourceData item)
    {
        this._item = item;
        itemImage.enabled = true;
        itemImage.sprite = item.sprite;
    }
}
