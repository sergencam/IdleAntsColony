using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public Text totalCoinText;

    [Header("Test")]
    public int giveCoin;

    private void Awake()
    {
        Instance = this;
        totalCoinText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.TotalCoin).ToString();

        //Test
        SetCoin(giveCoin);
    }

    public void SetCoin(int coinAmount)
    {
        //Girilen değere göre coin ekler veya azaltır
        var totalCoin = PlayerPrefs.GetInt(PlayerPrefsKeys.TotalCoin);
        totalCoin += coinAmount;
        PlayerPrefs.SetInt(PlayerPrefsKeys.TotalCoin, totalCoin);

        totalCoinText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.TotalCoin).ToString();
    }

    public int TotalCoin()
    {
        //Toplam coin miktarını döndürür
        return PlayerPrefs.GetInt(PlayerPrefsKeys.TotalCoin);
    }
}
