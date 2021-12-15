using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [SerializeField] protected LayerMask hitLayer;
    [SerializeField] protected float damage;
    [SerializeField] protected float cooldown = 1;
    public float currentCooldown;
    
    protected Camera cam;

    private void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    protected virtual void Shoot()
    {
        if (currentCooldown > 0) return;
        currentCooldown = cooldown;
    }
}
