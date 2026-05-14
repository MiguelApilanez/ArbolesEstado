using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;

    public int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    private Animator animator;

    private NavMeshAgent agent;

    private MonoBehaviour bossController;

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        bossController = GetComponent<MonoBehaviour>();

        UpdateUI();
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        animator.SetTrigger("Die");

        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (bossController != null)
        {
            bossController.enabled = false;
        }

        Debug.Log("BOSS DEAD");

        Destroy(gameObject, 5f);
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}