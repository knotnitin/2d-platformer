using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platformer : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    public float speed;
    public float jumpForce;
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float rememberGroundedFor;
    float lastTimeGrounded;

    public float dashForce, startDashTimer;
    float CurrentDashTimer;
    float DashDirection;

    bool isDashing = false;
    bool obtainedDash = false;
    bool canDash = true;

    int additionalJumps;
    public int defaultAdditionalJumps = 1;
    bool obtainedDJump = false;

    public GameObject m_Player;
    public string m_Scene;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();
        if (Input.GetKeyDown(KeyCode.UpArrow) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float moveBy = x * speed;
        rb.velocity = new Vector2(moveBy, rb.velocity.y);

        if(Input.GetKeyDown(KeyCode.C) && !isGrounded && x != 0 && obtainedDash && canDash)
        {
            canDash = false;
            isDashing = true;
            CurrentDashTimer = startDashTimer;
            rb.velocity = Vector2.zero;
            DashDirection = (int)x;
        }

        if(isDashing)
        {
            rb.velocity = transform.right * DashDirection * dashForce;

            CurrentDashTimer -= Time.deltaTime;
            if(CurrentDashTimer <= 0)
            {
                isDashing = false;
            }
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || (additionalJumps > 0 && obtainedDJump)))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps--;
        }
    }

    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
        if (colliders != null)
        {
            isGrounded = true;
            canDash = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "spike")
        {
            Debug.Log("Dead!");
            ResetPosition();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if(other.tag == "nextlevel")
        {
            if(SceneManager.GetActiveScene().buildIndex == 9)
            {
                Debug.Log("End of game!");
                Destroy(this);
                SceneManager.LoadScene(10);
            }
            else
            {
                Debug.Log("Next Level!");
                ResetPosition();
                DontDestroyOnLoad(this);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                SceneManager.MoveGameObjectToScene(m_Player, SceneManager.GetSceneByName(m_Scene));
            }
        }
        else if (other.tag == "dash")
        {
            Debug.Log("Obtained Dash!");
            obtainedDash = true;
        }
        else if (other.tag == "dJump")
        {
            Debug.Log("Obtained Double Jump!");
            obtainedDJump = true;
        }
    }

    void ResetPosition()
    {
        transform.localPosition = new Vector3(-9, (float)-3.5);
    }
}
