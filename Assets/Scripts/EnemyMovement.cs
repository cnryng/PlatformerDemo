using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float RunSpeed;
    public Animator animator;
    public Transform wcStartHigh, wcStartLow;
    private RaycastHit2D wallCastHigh, wallCastLow;
    private Rigidbody2D enemy;
    public bool canRun, isRight;
    private Vector2 scale;
    public Collider2D stillAxe, swingAxe;
    public float vX;

    public float idleTimer, idleDuration;

    private LayerMask environmentMask;
    private LayerMask playerMask;

    // Start is called before the first frame update
    void Start()
    {
        canRun = true;
        isRight = true;
        idleTimer = idleDuration;
        enemy = gameObject.GetComponent<Rigidbody2D>();
        scale = enemy.transform.localScale;
        stillAxe.enabled = true;
        swingAxe.enabled = false;
        environmentMask = LayerMask.GetMask("Default");
        playerMask = LayerMask.GetMask("Player");
        

    }

    // Update is called once per frame
    void Update()
    {
        vX = enemy.velocity.x;
        WallDetection();
    }

    void FixedUpdate()
    {
        if (animator.GetBool("isRunning") && canRun)
        {
            if (isRight)
            {
                enemy.velocity = new Vector2(RunSpeed * Time.deltaTime, enemy.velocity.y);
            }
            else
            {
                enemy.velocity = new Vector2(-1 * RunSpeed * Time.deltaTime, enemy.velocity.y);
            }
        }

        else //if(!canRun)
        {
            enemy.velocity = new Vector2(0, 0);
            idleTimer -= Time.deltaTime;
            if (idleTimer < 0)
            {
                Flip();
                animator.SetBool("isRunning", true);
                idleTimer = idleDuration;
            }

        }
    }

    private void WallDetection()
    {
        float wallCastEndDistance = 0.03f;
        wallCastHigh = Physics2D.Linecast(wcStartHigh.position, new Vector2(wcStartHigh.position.x - wallCastEndDistance, wcStartHigh.position.y), environmentMask);
        wallCastLow = Physics2D.Linecast(wcStartLow.position, new Vector2(wcStartLow.position.x - wallCastEndDistance, wcStartLow.position.y), environmentMask);

        Debug.DrawLine(wcStartHigh.position, new Vector2(wcStartHigh.position.x - wallCastEndDistance, wcStartHigh.position.y), Color.red);
        Debug.DrawLine(wcStartLow.position, new Vector2(wcStartLow.position.x - wallCastEndDistance, wcStartLow.position.y), Color.red);

        if (wallCastHigh || wallCastLow)
        {
            canRun = false;
            animator.SetBool("isRunning", false);
        }
        else canRun = true;
    }

    private void AttackDetection()
    {
        float attackCastEndDistance = 1f;

    }


    private void Flip()
    {
        scale.x *= -1;
        enemy.transform.localScale = scale;
        scale = enemy.transform.localScale;
        isRight = !isRight;
    }
}
