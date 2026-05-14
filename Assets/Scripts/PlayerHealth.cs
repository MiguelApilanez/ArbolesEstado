using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;

    private int currentHealth;

    [Header("UI")]
    public Slider healthSlider;

    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

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

        Debug.Log("PLAYER DEAD");

        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerCombat>().enabled = false;
    }

    void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
