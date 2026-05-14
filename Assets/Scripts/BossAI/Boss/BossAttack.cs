using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public int damage = 20;

    public float attackRange = 2.5f;

    public Transform attackPoint;

    public LayerMask playerLayer;

    public void DealDamage()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayer);

        foreach (Collider player in hitPlayers)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
