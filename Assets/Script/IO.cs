using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public delegate void Trigger();
public delegate void Error(string s);
public class IO : MonoBehaviour
{
    public static string URL;
    public static string uuid;
    public static IO singleton;
    public static bool ready;
    public static List<string> list = new List<string>();
    public static List<string> sentList = new List<string>();

    public static Trigger OnDataAdded;
    public static Trigger OnDataSendEnd;
    public static Trigger OnDataSendStart;
    public static Trigger OnReady;
    public static Error OnFetchError;
    public static Error OnSendError;


    public const int timeOutSeconds = 1000;
    public const int listMaxLength = 10;
    public const string dataPersistanceString = "?keep_for=600";
    public const string baseURL = "http://www.cross-copy.net/api/";
    public const string uuidURL = "https://www.uuidgenerator.net/api/version1";

    public static void Initialise(string channel)
    {
        if (singleton == null)
        {
            GameObject g = new GameObject("IO");
            singleton = g.AddComponent<IO>();
            DontDestroyOnLoad(g);
            DontDestroyOnLoad(singleton);
        }
        URL = baseURL + channel;
        singleton.StartCoroutine(singleton.GetUUID());
        list.Clear();
        sentList.Clear();
        ready = false;
    }
    public static void TrimList()
    {
        if (list.Count > listMaxLength)
        {
            list.RemoveRange(0, listMaxLength - list.Count);
        }
    }
    public static void SendText(string text)
    {
        sentList.Add(text);
    }
    public static void OnInitialised()
    {
        singleton.StartCoroutine(singleton.FetchData());
        singleton.StartCoroutine(singleton.SendData());
    }

    IEnumerator FetchData()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(URL+"?"+uuid);
            request.timeout = timeOutSeconds;
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
            {
 
                if (request.error == "Received no data in response")
                {
                    Debug.Log("timeout");
                }
                else
                {
                    if (OnFetchError != null)
                    {
                        OnFetchError(request.error);
                    }
                    Debug.Log(request.error);
                    break;

                }
            }
            else
            {
                list.Add(request.downloadHandler.text);
                if (OnDataAdded != null)
                {
                    OnDataAdded();
                }
                Debug.Log("GET Some:"+ request.downloadHandler.text);
            }
            request.Dispose();
        }
    }
    IEnumerator SendData()
    {
        while (true)
        {
            while (sentList.Count > 0)
            {
                UnityWebRequest request = UnityWebRequest.Put(URL + "?"+uuid, sentList[0]);
                if (OnDataSendStart != null)
                {
                    OnDataSendStart();
                }
                yield return request.SendWebRequest();
                if (request.isHttpError || request.isNetworkError)
                {
                    if (OnSendError !=null)
                        {
                        OnSendError(request.error);
                    }

                    Debug.Log(request.error);
                }
                else
                {
if (OnDataSendEnd != null)
                    {
                        OnDataSendEnd();
                    }
                    Debug.Log("sent:" + sentList[0]);
                }
                sentList.RemoveAt(0);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator GetUUID()
    {
        UnityWebRequest request = UnityWebRequest.Get(uuidURL);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
if (OnFetchError != null)
            {
                OnFetchError(request.error);
            }
                Debug.Log(request.error);
        }
        else
        {
            uuid = "device_id=" + request.downloadHandler.text;
            ready = true;
            OnInitialised();
if (OnReady != null)
            {
                OnReady();
            }
        }
        request.Dispose();
    }

}
