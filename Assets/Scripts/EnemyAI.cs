using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer sprite;
    public int NextMove;
    float NextThinkTime;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
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
        if (rayHit.collider == null)
        {
            NextMove = NextMove * -1;
            CancelInvoke();
            Invoke("Think", NextThinkTime);
        }

    }

    //Àç±Í ÇÔ¼ö
    void Think()
    {
        //Set Next Active
        NextMove = Random.Range(-1, 2);

        Invoke("Think", NextThinkTime);

        //Sprite Animation
        anime.SetInteger("WalkSpeed", NextMove);

        //Flip Sprite
        if (NextMove != 0)
            sprite.flipX = NextMove == 1;
    }
}
