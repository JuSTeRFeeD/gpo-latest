using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public LayerMask defaultLayerMask;
    public LayerMask objectsLayerMask;
    public float aggressionRadius = 5f;
    private PlayerMovement _playerMovement;
    private EnemyState _enemyState = EnemyState.Idle;

    [Space]
    [SerializeField] private float moveSpeed = 3f;
    private Rigidbody2D rb;
    private Vector2 destination;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_enemyState == EnemyState.Idle)
        {
            CheckPlayerIsNear();
        }
    }

    private void FixedUpdate()
    {
        if (_enemyState != EnemyState.Follow) return;
        if (Time.frameCount % 2 == 0) destination = UpdateDestination();
        if (destination == (Vector2)transform.position) return;
        var direction = destination - (Vector2)transform.position;
        var angle = Mathf.Atan2 (direction.y, direction.x);
        direction = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle));
        rb.velocity = direction * moveSpeed;
    }

    private Vector2 UpdateDestination()
    {
        var payerPos = _playerMovement.transform.position;
        var myPos = transform.position;
        // var objectHit = Physics2D.Raycast(myPos,  payerPos - myPos, 100f, objectsLayerMask);
        // if (!objectHit || objectHit.collider && objectHit.collider.isTrigger)
        // {
        // return payerPos;
        // }
        var objectHits = Physics2D.RaycastAll(myPos,  payerPos - myPos, 100f, objectsLayerMask);
        var objectFounded = false;
        foreach (var objectHit in objectHits)
        {
            if (objectHit && objectHit.collider && !objectHit.collider.isTrigger)
            {
                objectFounded = true;
                break;
            }
        }
        if (!objectFounded) return payerPos;

        var path = _playerMovement.PathPositions;
        for (var i = 0; i < path.Length; i++)
        {
            var pos = path[i];
            // var hit = Physics2D.Raycast(transform.position, pos - (Vector2)myPos, 100f, objectsLayerMask);
            // if (!hit || hit.collider && hit.collider.isTrigger)
            // {
            // return pos;
            // }
            var hits = Physics2D.RaycastAll(transform.position, pos - (Vector2)myPos, 100f, objectsLayerMask);
            objectFounded = false;
            foreach (var hit in hits)
            {
                if (hit && hit.collider && !hit.collider.isTrigger)
                {
                    objectFounded = true;
                    break;
                }    
            }
            if (!objectFounded) return pos;
        }
        return transform.position;
    }

    private void CheckPlayerIsNear()
    {
        var playerPos = _playerMovement.transform.position;
        var myPos = transform.position;
        if (Vector2.Distance(myPos, playerPos) <= aggressionRadius)
        {
            var hit = Physics2D.Raycast(transform.position, (playerPos - myPos), aggressionRadius * 2, defaultLayerMask);
            if (hit && hit.collider.CompareTag("Player"))
            {
                _enemyState = EnemyState.Follow;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_playerMovement) return;
        var myPos = transform.position;
        if (_enemyState == EnemyState.Idle) Gizmos.DrawWireSphere(myPos, aggressionRadius);
        else
        {
            Gizmos.DrawLine(myPos, destination);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(destination, 0.2f);
        }
    }
}

public enum EnemyState
{
    Idle,
    Follow,
    Attack,
}