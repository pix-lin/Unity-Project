using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer spriteRenderer;
    CircleCollider2D CircleCollider;
    public int NextMove;
    float NextThinkTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CircleCollider = GetComponent<CircleCollider2D>();
        NextThinkTime = Random.Range(2.0f, 5.0f);

        Invoke("Think", NextThinkTime);
    }

    private void Start()
    {
        //Set Next Active
        NextMove = Random.Range(-1, 2);
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(NextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + NextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHitDown = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        
        Vector2 toFrontVec = new Vector2(NextMove, 0);
        Debug.DrawRay(rigid.position, toFrontVec, new Color(1, 0, 0));
        RaycastHit2D rayHitFront = Physics2D.Raycast(rigid.position, toFrontVec, 1, LayerMask.GetMask("Platform"));
        if (rayHitDown.collider == null) //낭떠러지를 만났을 때(앞에 Platform 오브젝트가 없을때)
        {
            NextMove = NextMove * -1;
            spriteRenderer.flipX = NextMove == 1;

            CancelInvoke();
            Invoke("Think", NextThinkTime);
        }

        if (rayHitFront.collider != null)
        {
            if (rayHitFront.distance < 0.55f)
            {
                //Debug.Log(rayHitFront.distance);
                NextMove = NextMove * -1;
                spriteRenderer.flipX = NextMove == 1;

                CancelInvoke();
                Invoke("Think", NextThinkTime);
            }
        }
            
    }

    //Monster Die
    public void OnMonsterDamaged()
    {
        anime.SetBool("MonsterDie", true);

        //Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        CircleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("Deavtive", 5);
    }

    void Deavtive()
    {
        gameObject.SetActive(false);
    }

    //재귀 함수(Recursive)
    void Think()
    {
        NextMove = Random.Range(-1, 2);
        //Sprite Animation
        anime.SetInteger("WalkSpeed", NextMove);

        //Flip Sprite
        if (NextMove != 0)
            spriteRenderer.flipX = NextMove == 1;

        //재귀함수 : 맨 마지막에 작성
        Invoke("Think", NextThinkTime);
    }
}
