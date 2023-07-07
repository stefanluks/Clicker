using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RedeController : MonoBehaviour
{
    public static RedeController _instancia { get; private set; }
    public string _url = "http://localhost:8000/api/Clicker";


    private void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            Destroy(this.gameObject);
        } else {
            _instancia = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private IEnumerator SavePontuacao(Pontuacao Jogador, Action<bool> callback){
        UnityWebRequest www = UnityWebRequest.Get(_url);
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if(www.isHttpError || www.isNetworkError){
            callback(false);
        }else{
            string text = www.downloadHandler.text;
        }
        callback(true);

        // WWWForm form = new WWWForm();
        // form.AddField("myField", "myData");

        // using (UnityWebRequest www = UnityWebRequest.Post("https://www.my-server.com/myform", form))
        // {
        //     yield return www.SendWebRequest();

        //     if (www.result != UnityWebRequest.Result.Success)
        //     {
        //         Debug.Log(www.error);
        //     }
        //     else
        //     {
        //         Debug.Log("Form upload complete!");
        //     }
        // }
    }

    public IEnumerator GetRanking(Action<Ranking> callback){
        UnityWebRequest www = UnityWebRequest.Get(_url);
        www.downloadHandler = new DownloadHandlerBuffer();

        Ranking rank = new Ranking();

        yield return www.SendWebRequest();

        if(www.isHttpError || www.isNetworkError){
            rank = null;
        }else{
            string text = www.downloadHandler.text;
            rank = JsonUtility.FromJson<Ranking>(text);
        }
        callback(rank);
    }

}
