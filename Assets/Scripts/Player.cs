using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rigid;
    public float jumpPower;
    public float maxSpeed;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public GameManager manager;
    CapsuleCollider2D capsuleCollider;
    AudioSource audioSource;
    public AudioClip audioJump;
    public AudioClip audioDie;
    public AudioClip audioEat;
    public AudioClip audioFinish;
    public AudioClip audioDamaged;
    public AudioClip audioAttack;

    Vector2 initPosition;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        initPosition = transform.position;
    }

    private void Update()
    {
        //Stop
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x, rigid.velocity.y);
        }

        if(Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Walking Animation
        if (rigid.velocity.normalized.x == 0)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

        //Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            PlayAudio("JUMP");
        }

        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down * 0.5f, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                //Debug.Log(rayHit.collider.name + " " + rayHit.distance);
                if (rayHit.distance < 0.65f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }

    }

    void FixedUpdate()
    {
        //Move
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Spped
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < -maxSpeed)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Item")
        {
            bool isBronze = collision.name.Contains("Bronze");
            bool isSilver = collision.name.Contains("Silver");
            bool isGold = collision.name.Contains("Gold");

            if (isBronze)
                manager.stagePoint += 50;
            else if (isSilver)
                manager.stagePoint += 100;
            else if (isGold)
                manager.stagePoint += 300;

            collision.gameObject.SetActive(false);

            PlayAudio("EAT");
        }

        else if (collision.gameObject.tag == "GameManager")
        {
            OnDamaged(transform.position);
            Reposition();
        }

        else if(collision.gameObject.tag == "Finish")
        {
            manager.NextStage();
            audioSource.clip = audioFinish;
            audioSource.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(transform.position.y > collision.transform.position.y && rigid.velocity.y < 0)
            {
                //Attack
                OnAttack(collision.gameObject);

                
            }
            else
            {
                //Damaged
                OnDamaged(collision.transform.position);
            }
        }

    }

    void PlayAudio(string action)
    {
        switch(action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "EAT":
                audioSource.clip = audioEat;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
        }

        audioSource.Play();
    }

    void OnAttack(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        enemy.OnDamaged();

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        manager.stagePoint += 100;

        audioSource.clip = audioAttack;
        audioSource.Play();
    }

    void OnDamaged(Vector2 targetPos)
    {
        anim.SetBool("isJumping", true);

        gameObject.layer = 8;
        
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int direction = targetPos.x - rigid.position.x > 0 ? -1 : 1;

        rigid.AddForce(new Vector2(direction, 1) * 5, ForceMode2D.Impulse);
        anim.SetTrigger("doDamaged");

        manager.HealthDecrease();

        audioSource.clip = audioDamaged;
        audioSource.Play();

        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 6;

        spriteRenderer.color = new Color(1, 1, 1);
    }

    public void Reposition()
    {
        rigid.velocity = new Vector2(0, 0);

        transform.position = initPosition;
    }

    public void OnDie()
    {
        spriteRenderer.flipY = true;

        capsuleCollider.enabled = false;

        manager.ShowBtnRetry();
    }

}
