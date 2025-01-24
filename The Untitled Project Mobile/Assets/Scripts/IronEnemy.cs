using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class IronEnemy : MonoBehaviour
{
    [Header("Character Attributes")]
    public Animator animator;
    public AIPath aiPath;   // Pathfinding algorithm

    public int maxHealth = 100;
    public int currentHealth;
    int swordDamage = 20;

    [SerializeField]
    float attackRate = 4f;
    public float nextAttackTime = 0;
    public float attackDashSpeed = 10f;     // Speed of the dash when the enemy attack
    public float attackDashDuraiton = 0.1f; // Attack dash duration
    public float attackRange = 1f;

    [Space]
    public float staminaRefillCooldown = 2f;
    public float staminaRefillTime = 0f;
    public int stamina_max = 5;
    public float stamina_rate = 1;

    [Space]
    [Header("Character statistics")]
    public Vector2 moveDirection;

    public int stamina = 0;
    float stamina_counter = 0;

    [SerializeField]
    float enemyAttackRange = 0.5f;
    [SerializeField]
    LayerMask enemyEnemyLayers;

    [Space]
    [Header("References")]
    Vector2 moveVelocity;
    public Transform enemyAttackPoint;
    float timeBtwDash;
    float attackDashTime;

    Rigidbody2D rb;

    // Start is called when the code run
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }

    // FixedUdate is called 60 times per second
    private void FixedUpdate()
    {
        StaminaLoop();
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        Sword();
        Animate();
        TimersDecrease();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Debug.Log("Iron enemy health is currently: " + currentHealth);
        // Play hurt sound & animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Debug.Log("Enemy died!");

        // die animation

        // Disable enemy
        aiPath.canMove = false;

        GetComponentInChildren<Collider2D>().enabled = false;
        this.enabled = false;
    }

    // Decrease all the timers /!\ (every frame)
    void TimersDecrease()
    {
        if (staminaRefillTime > 0)  // stamina cooldown timer
        {
            staminaRefillTime -= Time.deltaTime;
        }

        if (attackDashTime > 0)  // attack dash timer
        {
            attackDashTime -= Time.deltaTime;
        }
    }

    // Update all the necessary values of the animator
    void Animate()
    {
        if (moveDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", moveDirection.x);
            animator.SetFloat("Vertical", moveDirection.y);
            animator.SetFloat("Magnitude", moveVelocity.magnitude);
        }

        animator.SetFloat("Speed", moveVelocity.magnitude);
    }

    void StaminaLoop()
    {
        if (stamina < stamina_max)
        {
            stamina_counter += 1;
        }

        if (stamina_counter * stamina_rate == 60)
        {
            stamina_counter = 0;
            stamina += 1;
        }
    }

    // Set the movement vector and movement type
    void Move()
    {
        moveVelocity = aiPath.desiredVelocity;
        moveDirection = moveVelocity.normalized;

        if (attackDashTime > 0)
        {
            aiPath.canMove = false;
            AttackDash();
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }
        else
        {
            aiPath.canMove = true;
        }
    }

    void Sword()
    {
        if (Vector2.Distance(rb.position, aiPath.destination) < attackRange)
        {
            if (staminaRefillTime <= 0)
            {
                if (Time.time >= nextAttackTime)
                {
                    if (stamina > 0)
                    {
                        // Attack animation
                        if (Random.value > 0.5f)
                        {  // Selects a random swing direction (L2R or R2L)
                            animator.SetTrigger("R2Lswing");
                        } else {
                            animator.SetTrigger("L2Rswing");
                        }

                        stamina -= 1;

                        attackDashTime = attackDashDuraiton;
                        nextAttackTime = Time.time + 1 / attackRate; // Attack cooldown timer

                        // Enemy detection & Damage
                        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(enemyAttackPoint.position, enemyAttackRange, enemyEnemyLayers);

                        foreach (Collider2D enemy in hitEnemies)
                        {
                            Debug.Log("We hit " + enemy.name);

                            if (enemy.GetComponentInParent<Player>() != null)
                                enemy.GetComponentInParent<Player>().TakeDamage(swordDamage);
                        }
                    }
                    else
                    {
                        staminaRefillTime = staminaRefillCooldown * stamina_max / stamina_rate;
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (enemyAttackPoint == null)
            return;

        Gizmos.DrawWireSphere(enemyAttackPoint.position, enemyAttackRange);
    }

    void AttackDash()
    {
        if (moveDirection == Vector2.zero)
        {
            moveDirection = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));
        }
        
        moveVelocity = moveDirection.normalized * attackDashSpeed;
    }


}
