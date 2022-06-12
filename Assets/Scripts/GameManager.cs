using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int playerHealth;
    public int stageNo;
    public GameObject[] stageObjects;
    public Player player;

    public Image[] UIhealthImage;
    public Text uiStageText;
    public Text uiPointText;
    public GameObject uiBtnRetry;


    private void Update()
    {
        uiPointText.text = (stagePoint + totalPoint).ToString();
    }

    public void NextStage()
    {
        totalPoint += stagePoint;
        stagePoint = 0;

        if (stageNo >= stageObjects.Length - 1)
        {

            ShowBtnRetry();
        }
        else
        {
            stageObjects[stageNo].SetActive(false);
            stageNo++;
            stageObjects[stageNo].SetActive(true);

            player.Reposition();
        }

        uiStageText.text = "STAGE " + (stageNo + 1);
    }

    public void HealthDecrease()
    {
        playerHealth--;

        UIhealthImage[playerHealth].color = new Color(1, 0, 0);

        if (playerHealth < 1)
        {
            player.OnDie();
        }
    }

    public void ShowBtnRetry()
    {
        Time.timeScale = 0;

        uiBtnRetry.SetActive(true);
    }

    public void ReStart()
    {
        SceneManager.LoadScene(0);

        Time.timeScale = 1;
    }
}
