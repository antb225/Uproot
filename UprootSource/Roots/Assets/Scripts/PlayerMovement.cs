using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed = 5f;
    public float jumpForce = 20f;

    public float fallAmount;

    public Rigidbody2D rb;
    float moveInputX;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask GroundLayer;

    Animator animator;

    private void Awake() {
        transform.position = GameData.Instance.lastCheckPointPos;
        
    }

    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        AudioManager.Instance.Play("music");
    }

    // Update is called once per frame
    void Update() {
        transform.position += new Vector3Int(0, 3, 1);
        moveInputX = Input.GetAxis("Horizontal");
        transform.position -= new Vector3Int(0, 3, 1);
        Jump();

        if (Input.GetAxisRaw("Horizontal") != 0) {
            animator.SetBool("Moving", true);
        } else animator.SetBool("Moving", false);

        if (rb.velocity.y < 0) {
            fallAmount += Mathf.Abs(rb.velocity.y) * Time.deltaTime;
        }
        else fallAmount = 0;
        
    }
    private void FixedUpdate() {
        Flip();
        rb.velocity = new Vector2(moveInputX * speed, rb.velocity.y);
    }

    public void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {

            if (IsGrounded()) {
                AudioManager.Instance.Play("jump");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }
    }
    public bool IsGrounded() {
        return Physics2D.OverlapCircle(GroundCheck.position, GroundCheck.GetComponent<CircleCollider2D>().radius, GroundLayer);
    }

    private void Flip() {
        if (moveInputX < 0) {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (moveInputX > 0) {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }
}
