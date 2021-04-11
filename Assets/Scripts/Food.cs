using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public static Food Instance;
    public List<Transform> foodPieces = new List<Transform>();

    private void Awake()
    {
        Instance = this;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            //Yemek parçalarını karınclara buldurabilmek için yemek parçalarını foodPieces listesine ekliyoruz
            foodPieces.Add(transform.GetChild(i).transform);
        }
    }
}
