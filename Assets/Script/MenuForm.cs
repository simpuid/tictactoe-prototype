using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuForm : MonoBehaviour
    {
        public Translation translation;
        public RectTransform loading;
        public RectTransform game;
        public InputField playerInput;
        public InputField roomInput;
        public Button joinButton;

        public string playerName;
        public string roomCode;
        public void OnClickJoin()
        {
            if (CheckString() == false)
            {
                return;
            }
            translation.View(loading);
            Client.singleton = new Client(playerName, roomCode);
            Client.singleton.form = this;
            Client.singleton.gameUI = game.GetComponent<GameUI>();
        }
        public bool CheckString()
        {
            playerName = playerName.Trim();
            roomCode = roomCode.Trim();
            if (playerName != "" && roomCode != "")
            {
                joinButton.interactable = true;
                return true;
            }
            else
            {
                joinButton.interactable = false;
                return false;
            }
        }
        public void OnChangePlayerName()
        {
            playerName = playerInput.text;
            CheckString();
        }
        public void OnChangeRoomCode()
        {
            roomCode = roomInput.text;
            CheckString();
        }
        public void OnStartGame()
        {
            translation.View(game);
            game.GetComponent<GameUI>().Initialise();
        }
        public void Awake()
        {
            OnChangePlayerName();
            OnChangeRoomCode();
            CheckString();
        }

    }
}