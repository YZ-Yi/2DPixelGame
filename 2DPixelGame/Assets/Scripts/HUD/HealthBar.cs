using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image mainHealthBar;
    public Image partialHealthBar;

    private float health;
    private float partialWidth;
    private float mainWidth;
    private float height;

    private PlayerStat playerStats;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = FindObjectOfType<PlayerStat>();
        health = playerStats.GetHealth();
        partialWidth = partialHealthBar.rectTransform.rect.width;
        mainWidth = mainHealthBar.rectTransform.rect.width;
        height = mainHealthBar.rectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        health = FindObjectOfType<PlayerStat>().GetHealth();
        Debug.Log(health);
        SetHealthBar();   
    }

    void SetHealthBar()
    {
        if (health > 90 && health <= 100)
            partialHealthBar.rectTransform.sizeDelta = new Vector2(partialWidth, height);
        else
        {
            partialHealthBar.rectTransform.sizeDelta = new Vector2(0, height);
            mainHealthBar.rectTransform.sizeDelta = new Vector2(mainWidth * health / 90, height);
        }
    }
}
