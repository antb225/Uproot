using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Mushroom : MonoBehaviour
{
    public float minBounce = 3f;
    public float divisor = 3f;

    CapsuleCollider2D myCollider;


    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if(playerMovement != null) {
            ApplyBounce(playerMovement);
        }
    }

    private void ApplyBounce(PlayerMovement playerMovement) {
        float displacement = minBounce + (playerMovement.fallAmount / divisor);
        float gravity = Mathf.Abs(Physics2D.gravity.y * playerMovement.rb.gravityScale);
        float bounceVelocity = Mathf.Sqrt(2 * gravity * displacement);
        playerMovement.rb.velocity = new Vector2(playerMovement.rb.velocity.x, bounceVelocity);
        AudioManager.Instance.Play("mushroom");
    }

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
