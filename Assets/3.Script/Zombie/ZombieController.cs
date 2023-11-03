using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // Zombie NavMesh
    private NavMeshAgent nav;
    private Transform targetPos;
    private Transform randomTarget; // �÷��̾� �������� �ʾ��� �� ��ġ
    private Transform player; // �÷��̾��� ��ġ

    // RandomTarget NavMesh
    private float range = 10f;
    private Vector3 point;

    private Animator zombieAnim;
    public bool isAttack = false;
    [SerializeField] private GameObject screamRange;
    [SerializeField] private bool isScreamZombie = false;

    private void Awake()
    {
        TryGetComponent(out nav);
        TryGetComponent(out zombieAnim);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        randomTarget = GameObject.FindGameObjectWithTag("RandomTarget").transform;

        // isScreamZombie = zombieData; zombie ������ ���� schrlghk gownrl
    }

    private void Start()
    {
        StartCoroutine(RandomTargetPos_Co());
    }

    private void FixedUpdate()
    {
        ZombieTransToPlayer();
    }

    private void ZombieTransToPlayer()
    {
        // target�� ���� �׺� ����
        nav.SetDestination(targetPos.position);
    }

    private IEnumerator RandomTargetPos_Co()
    {
        // random target position select
        if (RandomPoint(randomTarget.position, range, out point))
        {
            randomTarget.position = point;
        }
        targetPos = randomTarget;
        yield return new WaitForSeconds(5f);
        StartCoroutine(RandomTargetPos_Co());
    }

    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // �÷��̾� ���� ���� �� ���� Ÿ��
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieAnim.SetBool("isPlayerFind", true);
            if (isScreamZombie && Vector3.Distance(player.position, transform.position) > 1.5f)
            {
                StartCoroutine(ZombieScream_Co());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // Player Tag -> Sound Tag�� �ٲ��� ��
        {
            targetPos = player;
            if (Vector3.Distance(player.position, transform.position) <= 1.5f)
            {
                StartCoroutine(ZombieAttack_Co());
            }
        } else
        {
            if (other.CompareTag("Scream") && !isScreamZombie)
            {
                targetPos = other.gameObject.transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieAnim.SetBool("isPlayerFind", false);
        }
    }

    private IEnumerator ZombieAttack_Co()
    {
        zombieAnim.SetBool("isAttack", true);
        // Damage �־��ֱ�... todo
        yield return new WaitForSeconds(1.5f);
        zombieAnim.SetBool("isAttack", false);
    }

    private IEnumerator ZombieScream_Co()
    {
        // Player Sound Range�� ����, Scream Range�� �ִ� Zombie �ҷ�����
        zombieAnim.SetBool("isScream", true);
        screamRange.SetActive(true);
        yield return null;
        zombieAnim.SetBool("isScream", false);
        yield return new WaitForSeconds(10f);
        screamRange.SetActive(false);
    }
}