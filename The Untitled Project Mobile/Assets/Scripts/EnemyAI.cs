using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class EnemyAI : MonoBehaviour
{
    [Space]
    [Header("Character Attributes")]
    [Tooltip("Enemy speed")]
    public float speed = 5f;
    public float nextWayPointDIstance = 1f;
    public float attackRange = 1f;

    [Space]
    [Header("Character statistics")]
    public bool swing = false;

    [Space]
    [Header("Pathfinding statistics")]
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Vector2 direction;
    Vector2 moveVelocity;

    [Header("References")]
    public Transform target;
    public Animator animator;
    Seeker seeker;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        // Udates the paths every 500 miliseconds
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    // FixedUpdate is called 60 times every second
    void FixedUpdate()
    {
        if (!swing)
        {
            FixedFollowPlayer();
        }
        else
            moveVelocity = Vector2.zero;
    }

    // Update is called once every frame
    private void Update()
    {
        swing = (Vector2.Distance(rb.position, target.position) < attackRange) ? true : false;

        Animate();
    }

    // Updates animator parameters when called
    void Animate()
    {
        if (direction != Vector2.zero)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            animator.SetFloat("Magnitude", direction.magnitude);
        }

        animator.SetBool("isFollowing", (moveVelocity.sqrMagnitude > 1) ? true : false);
        animator.SetBool("isSwing", swing);
    }


    /***********************************************************************************
     *                          --- Pathfinding methods ---
     * 
     ***********************************************************************************/

    // Pathfinding update method called once every 0.5 seconds
    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    // Pathfinding error check
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    // Movement direction from pathfinding algorithm
    void FixedFollowPlayer()
    {
        if (path == null)
        {
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        moveVelocity = direction * speed;

        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime); // Movement

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDIstance)
        {
            currentWayPoint++;
        }
    }
}
