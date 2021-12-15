using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemiesSpawner : MonoBehaviour
{
    private bool _stopSpawning = true;

    public void SetSpawning(bool value)
    {
        _stopSpawning = value;
    }
    
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnCooldown = 20f;
    private float _cooldown = 0;
    [SerializeField] private Transform player;
    private readonly Random _random = new Random();

    private void Awake()
    {
        _cooldown = spawnCooldown;
    }

    private void SpawnEnemy()
    {
        Instantiate(
            enemyPrefab, 
            player.position + new Vector3(_random.Next(-10, 10), _random.Next(-10, 10), 0),
            Quaternion.identity);
    }

    private void Update()
    {
        if (_stopSpawning) return;
        if (_cooldown <= 0)
        {
            _cooldown = spawnCooldown;
            SpawnEnemy();
        }
        _cooldown -= Time.deltaTime;
    }
}
