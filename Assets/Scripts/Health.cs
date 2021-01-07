using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public void setMaxHealth(int health)
    {
        maxHealth = health;
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth < damage)
        {
            currentHealth = 0;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

}
