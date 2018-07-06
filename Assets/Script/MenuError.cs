using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class MenuError : MonoBehaviour
    {
        public Text errorText;
        public RectTransform form;
        public Translation translation;
        private bool showingError;

        public void Awake()
        {
            IO.OnFetchError += OnError;
            IO.OnSendError += OnError;
            gameObject.SetActive(false);
        }

        public void OnClickOK()
        {
            translation.View(form);
            showingError = false;
        }
        public void OnError(string s)
        {
            if (!showingError)
            {
                Debug.Log("lolwa");
                errorText.text = s;
                translation.View(GetComponent<RectTransform>());
                showingError = true;
            }
        }
    }
}