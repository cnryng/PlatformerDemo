using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticePlayerV1 : MonoBehaviour
{

    public float RunSpeed;
    public float JumpForce;
    public float RollSpeed;
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public float djForce;

    private float attackTimer;
    public float attackDuration; //set to attack keyframes divided by sampling rate
    private float rollTimer;
    public float rollDuration; //

    public float djTimer, djDuration;
    public bool doubleJumped;
    public bool isDj;

    public Animator animator;
    public Transform _WallCast, _LowLeftCast, _LowRightCast;
    public Collider2D attackOneHitBox;

    public float yV;

    public bool animLocked; //eg. attacking or rolling

    private Rigidbody2D player;
    [SerializeField]
    private Vector2 _inputAxis;
    [SerializeField]
    private bool canRun, isRight;
    public bool onGround;
    private Vector2 scale;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _groundLeftHit;
    private RaycastHit2D _groundRightHit;
    private LayerMask environmentMask; //layer mask for environment only raycast detection


    // Start is called before the first frame update
    void Start()
    {
        canRun = true;
        isRight = true;
        onGround = true;
        animLocked = false;
        doubleJumped = false;
        isDj = false;
        attackTimer = 0;
        rollTimer = 0;
        djTimer = 0;
        player = gameObject.GetComponent<Rigidbody2D>();
        scale = player.transform.localScale;
        attackOneHitBox.enabled = false;
        environmentMask = LayerMask.GetMask("Default");  //Grid is in the Default layer
        Physics2D.IgnoreLayerCollision(8, 9); //Makes rigid body not collide with objects from layer 8 to 9 (inclusive). Player is layer 8 and Enemy is layer 9
    }


    // Update is called once per frame
    void Update()
    {

        _inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        WallDetection();
        GroundDetection();

        //Attack
        if (Input.GetButtonDown("Fire1") && onGround && !animLocked)
        {
            animLocked = true;
            player.velocity = new Vector2(0, player.velocity.y); //player stops moving once attack is active
            animator.SetBool("isAttacking", true);
            attackTimer = attackDuration;
            attackOneHitBox.enabled = true;
        }

        if (animator.GetBool("isAttacking"))
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("isAttacking", false);
                attackOneHitBox.enabled = false;
                animLocked = false;
            }
        }
        //

        //Roll input
        if (Input.GetKeyDown("space") && onGround && !animLocked)
        {
            animLocked = true;
            animator.SetBool("isRolling", true);
            rollTimer = rollDuration;
        }
        //

        yV = player.velocity.y; //used for testing landing condition (y-velocity = 0 <--> player has landed)

    }

    //called at a fixed rate set by physics engine (default is 50 per second)
    void FixedUpdate()
    {
        //Horizontal movement input
        if (_inputAxis.x != 0 && !animLocked) //if right/left is inputted
        {
            player.velocity = new Vector2(_inputAxis.x * RunSpeed * Time.deltaTime, player.velocity.y); //give player a velocity based on direction inputted

            if ((_inputAxis.x == -1 && isRight == true) || (_inputAxis.x == 1 && isRight == false)) //if moving left and facing right OR if moving right and facing left --> flip
            {
                Flip();
            }

            if (canRun) //play a run animation is player can run
            {
                animator.SetBool("isRunning", true);
            }
        }
        else if (_inputAxis.x == 0 && !animLocked) //if there is no horizontal input and animation is not locked
        {
            player.velocity = new Vector2(0, player.velocity.y); //set horizontal velocity to zero
            animator.SetBool("isRunning", false);
        }
        //

        //Make sure vertical velocity resets to zero once ground is touched before trying to do any other vertical velocity operations
        if (onGround)
        {
            player.velocity = new Vector2(player.velocity.x, 0);
        }
        //

        //when Player lands
        if (player.velocity.y <= 0.001 && onGround) //there seems to be no zero y velocity when transitioning between landing and jumping while holding the jump button -> player considered to be landed when y velocity is sufficiently small
        {
            canRun = true;
            animator.SetBool("isFalling", false);
        }
        //

        //Enable faster fallspeed and give variable jump heights
        if (player.velocity.y < 0 && !onGround) //if the player is moving downwards while not grounded
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

        //Jump
        if (_inputAxis.y > 0 && onGround && !animLocked)
        {
            player.velocity = new Vector2(player.velocity.x, player.velocity.y);
            player.velocity += Vector2.up * JumpForce;
            animator.SetBool("isJumping", true);
            FindObjectOfType<PlayerSounds>().Play("Jump");
            Debug.Log("Single Jump");
            canRun = false;
        }
        //



        //Double Jump --> has a set duration and a fixed height
        if (_inputAxis.y <= 0 && !doubleJumped && !onGround) //true for the frame where the player lets go of the up key while in the air
        {
            djTimer = djDuration;
        }

        if (!isDj && _inputAxis.y > 0 && djTimer > 0) //if not already double jumping, press jump to double jump
        {
            //can put an animation here (animation duration should be equal to djDuration)
            isDj = true;
            doubleJumped = true;
            FindObjectOfType<PlayerSounds>().Play("Jump");
            Debug.Log("Double Jump");
        }

        if (isDj)
        {
            player.velocity = new Vector2(player.velocity.x, djForce * Time.deltaTime);
            djTimer -= Time.deltaTime;
        }

        if (djTimer < 0) //stop double jump animation when timer runs out or collision with ground
        {
            //turn off animation here
            djTimer = 0;
            isDj = false; 
        }

        if(onGround) //can only double jump again once Player has landed
        {
            djTimer = 0;
            doubleJumped = false;        
        }
        //

        //Roll (tried putting this in normal update and low frame rate extended the roll) 
        if (animator.GetBool("isRolling"))
        {
            if (rollTimer > 0)
            {
                rollTimer -= Time.deltaTime;
                if (isRight)
                    player.velocity = new Vector2(RollSpeed * Time.deltaTime, player.velocity.y);
                else
                    player.velocity = new Vector2(-1 * RollSpeed * Time.deltaTime, player.velocity.y);
            }
            else
            {
                animator.SetBool("isRolling", false);
                animLocked = false;
            }
        }
        //




    }

    private void Flip()
    {
        scale.x *= -1;
        player.transform.localScale = scale;
        scale = player.transform.localScale;
        isRight = !isRight;
    }

    private void WallDetection()
    {
        float wallCastEndDistance = 0.03f;
        if (isRight)
        {
            _wallHit = Physics2D.Linecast(_WallCast.position, new Vector2(_WallCast.position.x - wallCastEndDistance, _WallCast.position.y), environmentMask);
            Debug.DrawLine(_WallCast.position, new Vector2(_WallCast.position.x - wallCastEndDistance, _WallCast.position.y), Color.red);
        }
        else
        {
            _wallHit = Physics2D.Linecast(new Vector2(_WallCast.position.x + wallCastEndDistance, _WallCast.position.y), new Vector2(_WallCast.position.x, _WallCast.position.y), environmentMask);
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
        _groundLeftHit = Physics2D.Linecast(_LowLeftCast.position, new Vector2(_LowLeftCast.position.x, _LowLeftCast.position.y - groundCastEndDistance), environmentMask);
        _groundRightHit = Physics2D.Linecast(_LowRightCast.position, new Vector2(_LowRightCast.position.x, _LowRightCast.position.y - groundCastEndDistance), environmentMask);

        Debug.DrawLine(_LowLeftCast.position, new Vector2(_LowLeftCast.position.x, _LowLeftCast.position.y - groundCastEndDistance), Color.red);
        Debug.DrawLine(_LowRightCast.position, new Vector2(_LowRightCast.position.x, _LowRightCast.position.y - groundCastEndDistance), Color.red);

        if ((_groundLeftHit) || (_groundRightHit)) 
        {
            //can't comparetag if raycast is not hit <- nullreference error
            /*if (_groundLeftHit.transform.CompareTag("Enemy") || _groundRightHit.transform.CompareTag("Enemy"))
            {
                onGround = false;
                Debug.Log("enemy hit");
            }
            else
                onGround = true;
            */
            onGround = true;
        }  
        else onGround = false;

    }

}

