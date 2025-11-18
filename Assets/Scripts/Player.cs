using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float jumpSpeed = 6.0f;
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private float halfOnGroundOffset = 0.2f;
    [SerializeField] private float slurAcceleration = 25.0f;
    [SerializeField] private float slurDamp = 1.0f;

    [SerializeField] private LayerMask groundLayer;

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
        if ((GameManager.Instance.GetActiveEffects() & Effects.Slur) == Effects.Slur)
        {
            rigidbody.linearVelocityX = rigidbody.linearVelocityX * slurDamp;
        }

        if (canMove)
        {
            if ((GameManager.Instance.GetActiveEffects() & Effects.Slur) == Effects.Slur)
            {
                if (currentDir != 0.0f)
                {
                    rigidbody.AddForce(slurAcceleration * currentDir * Vector2.right);
                }
            }
            else
            {
                rigidbody.linearVelocityX = speed * currentDir;
            }
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
        Vector2 origin = transform.position + Vector3.down * (halfOnGroundOffset);
        Vector2 size = new Vector2(0.45f, halfOnGroundOffset);

        return Physics2D.OverlapBox(origin, size, 0.0f, groundLayer);
    }
}
