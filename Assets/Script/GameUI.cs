using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text nameText;
    public Text opNameText;
    public Text scoreText;
    public Text opScoreText;
    public Text comment;
    public Sprite mySprite;
    public Sprite opSprite;
    public RectTransform turnCounter;

    public Button[] button;

    private void Awake()
    {
        Initialise();
    }
    public void Initialise()
    {
        if (Client.singleton != null)
        {
            nameText.text = Client.singleton.name;
            opNameText.text = Client.singleton.opName;
            scoreText.text = Client.singleton.score.ToString();
            opScoreText.text = Client.singleton.opScore.ToString();
        }
        for (int i = 0; i < button.Length; i++)
        {
            button[i].GetComponent<Image>().sprite = null;
            button[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        Client.dataUpdate += UIUpdate;
    }
    public Button Get(int x,int y)
    {
        return button[x + y * 3];
    }
    public void OnClick(int i)
    {
        Client.Press(i % 3, i / 3);
    }
    public void UIUpdate()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Color c = button[y * 3 + x].GetComponent<Image>().color;
                if (Client.game.data[x,y] == Data.Empty)
                {
                    button[y * 3 + x].GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0);
                }
                else
                {
                    button[y * 3 + x].GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1);
                    if (Client.game.data[x,y] == Data.My)
                    {
                        button[y * 3 + x].GetComponent<Image>().sprite = mySprite;
                    }
                    else
                    {
                        button[y * 3 + x].GetComponent<Image>().sprite = opSprite;
                    }
                }
            }
        }
        Comment(Client.comment);
    }
    public void Comment(string s)
    {
        comment.text = s;
    }
    public IEnumerator WinCoroutie()
    {
        Comment(Client.singleton.name + " Wins");
        UIUpdate();
        yield return new WaitForSeconds(3);
        Client.RestartRequest(Data.My);
    }
    public IEnumerator LoseCoroutine()
    {
        Comment(Client.singleton.opName + " Wins");
        UIUpdate();
        yield return new WaitForSeconds(3);
        Client.RestartRequest(Data.Op);
    }
    public IEnumerator FinishCoroutine()
    {
        Comment("Game Finished");
        UIUpdate();
        yield return new WaitForSeconds(3);
        Client.RestartRequest(Data.Empty);
    }
    private void Update()
    {
        float f = turnCounter.anchorMin.x;
        if (Client.myTurn)
        {
            f = Mathf.MoveTowards(f, 0, Time.deltaTime);
        }
        else
        {
            f = Mathf.MoveTowards(f, 0.5f, Time.deltaTime);
        }
        turnCounter.anchorMax = new Vector2(f + 0.5f, 1);
        turnCounter.anchorMin = new Vector2(f, 0);
    }
}
