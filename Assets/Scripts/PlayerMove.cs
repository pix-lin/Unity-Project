using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerMove : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    public GameManager gameManager;
    //private bool isJumping = false;
    CapsuleCollider2D capsuleCollider;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    public Animator anime;

    public Vector2 respawnPosition;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();

    }

    private void Start()
    {
        respawnPosition = transform.position;
        transform.position = new Vector2(-8, 2);
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

        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

        //Landing Platform
        if (rigid.velocity.y < 0)
        {
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anime.SetBool("IsJump", false);
                    respawnPosition = new Vector2(rayHit.point.x, rayHit.point.y + 0.75f); 
                    Debug.Log(respawnPosition);
                    //isJumping = false;
                }
                
            }
        }

    }

    private void Update()
    {
        //Jump By Button Control
        if (Input.GetButton("Jump") && !anime.GetBool("IsJump"))
        {
            anime.SetBool("IsJump", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            //isJumping = true;
            PlaySound("Jump");
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal")) 
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //Animation Transition
        if (Mathf.Abs(rigid.velocity.x) < 0.3 && Mathf.Abs(rigid.velocity.y) < 0.1 && !anime.GetBool("IsJump"))
            anime.SetBool("IsWalk", false);
        else
            anime.SetBool("IsWalk", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //Attack
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("Attack");
            }

            //Damaged
            else
            {
                OnPlayerDamaged(collision.transform.position);
                PlaySound("Damaged");
            }
                
        }

        else if (collision.gameObject.tag == "Spikes")
        {
            OnPlayerDamaged(collision.transform.position);
            PlaySound("Damaged");
        }
            
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            PlaySound("Item");

            //Point
            bool isBronze = collision.gameObject.name.Contains("Coin1");
            bool isSilver = collision.gameObject.name.Contains("Coin2");
            bool isGold = collision.gameObject.name.Contains("Coin3");
            
            if(isBronze)
                gameManager.stagePoint += 50;
            if (isSilver)
                gameManager.stagePoint += 70;
            if (isGold)
                gameManager.stagePoint += 90;

            //Deactive Item
            collision.gameObject.SetActive(false);
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //Next Stage
            PlaySound("Finish");
            gameManager.NextStage();

        }
            
    }

    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;
        //Reaction Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnMonsterDamaged();
    }

    void OnPlayerDamaged(Vector2 targetPossition)
    {
        //Health Down
        gameManager.HealthDown();

        //Change Layer (Imortal Active)
        gameObject.layer = 12;

        //View Alpha
        spriteRenderer.material.color = new Color(1, 1, 1, 0.4f);

        //Reaction Force
        int dirc = transform.position.x - targetPossition.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //Animation
        anime.SetTrigger("Damaged");
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 11;
        spriteRenderer.material.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        anime.SetBool("IsWalk", false);
        anime.SetBool("IsJump", false);
        anime.SetBool("IsDie", true);

        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    public void PlaySound(string action)
    {
        switch (action)
        {
            case "Jump":
                audioSource.clip = audioJump;
                Debug.Log("Jump Sound");
                break;
            case "Attack":
                audioSource.clip = audioAttack;
                break;
            case "Damaged":
                audioSource.clip = audioDamaged;
                break;
            case "Item":
                audioSource.clip = audioItem;
                break;
            case "Die":
                audioSource.clip = audioDie;
                break;
            case "Finish":
                audioSource.clip = audioFinish;
                break;
        }

        audioSource.Play();
    }
}
