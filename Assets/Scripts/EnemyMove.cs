using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer sprite;
    CapsuleCollider2D capsuleCollider;
    public int NextMove;
    float NextThinkTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        NextThinkTime = Random.Range(2.0f, 5.0f);

        Invoke("Think", NextThinkTime);
    }

    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(NextMove, rigid.velocity.y);

        //Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + NextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        
        if (rayHit.collider == null) //낭떠러지를 만났을 때(앞에 Platform 오브젝트가 없을때)
        {
            NextMove = NextMove * -1;
            sprite.flipX = NextMove == 1;

            CancelInvoke();
            Invoke("Think", NextThinkTime);

        }
    }

    //Monster Die
    public void OnMonsterDamaged()
    {
        //Sprite Alpha
        sprite.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        sprite.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
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
        //Set Next Active
        NextMove = Random.Range(-1, 2);

        //Sprite Animation
        anime.SetInteger("WalkSpeed", NextMove);

        //Flip Sprite
        if (NextMove != 0)
            sprite.flipX = NextMove == 1;

        //재귀함수 : 맨 마지막에 작성
        Invoke("Think", NextThinkTime);
    }
}
