using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private ResourceData _resource;

    private void Start()
    {
        this._spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetParams(ResourceData resource)
    {
        this._resource = resource;
        if (!this._spriteRenderer) this._spriteRenderer = GetComponent<SpriteRenderer>();
        this._spriteRenderer.sprite = resource.sprite;
    }

    public ResourceData Pickup()
    {
        gameObject.SetActive(false);
        return _resource;
    }
}
