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

        CameraFollow cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
        cam?.Shake(0.8f, 0.4f);

        StartCoroutine(FinEnrageCoroutine());
    }

    private System.Collections.IEnumerator FinEnrageCoroutine()
    {
        yield return null;

        float tiempoEsperaEntrada = 0f;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Enrage") && tiempoEsperaEntrada < 2f)
        {
            tiempoEsperaEntrada += Time.deltaTime;
            yield return null;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Enrage"))
        {
            float duracionClip = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duracionClip);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

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
            if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out UnityEngine.AI.NavMeshHit hit, 3f, UnityEngine.AI.NavMesh.AllAreas))
                transform.position = hit.position;

            agent.isStopped = false;
            agent.speed = 5f;
        }
    }

    public bool HasEnraged()
    {
        return enraged;
    }
}
