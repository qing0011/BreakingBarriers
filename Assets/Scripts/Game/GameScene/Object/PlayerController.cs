using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;

    private Vector3 moveInput;
    private Vector3 currentVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // UI传入移动方向
    public void SetMoveInput(Vector3 input)
    {
        moveInput = input;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 targetVelocity = moveInput * maxSpeed;

        if (moveInput.magnitude > 0.01f)
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                Vector3.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.velocity = new Vector3(
            currentVelocity.x,
            rb.velocity.y,
            currentVelocity.z
        );

        if (currentVelocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity);

            rb.rotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}