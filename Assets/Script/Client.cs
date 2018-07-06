using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Client
{
    public static Client singleton;
    public static Game game;
    public static Trigger dataUpdate;
    public string name;
    public string opName;
    public int score;
    public int opScore;
    public static bool myTurn;
    public static bool iStartedGame;
    public static bool host;
    public static string comment;
    public Menu.MenuForm form;
    public GameUI gameUI;
    public Client(string name, string code)
    {
        IO.Initialise(code);
        this.name = name;
        singleton = this;
        IO.OnReady += OnIOReady;
        IO.OnDataAdded += OnMessageRecieve;
    }
    public void OnIOReady()
    {
        IO.SendText(Message.hi + "|" + name);
    }
    public static void Press(int x, int y)
    {
        Debug.Log(x.ToString() + " +++ " + y.ToString());
        Debug.Log(game.started);
        if (game.started)
        {
            if (myTurn)
            {
                game.data[x, y] = Data.My;
                IO.SendText(Message.call + "|" + x.ToString() + "|" + y.ToString());
                Verify();

                myTurn = false;
                comment = singleton.opName + " Turn";
                UIUpdate();
            }
        }
    }
    public static bool CheckWin(Data d)
    {
        Color c;
        if (d == Data.My)
        {
            c = Color.green;
        }
        else
        {
            c = Color.red;
        }
        for (int i = 0; i < 3; i++)
        {
            bool bb = true;
            for (int x = 0; x < 3; x++)
            {
                if (game.data[i, x] != d)
                {
                    bb = false;
                }
            }
            if (bb)
            {
                for (int x = 0; x < 3; x++)
                {
                    {
                        singleton.gameUI.Get(i, x).GetComponent<Image>().color = c;
                    }
                }
                return true;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            bool bb = true;
            for (int x = 0; x < 3; x++)
            {
                if (game.data[x, i] != d)
                {
                    bb = false;
                }
            }
            if (bb)
            {
                for (int x = 0; x < 3; x++)
                {
                    {
                        singleton.gameUI.Get(x, i).GetComponent<Image>().color = c;
                    }
                }
                return true;
            }
        }
        bool b = true;
        for (int i = 0; i < 3; i++)
        {
            if (game.data[i, i] != d)
            {
                b = false;
            }
        }
        if (b)
        {
            for (int i = 0; i < 3; i++)
            {
                {
                    singleton.gameUI.Get(i, i).GetComponent<Image>().color = c;
                }
            }
            return true;
        }
        b = true;
        for (int i = 0; i < 3; i++)
        {
            if (game.data[2 - i, i] != d)
            {
                b = false;
            }
        }
        if (b)
        {
            for (int i = 0; i < 3; i++)
            {
                {
                    singleton.gameUI.Get(2 - i, i).GetComponent<Image>().color = c;
                }
            }
            return true;
        }
        return false;
    }
    public static bool CheckEmpty()
    {
        bool b = false;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (game.data[x, y] == Data.Empty)
                {
                    b = true;
                }
            }
        }
        return b;
    }
    public void OnMessageRecieve()
    {
        while (IO.list.Count > 0)
        {
            ProcessText(IO.list[0]);
            IO.list.RemoveAt(0);
        }
    }
    public void ProcessText(string s)
    {
        string[] txt = s.Split('|');
        if (s.StartsWith(Message.hi))
        {
            GameStart(txt[1]);
            host = true;
            IO.SendText(Message.hiCallBack + "|" + name);
        }
        else if (s.StartsWith(Message.hiCallBack))
        {
            GameStart(txt[1]);
            host = false;
            IO.SendText(Message.youStart);
            myTurn = false;
            iStartedGame = false;
            comment = opName + " Turn";
            form.OnStartGame();
            UIUpdate();
        }
        else if (s.StartsWith(Message.youStart))
        {
            myTurn = true;
            iStartedGame = true;
            comment = name + " Turn";
            form.OnStartGame();
            UIUpdate();
        }
        else if (s.StartsWith(Message.call))
        {
            myTurn = true;
            int x = -1;
            int y = -1;
            if (int.TryParse(txt[1], out x) && int.TryParse(txt[2], out y))
            {
                game.data[x, y] = Data.Op;
                Debug.Log("Recea");
                Verify();
                UIUpdate();
            }
        }
        else if (s.StartsWith(Message.clean))
        {
            IO.SendText(Message.cleanCallBack);
            if (txt[1] == "You")
            {
                iStartedGame = true;
                myTurn = true;
            }
            else
            {
                iStartedGame = false;
                myTurn = false;
            }

            Restart();

        }
        else
        {
            Debug.Log("Unknow Message:" + s);
        }
    }
    public void GameStart(string opName)
    {
        this.opName = opName;
        this.opScore = 0;
        this.score = 0;
        game = new Game();
        game.Initialise();
        UIUpdate();
    }

    public static void UIUpdate()
    {
        if (dataUpdate != null)
        {
            dataUpdate();
        }
    }
    public static void Verify()
    {
        if (CheckWin(Data.My))
        {
            singleton.gameUI.StartCoroutine(singleton.gameUI.WinCoroutie());
            singleton.score++;
            Debug.Log("Win");
        }
        else if (CheckWin(Data.Op))
        {
            singleton.gameUI.StartCoroutine(singleton.gameUI.LoseCoroutine());
            Debug.Log("Lose");
            singleton.opScore++;
        }
        else if (!CheckEmpty())
        {
            singleton.gameUI.StartCoroutine(singleton.gameUI.FinishCoroutine());
            Debug.Log("Finish");
        }
        else
        {
            comment = singleton.name + " turn";
        }
    }
    public static void Restart()
    {
        game = new Game();
        game.Initialise();
        singleton.gameUI.Initialise();
        if (myTurn)
        {
            comment = singleton.name + " turn";
        }
        else
        {
            comment = singleton.opName + " turn";
        }
        UIUpdate();
    }
    public static void RestartRequest(Data d)
    {
        if (host)
        {
            string s = "";
            if (d == Data.My)
            {
                s = "Me";
            }
            else if (d == Data.Op)
            {
                s = "You";
            }
            else if (d == Data.Empty)
            {
                s = "None";
            }
            if (iStartedGame)
            {

                IO.SendText(Message.clean + "|" + "You" + "|" + s);
                iStartedGame = false;
                myTurn = false;
            }
            else
            {

                IO.SendText(Message.clean + "|" + "Me" + "|" + s);
                iStartedGame = true;
                myTurn = true;
            }
        }
        Restart();
    }
}

public class Message
{
    public const string hi = "HINormal";
    public const string hiCallBack = "HICallBack";
    public const string youStart = "YOUStart";
    public const string call = "CallNormal";
    public const string callWin = "CallWin";
    public const string callFinish = "CallFinish";
    public const string clean = "CleanNormal";
    public const string cleanCallBack = "CleanCallBack";
}
