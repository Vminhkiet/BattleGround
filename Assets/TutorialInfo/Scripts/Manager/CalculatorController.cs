using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculatorController : MonoBehaviour
{
    public static CalculatorController instance;
    public InfoPlayerDatabase infoplayer;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    public CalculatorController getInstance()
    {
        if (instance == null) instance = this;
        return instance;
    }

    public int CalCoin(int coin)
    {
        int s = coin + infoplayer.getMoney();
        if (s < 0) { FindObjectOfType<NotificationController>().ShowMessage("Bạn không đủ tiền!"); return 0; }
        infoplayer.setMoney(s);
        return 1;
    }

    public void CalScore(int point)
    {
        int s = point + infoplayer.getScore();
        if (s < 0) { infoplayer.setScore(0); return; }
        infoplayer.setScore(s);
    }
}
