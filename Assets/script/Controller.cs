using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Networking;
// using Newtonsoft.Json.Linq;
using System.Linq;
using SimpleJSON;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);
    public int sum;
    public TMP_Text multiply;
    public TMP_InputField prediction;
    public Button startbtn;
    public float priceValue;
    public float totalValue;
    public TMP_InputField inputPriceText;
    public TMP_Text totalPriceText;
    public TMP_Text alertText;
    public GameObject cat;
    public GameObject modal;
    public TMP_Text errorMsg;
    public static APIForm apiform;
    public static Globalinitial _global;

    // Start is called before the first frame update
    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];
        float i_balance = float.Parse(usersInfo["amount"]);
        totalValue = i_balance;
        totalPriceText.text = totalValue.ToString("F2");
    }
    BetPlayer _player;
    void Start()
    {
        _player = new BetPlayer();
#if UNITY_WEBGL == true && UNITY_EDITOR == false
                            GameReady("Ready");
#endif
        priceValue = 10f;
        inputPriceText.text = priceValue.ToString("F2");
        totalValue = 100000f;
        totalPriceText.text = totalValue.ToString("F2");
        sum = 2;
        prediction.text = sum.ToString();
        multiply.text = "1.01X";
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void increase()
    {
        if (DirectionController.isDirection)
        {
            if (sum < 12)
            {
                sum = sum + 1;
                prediction.text = sum.ToString();
            }
        }
        else
        {
            if (sum < 11)
            {
                sum = sum + 1;
                prediction.text = sum.ToString();
            }
        }
        multi(sum);
    }
    public void decrease()
    {
        if (DirectionController.isDirection)
        {
            if (sum > 3)
            {
                sum = sum - 1;
                prediction.text = sum.ToString();
            }
        }
        else
        {
            if (sum > 2)
            {
                sum = sum - 1;
                prediction.text = sum.ToString();
            }
        }
        multi(sum);
    }

    public void multi(int val)
    {
        if (DirectionController.isDirection == false)
        {
            switch (val)
            {
                case 2:
                    multiply.text = "1.01X";
                    break;
                case 3:
                    multiply.text = "1.07X";
                    break;
                case 4:
                    multiply.text = "1.18X";
                    break;
                case 5:
                    multiply.text = "1.36X";
                    break;
                case 6:
                    multiply.text = "1.68X";
                    break;
                case 7:
                    multiply.text = "2.35X";
                    break;
                case 8:
                    multiply.text = "3.53X";
                    break;
                case 9:
                    multiply.text = "5.88X";
                    break;
                case 10:
                    multiply.text = "11.8X";
                    break;
                case 11:
                    multiply.text = "35.3X";
                    break;
            }
        }
        else
        {
            switch (val)
            {
                case 3:
                    multiply.text = "35.3X";
                    break;
                case 4:
                    multiply.text = "11.8X";
                    break;
                case 5:
                    multiply.text = "5.88X";
                    break;
                case 6:
                    multiply.text = "3.53X";
                    break;
                case 7:
                    multiply.text = "2.35X";
                    break;
                case 8:
                    multiply.text = "1.68X";
                    break;
                case 9:
                    multiply.text = "1.36X";
                    break;
                case 10:
                    multiply.text = "1.18X";
                    break;
                case 11:
                    multiply.text = "1.07X";
                    break;
                case 12:
                    multiply.text = "1.01X";
                    break;
            }
        }
    }

    public void over()
    {
        DirectionController.isDirection = false;
        if (sum == 12)
        {
            sum = 11;
            prediction.text = sum.ToString();
        }
        multi(sum);
    }
    public void under()
    {
        DirectionController.isDirection = true;
        if (sum == 2)
        {
            sum = 3;
            prediction.text = sum.ToString();
        }
        multi(sum);
    }
    public void play()
    {
        if (priceValue == 0)
        {
            StartCoroutine(resultAlert("Input balace!"));
        }
        else if (priceValue > 1000000)
        {
            StartCoroutine(resultAlert("Can't bet over 1000000!"));
        }
        else if (priceValue < 10)
        {
            StartCoroutine(resultAlert("Can bet over 10!"));
        }
        else
        {
            if (totalValue >= priceValue)
            {
                if (totalValue >= 10)
                {
                    DiceController1.isDice1 = 0;
                    DiceController2.isDice2 = 0;
                    startbtn.interactable = false;
                    StartCoroutine(Server());
                }
                else
                {
                    StartCoroutine(resultAlert("Balance need over 10!"));
                }
            }
            else
            {
                StartCoroutine(resultAlert("Insufficient balance!"));
            }
        }
    }
    IEnumerator Server()
    {
        yield return new WaitForSeconds(0.5f);
        WWWForm form = new WWWForm();
        form.AddField("userName", "_player.username");
        form.AddField("betAmount", priceValue.ToString());
        form.AddField("token", "_player.token");
        form.AddField("amount", totalValue.ToString());
        form.AddField("direction", DirectionController.isDirection.ToString());
        form.AddField("setSum", sum.ToString());
        _global = new Globalinitial();
        UnityWebRequest www = UnityWebRequest.Post(_global.BaseUrl + "api/start-Dice", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            string strdata = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            apiform = JsonUtility.FromJson<APIForm>(strdata);
            if (apiform.serverMsg == "Success")
            {
                DiceController1.isDice1 = apiform.diceArray[0];
                DiceController2.isDice2 = apiform.diceArray[1];
                yield return new WaitForSeconds(1f);
                totalValue = apiform.total;
                totalPriceText.text = totalValue.ToString("F2");
                yield return new WaitForSeconds(1f);
                if (apiform.raisePrice == 0)
                {
                    StartCoroutine(resultAlert("Better luck next time!"));
                }
                else
                {
                    StartCoroutine(resultAlert(apiform.msg));
                }
                yield return new WaitForSeconds(1f);
                startbtn.interactable = true;
            }
            else if (apiform.serverMsg == "Bet Error!")
            {
                StartCoroutine(Error(apiform.serverMsg));
            }
            else if (apiform.serverMsg == "WinLose Error!")
            {
                StartCoroutine(Error(apiform.serverMsg));
            }
            yield return new WaitForSeconds(1.5f);
            startbtn.interactable = true;
        }
        else
        {
            StartCoroutine(Error("Can't find server!"));
        }
    }
    public void inputChanged()
    {
        AlertController.isAlert = false;
        priceValue = float.Parse(string.IsNullOrEmpty(inputPriceText.text) ? "0" : inputPriceText.text);
    }
    IEnumerator Error(string msg)
    {
        modal.SetActive(true);
        ServerErrorAlert.isServer = true;
        errorMsg.text = msg;
        yield return new WaitForSeconds(0.001f);
    }
    public void close()
    {
        ServerErrorAlert.isServer = false;
        modal.SetActive(false);
        startbtn.interactable = true;
    }
    IEnumerator resultAlert(string msg)
    {
        cat.SetActive(true);
        alertText.text = msg;
        AlertController.isAlert = true;
        yield return new WaitForSeconds(4f);
        AlertController.isAlert = false;
        yield return new WaitForSeconds(1.5f);
        cat.SetActive(false);
    }
}
public class BetPlayer
{
    public string username;
    public string token;
}
