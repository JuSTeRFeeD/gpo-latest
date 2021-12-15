using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : WeaponBase
{
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    [Space] [SerializeField] private float radius = 5;  
    
    private bool _isShooting = false;
    
    private void Update()
    {
        this._isShooting = Input.GetMouseButton(0);
        this.firePoint.gameObject.SetActive(this._isShooting);
        if (!this._isShooting)
        {
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime / 4;
            return;
        }
        else
        {
            if (currentCooldown > 0) currentCooldown -= Time.deltaTime;
        }
        HandleVisual();
        Shoot();
    }

    protected override void Shoot()
    {
        if (currentCooldown > 0) return;
        var hit = Physics2D.OverlapCircle(endPoint.position, 0.2f, hitLayer);
        if (hit && hit.CompareTag("Damageable"))
        {
            var health = hit.GetComponent<Health>();
            if (health)
            {
                currentCooldown = cooldown;
                health.TakeDamage(damage, transform.parent.parent.gameObject);
            }
        }
    }

    private void HandleVisual()
    {
        var startFirePosition = this.firePoint.position;
        this.laserLine.SetPosition(0, startFirePosition);
        
        var mousePos = (Vector2) cam.ScreenToWorldPoint(Input.mousePosition);
        var endPos = (Vector2) startFirePosition + Vector2.ClampMagnitude((mousePos - (Vector2)startFirePosition), radius);

        Vector2 direction = endPos - (Vector2) startFirePosition;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)startFirePosition, direction.normalized, direction.magnitude, hitLayer);
        if (hit)
        {
            this.endPoint.position = hit.point;
            this.laserLine.SetPosition(1, hit.point);
        }
        else
        {
            this.endPoint.position = endPos;
            this.laserLine.SetPosition(1, endPoint.position);
        }
    }
}
