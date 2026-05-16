using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public int phase = 1;

    private BossHealth health;

    private Animator animator;

    private bool enraged = false;

    void Start()
    {
        health = GetComponent<BossHealth>();

        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        // FASE 1

        if (phase == 1)
        {
            int randomAttack = Random.Range(0, 2);

            if (randomAttack == 0)
            {
                Debug.Log("ATTACK 1 TRIGGER");

                animator.Play("Attack1");
            }
            else
            {
                animator.Play("Attack2");
            }
        }

        // FASE 2
        else
        {
            animator.Play("Attack3");
        }
    }

    void Update()
    {
        // ENRAGE
        if (!enraged && health.currentHealth <= health.maxHealth / 2)
        {
            EnterPhase2();
        }
    }
    public void EnterPhase2()
    {
        enraged = true;

        phase = 2;

        animator.SetTrigger("Enrage");

        Debug.Log("BOSS PHASE 2");

        // Buffs
        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 5f;
    }
}
