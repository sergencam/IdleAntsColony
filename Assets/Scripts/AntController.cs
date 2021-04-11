using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    SEARCH,
    TAKE,
    DELIVER
}

public class AntController : MonoBehaviour
{
    [SerializeField] private State _currentState;
    private Transform _targetFoodPiece;
    [SerializeField] private Transform _foodPieceCarryTransform;
    [SerializeField] private ParticleSystem _crumbFX;
    private GameObject[] _anthill;
    private int _targetAnthillIndex = 0;
    public float speed = 2f;
    [SerializeField] private float _stopDist = 1f;
    public float strength = 2f;
    private float _foodTakeTimer = 0f;
    private bool _isInAnthill;
    [HideInInspector] public bool imFinishedMyJob;


    private void Awake()
    {
        _anthill = GameObject.FindGameObjectsWithTag("Anthill");

        if (_crumbFX.isPlaying == true)
            _crumbFX.Stop();
    }

    private void Start()
    {
        speed = PlayerPrefs.GetFloat(PlayerPrefsKeys.Speed);
        strength = PlayerPrefs.GetFloat(PlayerPrefsKeys.Strength);
        GameManager.Instance.ants.Add(this);
        GameManager.Instance.totalAntsText.text = GameManager.Instance.ants.Count.ToString();
    }

    private void Update()
    {
        //Aktif state'e göre o state'in fonksiyonunu çalıştırıyoruz
        switch (_currentState)
        {
            case State.SEARCH:
                Search();
                break;
            case State.TAKE:
                Take();
                break;
            case State.DELIVER:
                Deliver();
                break;
        }

        //Tüm yemekler bitmişse ve aldığımız son parçayıda yuvaya bırakmışsak objemizi kapatıyoruz
        if (Food.Instance.foodPieces.Count <= 0)
        {
            if (_currentState == State.SEARCH && _targetFoodPiece == null && imFinishedMyJob == false)
            {
                GameManager.Instance.countOfAntsWhoFinishedTheirJob++;
                imFinishedMyJob = true;
            }
        }
    }

    private void Search()
    {
        FindTheNearestFood();

        if (!_targetFoodPiece) return;
        var targetPos = _targetFoodPiece.position;
        targetPos.y = 0f;

        var distBetweenFood = Vector3.Distance(transform.position, targetPos);

        //Yemek parçasının yanına gelmişsek bi sonraki state'e geçiyoruz
        if (distBetweenFood < _stopDist)
        {
            _currentState = State.TAKE;
        }
        else
        {
            //Eğer karınca yuvanın içerisindeyse önce karıncayı yuvasında çıkarıyoruz
            //Çıkardıktan sonra hedef yemeğine götürüyoruz
            if (_isInAnthill == true)
            {
                transform.position = Vector3.MoveTowards(transform.position, _anthill[0].transform.position, speed * Time.deltaTime);
                SmoothLookAt(_anthill[0].transform.position, 100f * speed);

                var distBetweenAnthillExit = Vector3.Distance(transform.position, _anthill[0].transform.position);
                if (distBetweenAnthillExit < 0.1f)
                {
                    _isInAnthill = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                SmoothLookAt(targetPos, 100f * speed);
            }
        }
    }

    private void Take()
    {
        _foodTakeTimer += Time.deltaTime;

        if (_crumbFX.isPlaying == false)
            _crumbFX.Play();

        //Yemek parçasını alabilme süresi dolduğunda parçayı sırtımıza alıp bir sonraki state'e geçiyoruz
        if (_foodTakeTimer >= strength)
        {
            if (_targetFoodPiece)
            {
                _targetFoodPiece.parent = _foodPieceCarryTransform;
                _targetFoodPiece.position = _foodPieceCarryTransform.position;
            }

            if (_crumbFX.isPlaying == true)
                _crumbFX.Stop();

            _foodTakeTimer = 0;

            _currentState = State.DELIVER;
        }
    }

    private void Deliver()
    {
        //Aldığımız parçayı yuvaya götürüyoruz
        var distBetweenAnthill = Vector3.Distance(transform.position, _anthill[_targetAnthillIndex].transform.position);
        transform.position = Vector3.MoveTowards(transform.position, _anthill[_targetAnthillIndex].transform.position, speed * Time.deltaTime);
        SmoothLookAt(_anthill[_targetAnthillIndex].transform.position, 100f * speed);

        if (distBetweenAnthill > _stopDist)
        {
            return;
        }
        else
        {
            _targetAnthillIndex++;
            if (_targetAnthillIndex < _anthill.Length)
                return;
        }

        //Yuvanın içine girdiğimizde bir sonraki state'e geçiyoruz
        _targetAnthillIndex = 0;
        _isInAnthill = true;
        _currentState = State.SEARCH;

        CoinManager.Instance.SetCoin(5);

        FXManager.Instance.PlayConfettiFX();

        if (_targetFoodPiece)
        {
            Destroy(_targetFoodPiece.gameObject);
            _targetFoodPiece = null;
        }
    }

    private void FindTheNearestFood()
    {
        //Hedef yemeği varsa veya yemek bitmişse return ediyoruz
        if (_targetFoodPiece || Food.Instance.foodPieces.Count <= 0) return;

        _targetFoodPiece = Food.Instance.foodPieces[0];

        //Tüm yemek parçalarının içinde dönüp en yakındaki yemek parçasını buluyoruz
        for (int i = 0; i < Food.Instance.foodPieces.Count; i++)
        {
            var distBetweenSettedFood = Vector3.Distance(transform.position, _targetFoodPiece.position);
            var distBetweenCurrentFood = Vector3.Distance(transform.position, Food.Instance.foodPieces[i].position);

            if (distBetweenCurrentFood < distBetweenSettedFood)
            {
                _targetFoodPiece = Food.Instance.foodPieces[i];
            }
        }

        //En yakındaki parçayı kendimize hedef yaptıktan sonra başka karıncaların aynı
        //yemeği hedef almaması için hedef parçamızı yemek parçası listesinden çıkarıyoruz
        Food.Instance.foodPieces.Remove(_targetFoodPiece);
    }

    private void SmoothLookAt(Vector3 target, float sSpeed)
    {
        Vector3 targetDir = target - transform.position;
        if (targetDir != Vector3.zero)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDir), sSpeed * Time.deltaTime);
    }
}
