using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{

    const float rayLength = 1.5f;

    [Header("Movement parameters")]
    [Range(0.01f, 20.0f)] [SerializeField] private float moveSpeed = 0.1f; // moving speed of the player
    [Range(0.01f, 20.0f)] [SerializeField] private float jumpForce = 6.0f; // jump force of the player
    [Space(10)]
    public LayerMask groundLayer;

    private Rigidbody2D rigidBody;
    private Animator animator;

    private bool isFacingRight = true;
    private bool isWalking;

    private int score = 0;

    private bool finishedLevel = false;

    private bool IsGrounded()
    {
        return Physics2D.Raycast(this.transform.position, Vector2.down, rayLength, groundLayer.value);
    }

	private void Jump() 
    {
        if (IsGrounded())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
		Vector3 theScale = transform.localScale;
        theScale.x = -theScale.x;
        transform.localScale = theScale;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Bonus"))
		{
            score += 1;
            Debug.Log("Score: " + score);

            other.gameObject.SetActive(false);
		}

        if (other.CompareTag("LevelEnd") && finishedLevel == false)
		{
            finishedLevel = true;
            Debug.Log("Level finished!");
		}
	}

	private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isWalking = false;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) 
        {
            transform.Translate(moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
            isWalking = true;
            if (!isFacingRight)
            {
                Flip();
            }
		}

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.Translate(-moveSpeed * Time.deltaTime, 0.0f, 0.0f, Space.World);
			isWalking = true;
			if (isFacingRight)
			{
				Flip();
			}
		}

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

		// Debug.DrawRay(transform.position, rayLength * Vector3.down, Color.white, 0.1f, false);

		animator.SetBool("isGrounded", IsGrounded());
		animator.SetBool("isWalking", isWalking);
	}
}
