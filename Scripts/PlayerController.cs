using UnityEngine.SceneManagement;
using UnityEngine;
using Fusion;

public class PlayerController : BaseController
{
    [SerializeField] private float moveSpeed = 6f;
    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Start()
    {
        maxHealth = 200f;
        base.Start();
        CameraFollow();
    }

    public override void FixedUpdateNetwork()
    {
        Move();
    }

    private void CameraFollow()
    {
        if (Object.HasInputAuthority)
        {
            Camera.main.GetComponent<CameraController>().SetTarget(transform);
        }
    }

    protected override void Move()
    {
        if (GetInput(out NetworkInputData data))
        {
            Vector3 moveDirection = data.direction;

            if (moveDirection != Vector3.zero)
            {
                transform.position += moveDirection * moveSpeed * Runner.DeltaTime;
                transform.forward = Vector3.Slerp(transform.forward, moveDirection, Runner.DeltaTime * moveSpeed * 2);

                animator.SetBool("canRun", true);
            }
            else
            {
                animator.SetBool("canRun", false);
            }
        }
    }


    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Dead()
    {
        if (Object != null && Runner != null)
        {
            Runner.Despawn(Object);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("Idling", true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            animator.SetBool("Idling", false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ruby"))
        {
            other.gameObject.SetActive(false);
            ItemManager.instance.Score();
        }
    }
}