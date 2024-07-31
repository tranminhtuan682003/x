using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float rotationSpeed = 60f;
    public float arrivalDistance = 5f;
    private NavMeshAgent agent;
    public Transform targetPoint;
    public LayerMask targetLayerMask;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        FollowTargets();
        if (agent.velocity.magnitude > 0)
        {
            Move();
        }
    }

    private void SetDestination(Transform target)
    {
        agent.SetDestination(target.position);
    }

    private void Move()
    {
        Vector3 direction = agent.velocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        ChangePositionTarget(targetPoint);
    }

    private void ChangePositionTarget(Transform target)
    {
        if (Vector3.Distance(transform.position, targetPoint.position) < arrivalDistance)
        {
            ItemManager.instance.SetPositionTarget();
        }
    }

    private void FollowTargets()
    {
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, 10f, targetLayerMask);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in targetsInRange)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        if (closestTarget != null)
        {
            SetDestination(closestTarget);
        }
        else
        {
            SetDestination(targetPoint);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            BaseController player = other.GetComponentInParent<BaseController>();
            player.TakeDamage(50f);
        }
    }

}