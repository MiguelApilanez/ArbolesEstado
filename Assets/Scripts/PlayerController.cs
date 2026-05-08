using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Salto")]
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position,groundDistance,groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0f, z).normalized;

        animator.SetFloat("Speed", move.magnitude);

        if (move.magnitude >= 0.1f)
        {
            Vector3 movement = move * speed * Time.deltaTime;
            transform.position += movement;

            Quaternion targetRotation = Quaternion.LookRotation(move);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            animator.SetTrigger("Jump");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}
