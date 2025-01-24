using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Character Attributes")]
    public Animator animator;
    public Joystick joystick;
    [SerializeField]
    GameObject playerIndicator;

    public int maxHealth = 100;
    public int currentHealth = 0;
    int swordDamage = 40;

    public float movementSpeed = 6f;
    
    public float dashCooldown = 0.1f;
    public float dashSpeed = 2f;
    public float dashDuraiton = 0.1f;
    
    [SerializeField]
    float attackRate = 4f;
    public float nextAttackTime = 0;
    public float attackDashSpeed = 2f;
    public float attackDashDuraiton = 0.1f;

    [Space]
    [Header("Character statistics")]
    public Vector2 moveDirection;

    public int stamina = 0;
    public int stamina_max = 7;
    float stamina_counter = 0;
    public float stamina_rate = 1;

    public bool shootBlunderbuss = false;
    public bool endOfAiming = false;

    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Space]
    [Header("References")]
    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    public Transform attackPoint;
    private float timeBtwDash;
    private float attackDashTime;
    private float dashTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        stamina = stamina_max;
        attackDashTime = 0;
        dashTime = 0;

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        TimersDecrease();
        Move();
        Animate();
    }

    // FixedUpdate is called 60 times per second
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        StaminaLoop();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Debug.Log("Iron player health is currently: " + currentHealth);

        // Play hurt sound & animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Debug.Log("The player Iron died!");

        // die animation

        // Disable enemy
        playerIndicator.SetActive(false);
        GetComponentInChildren<Collider2D>().enabled = false;

        this.enabled = false;
    }


    /**************************************************************************
     * Timers functions:
     *      Timers Decrease: decreases all Cooldown Timers over time.
     *      All others are delayed invoked functions.
     **************************************************************************/

    void TimersDecrease()
    {
        if (stamina < 0)
        {
            dashTime = -dashDuraiton;
            attackDashTime = -attackDashDuraiton;
        }

        if (timeBtwDash > 0)
            timeBtwDash -= Time.deltaTime;
    }

    /**************************************************************************
     * Can be called delayed by using:
     *      Invoke("SetSwingFalse", 0.1f);
     **************************************************************************      
    void SetSwingFalse()
    {
        swingR2L = false;
        swingL2R = false;
    }
     **************************************************************************/

    /**************************************************************************
     * Movement functions
     **************************************************************************/

    void Move()
    {
        moveDirection = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (attackDashTime > 0)
        {
            AttackDash();
        }
        else if (dashTime > 0)
        {
            Dash();
        }
        else
        {
            moveVelocity = moveDirection.normalized * movementSpeed;
        }
    }

    void Dash()
    {
        if (moveDirection == Vector2.zero)
        {
            moveDirection = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));
        }

        dashTime -= Time.deltaTime;
        moveVelocity = moveDirection.normalized * dashSpeed;
    }

    void AttackDash()
    {
        if (moveDirection == Vector2.zero)
        {
            moveDirection = new Vector2(animator.GetFloat("Horizontal"), animator.GetFloat("Vertical"));
        }

        attackDashTime -= Time.deltaTime * 1000;
        moveVelocity = moveDirection.normalized * attackDashSpeed;
    }


    /****************************************************************************
     * Udates the stamina when called
     * /!\ Must be called at FixedUpdate() to update every second
     ****************************************************************************/

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


    /**************************************************************************
     * Udates all the animator values when called
     * Debugging:
     *      var clipInfo = animator.GetCurrentAnimatorClipInfo(0);  //
     *      Debug.Log(clipInfo[0].clip.name);                       // Displays the name of the current animation playing in console
     **************************************************************************/

    void Animate()
    {
        if (moveDirection != Vector2.zero)
        {
            animator.SetFloat("Horizontal", moveDirection.x);
            animator.SetFloat("Vertical", moveDirection.y);
            animator.SetFloat("Magnitude", moveDirection.magnitude);
        }

        animator.SetFloat("Speed", moveVelocity.sqrMagnitude);
    }

    
    /*****************************************************************************
     * Functions called by buttons
     *****************************************************************************/

    public void ButtonDash()
    {
        if (stamina > 0 && timeBtwDash <= 0)
        {
            dashTime = dashDuraiton;
            moveVelocity = Vector2.zero;
            stamina -= 1;
            timeBtwDash = dashCooldown;
        }
    }

    public void ButtonSword()
    {
        if (stamina > 0)
        {
            if (Time.time >= nextAttackTime) // (timeBtwAttack <= 0) // 
            {
                // Stamina & Attack Cooldown
                stamina -= 1;
                nextAttackTime = Time.time + 1 / attackRate; // Attack cooldown timer

                // Attack animation
                if (Random.value > 0.5f)
                {  // Selects a random swing direction (L2R or R2L)
                    animator.SetTrigger("R2Lswing");
                } else {
                    animator.SetTrigger("L2Rswing");
                }

                // Attack dash movement
                attackDashTime = attackDashDuraiton;
                moveVelocity = Vector2.zero;

                // Enemy detection & Damage
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

                foreach(Collider2D enemy in hitEnemies)
                {
                    Debug.Log("We hit " + enemy.name);
                    if (enemy.GetComponentInParent<IronEnemy>() != null)
                        enemy.GetComponentInParent<IronEnemy>().TakeDamage(swordDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void Blunderbuss()
    {
        //shootBlunderbuss = Input.GetButtonDown("Fire2");
    }


}
