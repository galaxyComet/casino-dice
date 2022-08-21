using UnityEngine;
using System.Collections;

public class JsonType
{
    public string userName;
    public float betAmount;
    public bool isBetting;
    public string token;
    public float amount;
    public bool direction;
    public int setSum;
}

public class ReceiveJsonObject
{
    public int[] diceArray;
    public string msg;
    public float total;
    public float raisePrice;
    public ReceiveJsonObject()
    {
    }
    public static ReceiveJsonObject CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ReceiveJsonObject>(data);
    }
}