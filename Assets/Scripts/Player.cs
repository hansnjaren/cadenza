using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpSpeed = 6.0f;
    [SerializeField] private new Rigidbody2D rigidbody;

    private float currentDir;
    public bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rigidbody.linearVelocityX = speed * currentDir;
        }
        else
        {
            rigidbody.linearVelocityX = 0;
        }
    }

    void OnDirection(InputValue dir)
    {
        currentDir = dir.Get<float>();
    }

    void OnJump()
    {
        if (!canMove) return;
        if (OnGround())
        {
            rigidbody.linearVelocityY = jumpSpeed;
        }
    }

    private bool OnGround()
    {
        return Physics2D.Raycast(transform.position, Vector3.down, 0.2f);
    }
}
