using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    private float health;
    private int coin;

    // Start is called before the first frame update
    void Start()
    {
        coin = 0;
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if (health < damage)
            health = 0;
        else
            health -= damage;

        Debug.Log("Current Health: " + health);
    }

    public float GetHealth()
    {
        return health;
    }
}
