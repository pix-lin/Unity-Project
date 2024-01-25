using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;

    public void NextStage()
    {
        //Change Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
        }
        //Game Clear
        else
        {
            //Player Control Lock
            Time.timeScale = 0;
        }
        
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
            health--;
        else
        {
            //Player Die Effect
            player.OnDie();

            //Result UI

            //Retry Button UI

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Health Down
            HealthDown();

            //Player Reposition
            if (health > 1)
                PlayerReposition();
        }   
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-8, 1, -1);
        player.VelocityZero();
    }
}
