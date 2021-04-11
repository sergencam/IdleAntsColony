using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CostAndLevelProps
{
    public string name;
    public string itemUpgradeCostKey;
    public string itemLevelKey;
    public int firstCost;
    public float costMultiplier;
    public Button upgradeButton;
    public Text costText;
    public Text levelText;
}

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<CostAndLevelProps> costAndLevelProps = new List<CostAndLevelProps>();

    [Space]

    [Header("Speed")]
    [SerializeField] private int _speedIndexOnCostAndLevelPropsList;
    [SerializeField] private float _firstSpeed;
    [SerializeField] private float _speedMultiplier;

    [Space]

    [Header("Workers")]
    [SerializeField] private int _workersIndexOnCostAndLevelPropsList;
    [SerializeField] private GameObject _worker;
    [SerializeField] private Collider _workerSpawnArea;
    [SerializeField] private Transform _workersParent;

    [Space]

    [Header("Strength")]
    [SerializeField] private int _strengthIndexOnCostAndLevelPropsList;
    [SerializeField] private float _firstStrength;
    [SerializeField] private float _strengthMultiplier;

    [Space]

    public Sprite normalUpgradeButtonSprite;
    public Sprite disableUpgradeButtonSprite;

    private void Awake()
    {
        SetKeys();

        //Oyun kapatıp açıldığında en sonki karınca sayısı kadar karınca spawnlıyoruz
        for (int i = 0; i < PlayerPrefs.GetInt(PlayerPrefsKeys.WorkersLevel); i++)
        {
            var randomSpawnPos = new Vector3(Random.Range(_workerSpawnArea.bounds.min.x, _workerSpawnArea.bounds.max.x), 0, Random.Range(_workerSpawnArea.bounds.min.z, _workerSpawnArea.bounds.max.z));
            var antObject = Instantiate(_worker, randomSpawnPos, _worker.transform.rotation);
            antObject.transform.parent = _workersParent;
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    public void UpgradeSpeed()
    {
        //Hızı upgrade ediyoruz
        var speed = PlayerPrefs.GetFloat(PlayerPrefsKeys.Speed);
        speed *= _speedMultiplier;
        PlayerPrefs.SetFloat(PlayerPrefsKeys.Speed, speed);

        //Upgrade bedelini toplam coinden çıkartıyoruz
        CoinManager.Instance.SetCoin(-PlayerPrefs.GetInt(PlayerPrefsKeys.SpeedUpgradeCost));

        //Upgrade bedelini arttırıyoruz
        SetUpgradeCost(PlayerPrefsKeys.SpeedUpgradeCost, costAndLevelProps[_speedIndexOnCostAndLevelPropsList].costMultiplier);

        //Hız levelını arttırıyoruz
        SetUpgradeLevel(PlayerPrefsKeys.SpeedLevel);

        //Sahnedeki karıncaların hızlarını güncelliyoruz
        foreach (var item in GameManager.Instance.ants)
        {
            item.speed = speed;
        }
    }

    public void UpgradeWorkers()
    {
        //Yeni işçi ekliyoruz
        var randomSpawnPos = new Vector3(Random.Range(_workerSpawnArea.bounds.min.x, _workerSpawnArea.bounds.max.x), 0, Random.Range(_workerSpawnArea.bounds.min.z, _workerSpawnArea.bounds.max.z));
        var antObject = Instantiate(_worker, randomSpawnPos, _worker.transform.rotation);
        antObject.transform.parent = _workersParent;

        //Upgrade bedelini toplam coinden çıkartıyoruz
        CoinManager.Instance.SetCoin(-PlayerPrefs.GetInt(PlayerPrefsKeys.WorkersUpgradeCost));

        //Upgrade bedelini arttırıyoruz
        SetUpgradeCost(PlayerPrefsKeys.WorkersUpgradeCost, costAndLevelProps[_workersIndexOnCostAndLevelPropsList].costMultiplier);

        //Worker levelını arttırıyoruz
        SetUpgradeLevel(PlayerPrefsKeys.WorkersLevel);
    }

    public void UpgradeStrength()
    {
        //Hızı upgrade ediyoruz
        var strength = PlayerPrefs.GetFloat(PlayerPrefsKeys.Strength);
        strength *= _strengthMultiplier;
        PlayerPrefs.SetFloat(PlayerPrefsKeys.Strength, strength);

        //Upgrade bedelini toplam coinden çıkartıyoruz
        CoinManager.Instance.SetCoin(-PlayerPrefs.GetInt(PlayerPrefsKeys.StrengthUpgradeCost));

        //Upgrade bedelini arttırıyoruz
        SetUpgradeCost(PlayerPrefsKeys.StrengthUpgradeCost, costAndLevelProps[_strengthIndexOnCostAndLevelPropsList].costMultiplier);

        //Worker levelını arttırıyoruz
        SetUpgradeLevel(PlayerPrefsKeys.StrengthLevel);

        //Sahnedeki karıncaların güçlerini güncelliyoruz
        foreach (var item in GameManager.Instance.ants)
        {
            item.strength = strength;
        }
    }

    private void SetKeys()
    {
        //Boş olan keylere ilk değerlerini veriyoruz
        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.Speed))
            PlayerPrefs.SetFloat(PlayerPrefsKeys.Speed, _firstSpeed);

        if (!PlayerPrefs.HasKey(PlayerPrefsKeys.Strength))
            PlayerPrefs.SetFloat(PlayerPrefsKeys.Strength, _firstStrength);

        foreach (var item in costAndLevelProps)
        {
            if (!PlayerPrefs.HasKey(item.itemUpgradeCostKey))
            {
                PlayerPrefs.SetInt(item.itemUpgradeCostKey, item.firstCost);
            }

            if (!PlayerPrefs.HasKey(item.itemLevelKey))
            {
                PlayerPrefs.SetInt(item.itemLevelKey, 1);
            }
        }
    }

    private void SetUpgradeCost(string playerPrefsKey, float costMultiplier)
    {
        var upgradeCost = PlayerPrefs.GetInt(playerPrefsKey);
        upgradeCost = (int)(costMultiplier * upgradeCost);
        PlayerPrefs.SetInt(playerPrefsKey, upgradeCost);
    }

    private void SetUpgradeLevel(string playerPrefsKey)
    {
        var level = PlayerPrefs.GetInt(playerPrefsKey);
        level++;
        PlayerPrefs.SetInt(playerPrefsKey, level);
    }

    private void UpdateUI()
    {
        var totalCoin = CoinManager.Instance.TotalCoin();

        foreach (var item in costAndLevelProps)
        {
            //Coinimiz yetmediği özelliklerin butonlarını disable ediyoruz
            if (totalCoin >= PlayerPrefs.GetInt(item.itemUpgradeCostKey))
            {
                item.upgradeButton.interactable = true;
                item.upgradeButton.GetComponent<Image>().sprite = normalUpgradeButtonSprite;
            }
            else if (totalCoin < PlayerPrefs.GetInt(item.itemUpgradeCostKey))
            {
                item.upgradeButton.interactable = false;
                item.upgradeButton.GetComponent<Image>().sprite = disableUpgradeButtonSprite;
            }

            //Textleri güncelliyoruz
            item.costText.text = PlayerPrefs.GetInt(item.itemUpgradeCostKey).ToString();
            item.levelText.text = "Level " + PlayerPrefs.GetInt(item.itemLevelKey).ToString();
        }
    }
}
