using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rigid;
    public int move;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        MoveLogic();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(move, rigid.velocity.y);


        Vector2 frontVec = new Vector2(rigid.position.x + move * 0.5f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 2f, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            if(spriteRenderer.flipY == false)
                Turn();
        }
    }

    void MoveLogic()
    {

        move = Random.Range(-1, 2);

        anim.SetInteger("WalkSpeed", move);
        
        if(move != 0)
            spriteRenderer.flipX = move == 1;
        
        Invoke("MoveLogic", 2);
    }

    void Turn()
    {

        move = -move;
        spriteRenderer.flipX = move == 1;

        CancelInvoke();
        Invoke("MoveLogic", 2);
    }

    public void OnDamaged()
    {
        capsuleCollider.enabled = false;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        spriteRenderer.flipY = true;

        move = 0;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("DeActive", 5);

    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
