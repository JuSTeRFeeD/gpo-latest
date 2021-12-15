using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        Clear();
    }

    public void Show(ResourceData item)
    {
        title.text = item.resourceName;
        description.text = item.description;
        image.enabled = true;
    }

    public void Clear()
    {
        image.enabled = false;
        title.text = "";
        description.text = "";
    }
    
}
