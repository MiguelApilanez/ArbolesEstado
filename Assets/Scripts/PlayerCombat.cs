using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ataque")]
    public float attackRange = 2f;
    public int attackDamage = 20;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Cooldown")]
    public float attackCooldown = 1f;

    private float nextAttackTime;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            BossHealth bossHealth = enemy.GetComponent<BossHealth>();

            if (bossHealth != null)
            {
                bossHealth.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
