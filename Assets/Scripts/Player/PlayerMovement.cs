using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour {
    [SerializeField] private float speed;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator animator;

    private Vector2 direction;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();    
    }

    private void Update() {
        if (!isLocalPlayer) return;
        HandleInput();
    }

    private void FixedUpdate() {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

    }

    private void HandleInput() {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        direction = new Vector2(horizontalInput, verticalInput).normalized;

        if (horizontalInput == 1) {
            CmdFlipSprite(false);
        }
        else if (horizontalInput == -1) {
            CmdFlipSprite(true);

        }

        if (direction != Vector2.zero) {
            animator.SetBool("isRunning", true);
        }
        else {
            animator.SetBool("isRunning", false);
        }
    }

    [Command]
    private void CmdFlipSprite(bool flipX) {
        RpcFlipSprite(flipX);
    }

    [ClientRpc]
    private void RpcFlipSprite(bool flipX) {
        sprite.flipX = flipX;

    }
}
