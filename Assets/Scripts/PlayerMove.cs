using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    private bool isJumping = false;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        //Move By Button Control
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) //right max speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1)) //left max speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    Debug.Log(rayHit.collider.name);
                    anime.SetBool("IsJump", false);
                    isJumping = false;
                }
                
            }
        }
    }

    private void Update()
    {
        //Jump By Button Control
        if (Input.GetButton("Jump") && !anime.GetBool("IsJump") && !isJumping)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anime.SetBool("IsJump", true);
            isJumping = true;
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal")) 
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation Transition
        if (Mathf.Abs(rigid.velocity.x) < 0.3 && Mathf.Abs(rigid.velocity.y) < 0.1 && !isJumping)
            anime.SetBool("IsWalk", false);
        else
            anime.SetBool("IsWalk", true);
    }
}
