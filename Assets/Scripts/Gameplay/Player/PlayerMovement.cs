using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float horizontal = 0;
    private float vertical = 0;
    private float prevX = 0;
    private float prevY = 0;

    [SerializeField] float runSpeed = 6.0f;
    private static readonly int animIsWalk = Animator.StringToHash("isWalk");
    private static readonly int animSpeedX = Animator.StringToHash("speedX");
    private static readonly int animSpeedY = Animator.StringToHash("speedY");

    [Space] [Header("Pathfinding")] 
    public const float DistBetweenPathPoints = 0.15f;
    private readonly Vector2[] _pathPositions = new Vector2[100];
    public Vector2[] PathPositions => _pathPositions;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();

        for (var i = 0; i < _pathPositions.Length; i++)
        {
            _pathPositions[i] = transform.position;
        }
    }

    void Update ()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (prevX == horizontal && prevY == vertical) return;
        prevX = horizontal;
        prevY = vertical;
        
        // Animations:
        // animator.SetBool(animIsWalk, horizontal != 0 || vertical != 0);
        // if (horizontal > 0) spriteRenderer.flipX = true;
        // else if (horizontal < 0) spriteRenderer.flipX = false;
        // animator.SetInteger(animSpeedX, horizontal != 0 ? 1 : 0);
        //
        // if (vertical > 0) animator.SetInteger(animSpeedY, 1);
        // else if (vertical < 0) animator.SetInteger(animSpeedY, -1);
        // else animator.SetInteger(animSpeedY, 0);
    }

    private void FixedUpdate()
    {
        var isMoving = horizontal != 0 || vertical != 0;
        if (isMoving) PoopPathfinding();
        
        
        var move = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        if (move.x != 0 && move.y != 0) move *= 0.7f;
        rb.velocity = move;
    }

    private void PoopPathfinding()
    {
        Vector2 playerPos = transform.position; 
        if (Vector2.Distance(_pathPositions[0], playerPos) > DistBetweenPathPoints)
        {
            for (var j = _pathPositions.Length - 1; j >= 1; j--)
                _pathPositions[j] = _pathPositions[j - 1];
            _pathPositions[0] = playerPos;
        }
    }
    private void OnDrawGizmos()
    {
        foreach (var pos in _pathPositions)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(pos, 0.08f);
        }
    }
}
