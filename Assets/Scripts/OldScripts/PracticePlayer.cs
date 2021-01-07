using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public float RunSpeed;
    public float JumpForce;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float attackTimer;
    public float attackDuration;
    public Animator animator;
    public Transform _WallCast, _LowLeftCast, _LowRightCast;
    public Collider2D attackOneHitBox;

    public float yV;

    private Rigidbody2D player;
    private Vector2 _inputAxis;
    [SerializeField]
    private bool canRun;
    private bool isRight;
    public bool onGround;
    private Vector2 scale;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _groundLeftHit;
    private RaycastHit2D _groundRightHit;


    // Start is called before the first frame update
    void Start()
    {
        canRun = true;
        isRight = true;
        onGround = true;
        attackTimer = 0;
        player = gameObject.GetComponent<Rigidbody2D>();
        scale = player.transform.localScale;
        attackOneHitBox.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        _inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        WallDetection();
        GroundDetection();

        //Attack
        if (Input.GetButtonDown("Fire1") && onGround)
        {
            animator.SetBool("isAttacking", true);
            attackTimer = attackDuration;
            attackOneHitBox.enabled = true;
        }

        if(animator.GetBool("isAttacking"))
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("isAttacking", false);
                attackOneHitBox.enabled = false;
            }
        }
        //

        yV = player.velocity.y; //used for testing landing condition (y-velocity = 0 <--> player has landed)

    }

    void FixedUpdate()
    {

        if (_inputAxis.x != 0 && !animator.GetBool("isAttacking")) //if right/left is inputted
        {
            player.velocity = new Vector2(_inputAxis.x * RunSpeed * Time.deltaTime, player.velocity.y); //give player a velocity based on direction inputted

            if ((_inputAxis.x == -1 && isRight == true) || (_inputAxis.x == 1 && isRight == false)) //if moving left and facing right OR if moving right and facing left --> flip
            {
                scale.x *= -1;
                player.transform.localScale = scale;
                scale = player.transform.localScale;
                isRight = !isRight;
            }

            if (canRun) //play a run animation is player can run
            {
                animator.SetBool("isRunning", true);
            }
        }
        else //if there is no horizontal input
        {
            player.velocity = new Vector2(0, player.velocity.y); //set horizontal velocity to zero
            animator.SetBool("isRunning", false);
        }

        /*if (_inputAxis.y > 0) //if the player is can jump and a jump input is read
        {
            player.AddForce(new Vector2(0, JumpForce)); //add a vertical force to the player
        }*/

        //Enable faster fallspeed and give variable jump heights
        if (player.velocity.y < 0)
        {
            player.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        else if (player.velocity.y > 0 && _inputAxis.y != 1)
        {
            player.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //

        //when Player lands
        if (player.velocity.y <= 0.001 && onGround) //there seems to be no zero y velocity when transitioning between landing and jumping while holding the jump button -> player considered to be landed when y velocity is sufficiently small
        {
            canRun = true;
            animator.SetBool("isFalling", false);
        }
        //

        //Jump
        if (_inputAxis.y > 0 && onGround && !animator.GetBool("isAttacking"))
        {
            player.velocity = new Vector2(player.velocity.x, player.velocity.y);
            player.velocity += Vector2.up * JumpForce;
            animator.SetBool("isJumping", true);
            canRun = false;
        }
        //


    }

    private void Flip(float direction)
    {

    }

    private void WallDetection()
    {
        float wallCastEndDistance = 0.03f;
        if (isRight)
        {
            _wallHit = Physics2D.Linecast(_WallCast.position, new Vector2(_WallCast.position.x - wallCastEndDistance, _WallCast.position.y));
            Debug.DrawLine(_WallCast.position, new Vector2(_WallCast.position.x - wallCastEndDistance, _WallCast.position.y), Color.red);
        }
        else
        {
            _wallHit = Physics2D.Linecast(new Vector2(_WallCast.position.x + wallCastEndDistance, _WallCast.position.y), new Vector2(_WallCast.position.x, _WallCast.position.y));
            Debug.DrawLine(new Vector2(_WallCast.position.x + wallCastEndDistance, _WallCast.position.y), new Vector2(_WallCast.position.x, _WallCast.position.y), Color.red);
        }
        if (_wallHit)
        {
            canRun = false;
            animator.SetBool("isRunning", false);
        }
        else canRun = true;
    }

    private void GroundDetection()
    {
        float groundCastEndDistance = 0.03f;
        _groundLeftHit = Physics2D.Linecast(_LowLeftCast.position, new Vector2(_LowLeftCast.position.x, _LowLeftCast.position.y - groundCastEndDistance));
        _groundRightHit = Physics2D.Linecast(_LowRightCast.position, new Vector2(_LowRightCast.position.x, _LowRightCast.position.y - groundCastEndDistance));

        Debug.DrawLine(_LowLeftCast.position, new Vector2(_LowLeftCast.position.x, _LowLeftCast.position.y - groundCastEndDistance), Color.red);
        Debug.DrawLine(_LowRightCast.position, new Vector2(_LowRightCast.position.x, _LowRightCast.position.y - groundCastEndDistance), Color.red);

        if (_groundLeftHit || _groundRightHit) onGround = true;
        else onGround = false;

    }

}

