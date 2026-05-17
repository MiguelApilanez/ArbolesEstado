using UnityEngine;

public class BossCombat : MonoBehaviour
{
    public int phase = 1;

    private Animator animator;
    private bool enraged = false;
    private bool isEnraging = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack()
    {
        if (isEnraging) return;

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
        else
        {
            animator.SetTrigger("Attack3");
            Debug.Log("ATTACK 3");
        }
    }

    public void EnterPhase2()
    {
        if (enraged) return;

        enraged = true;
        phase = 2;
        isEnraging = true;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.isStopped = true;

        animator.SetTrigger("Enrage");
        Debug.Log("BOSS PHASE 2");

        StartCoroutine(FinEnrageCoroutine());
    }

    private System.Collections.IEnumerator FinEnrageCoroutine()
    {
        yield return null;
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).IsName("Enrage"));
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
