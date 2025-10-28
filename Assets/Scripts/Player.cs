using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpSpeed = 6.0f;
    [SerializeField] private Rigidbody2D rigidbody;

    private float currentDir;

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
        rigidbody.linearVelocityX = speed * currentDir;
    }

    void OnDirection(InputValue dir)
    {
        currentDir = dir.Get<float>();
    }

    void OnJump()
    {
        if (rigidbody.linearVelocityY == 0.0f)
        {
            rigidbody.linearVelocityY = speed;
        }
    }
}
