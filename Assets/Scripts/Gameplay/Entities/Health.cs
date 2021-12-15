using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [Header("Drop")] [SerializeField] private List<Resource> resources = new List<Resource>();
    private DropsController _dropsController;
    [Space]
    [Header("Properties")]
    [SerializeField] private bool scaleOnDamage = true;
    [SerializeField] private string hitEffectName = null;
    [SerializeField] private bool godMode = false;
    [SerializeField] private float healthAmount = 1;
    private float _currentHealth;

    private ParticlesController _particlesController;
    
    public void SetHitEffectName(string newHitEffectName) => this.hitEffectName = newHitEffectName;
    public void SetGodMode(bool isGodMode) => this.godMode = isGodMode;
    
    private void Start()
    {
        _currentHealth = healthAmount;
        if (this.hitEffectName != null)
        {
            _particlesController = GameObject.FindGameObjectWithTag("ParticlesController").GetComponent<ParticlesController>();
        }

        if (this.resources.Count != 0)
        {
            _dropsController = GameObject.FindGameObjectWithTag("DropsController").GetComponent<DropsController>();
        }
    }
    
    public void TakeDamage(float damage, GameObject from = null)
    {
        if (this.hitEffectName != null)
        {
            if (_particlesController != null) _particlesController.PlayParticle(hitEffectName, (Vector2) transform.position);
            if (scaleOnDamage && this._currentHealth - damage > 0)
            {
                if (TakeDamageScaler.Instance) TakeDamageScaler.Instance.PlayEffect(transform);
            }
        }
        if (this.godMode) return;
        
        this._currentHealth -= damage;
        if (this._currentHealth <= 0)
        {
            Death(from);
        }
    }
    
    private void Death(GameObject from = null)
    {
        if (from != null)
        {
            if (from.CompareTag("Player"))
            {
                from.GetComponent<Leveling>().AddExp(5);
            }
        }
        // TODO: для игрока можно сделать потерую случайных 50% предметов
        
        foreach (var res in resources)
        {
            _dropsController.DropItem(res.data, transform.position);
        }
        Destroy(gameObject);
    }
}
