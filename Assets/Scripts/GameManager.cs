using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [HideInInspector] public List<AntController> ants = new List<AntController>();
    public Text totalAntsText;
    [SerializeField] private List<Food> _foods = new List<Food>();
    [SerializeField] private Transform _foodParent;
    [HideInInspector] public int countOfAntsWhoFinishedTheirJob;

    private void Awake()
    {
        Instance = this;

        SpawnFood(false);
    }

    private void Update()
    {
        //Tüm karıncalar işini bitirmişse yeni yemeği spawn ediyoruz
        if (countOfAntsWhoFinishedTheirJob >= ants.Count)
        {
            SpawnFood(true);
            countOfAntsWhoFinishedTheirJob = 0;

            foreach (var item in ants)
            {
                item.imFinishedMyJob = false;
            }
        }
    }

    public void SpawnFood(bool setNewFood)
    {
        //Yemeğimizi spawn ediyoruz
        var randomFood = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentFood);
        if (setNewFood == true)
        {
            randomFood++;
            if (randomFood >= _foods.Count)
                randomFood = 0;

            PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentFood, randomFood);
        }

        if (Food.Instance)
            Destroy(Food.Instance.gameObject);

        var food = Instantiate(_foods[randomFood]);
        food.transform.parent = _foodParent;
        food.transform.position = _foodParent.position;
        food.transform.rotation = _foodParent.rotation;
    }
}
