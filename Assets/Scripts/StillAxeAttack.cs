using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StillAxeAttack : MonoBehaviour
{
    public int damage;
    public Health playerHealth;
    public float velocity;
    public Transform transform;

    // Start is called before the first frame update
    void Start()
    {
        damage = 10;
        transform = gameObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {

    }

    void OnTriggerEnter2D(Collider2D col) //should be OnTriggerStay2D once 
    {
        if (col.isTrigger != true && col.CompareTag("Player"))
        {
            playerHealth = col.GetComponent<Health>();
            //knockback
            //player.velocity = Vector2.Reflect(player.velocity, )
            //
            playerHealth.ReceiveDamage(damage);
            Debug.Log("Player Hit: Player HP = " + playerHealth.GetCurrentHealth());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
