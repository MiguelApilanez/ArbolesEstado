using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public int phase = 1;

    private BossHealth health;

    private Animator animator;

    private bool enraged = false;
    private bool isEnraging = false;

    void Start()
    {
        health = GetComponent<BossHealth>();

        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        // NO atacar durante Enrage
        if (isEnraging)
            return;

        // FASE 1
        if (phase == 1)
        {
            int randomAttack = Random.Range(0, 2);

            if (randomAttack == 0)
            {
                animator.SetTrigger("Attack1");

                Debug.Log("ATTACK 1");
            }
            else
            {
                animator.SetTrigger("Attack2");

                Debug.Log("ATTACK 2");
            }
        }

        // FASE 2
        else
        {
            animator.SetTrigger("Attack3");

            Debug.Log("ATTACK 3");
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
        Debug.Log("PHASE = " + phase);
        enraged = true;

        phase = 2;

        isEnraging = true;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = true;
        }

        animator.SetTrigger("Enrage");

        Debug.Log("BOSS PHASE 2");

        StartCoroutine(FinEnrageCoroutine());
    }

    private System.Collections.IEnumerator FinEnrageCoroutine()
    {
        // Espera un frame para que el Animator procese el trigger
        yield return null;
        // Espera a que entre en el estado Enrage
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Enrage"));
        // Espera a que termine la animación de Enrage
        yield return new WaitUntil(() =>
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Enrage"));
        EndEnrage();
    }
    public bool IsEnraging()
    {
        return isEnraging;
    }

    public void EndEnrage()
    {
        isEnraging = false;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (agent != null)
        {
            agent.isStopped = false;

            agent.speed = 5f;
        }
    }
    public bool HasEnraged()
    {
        return enraged;
    }
}
