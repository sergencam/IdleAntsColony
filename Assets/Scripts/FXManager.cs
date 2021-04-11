using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;
    [SerializeField] private ParticleSystem _confettiFX;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayConfettiFX()
    {
        _confettiFX.Stop();
        _confettiFX.Play();
    }
}
