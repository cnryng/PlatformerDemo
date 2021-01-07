using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public Animator playerAnim;
    public PracticePlayerV1 playerScript;
    public float deathDuration;
    public float deathTimer;

    void Start()
    {
        deathTimer = deathDuration;
        playerScript = gameObject.GetComponent<PracticePlayerV1>();
    }
    void Update()
    {
        if(currentHealth == 0)
        {
            playerAnim.SetBool("isDying", true);
            playerScript.animLocked = true;
        }
        if(playerAnim.GetBool("isDying"))
        {
            if(deathTimer < 0)
            {
                playerAnim.SetBool("isDying", false);
                playerScript.animLocked = false;
                gameObject.SetActive(false);
            }
            deathTimer -= Time.deltaTime;
        }
    }
}
