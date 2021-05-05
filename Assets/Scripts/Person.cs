using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Person : MonoBehaviour
{
    [Header("Changeable")]
    public ReactiveProperty<int> PointsGrabSize = new ReactiveProperty<int>();
    public ReactiveProperty<int> StillProbability = new ReactiveProperty<int>();
    public ReactiveProperty<Bank> HouseStill = new ReactiveProperty<Bank>();
    public float LifeTime = 10;
    
    [HideInInspector] public Bank HomeBank;
    [HideInInspector] public List<Bank> OtherBanks;
    [HideInInspector] public Bank CommonBank;

    [Header("Constant")]
    public float PointsSpeedCoefficient = 1;
    [SerializeField] private float _baseSpeed = 10;
    [SerializeField] private int _points;
    [SerializeField] private Bank _target;
    [SerializeField] private float _timer;
    private int _generation = 0;
    private MeshRenderer _meshRenderer;
    private string _teamName;
    private Bank _lastBank;
    private NavMeshAgent _navMeshAgent;
    private int _pointsBringAmount;
    private Selector _selector;

    public int PointsBringAmount => _pointsBringAmount;

    public string TeamName => _teamName;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _meshRenderer = GetComponent<MeshRenderer>();
        DefineTeam();
    }

    private void DefineTeam()
    {
        Color _color = _meshRenderer.sharedMaterial.color;
        if (_color == Color.blue) _teamName = "Blue";
        else if (_color == Color.green) _teamName = "Green";
        else if (_color == Color.red) _teamName = "Red";
        else if (_color == new Color(1,1,0,1)) _teamName = "Yellow";
    }
    

    private void Start()
    {
        _selector = HomeBank.GetComponent<Selector>();
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0) Die();
        }
    }

    private void OnEnable()
    {
        _generation++;
        _target = HomeBank;
        _lastBank = HomeBank;
        name = "Person " + TeamName + " " + _generation;
        LifeTime = Random.Range(5, 15) + Random.value;
        _timer = LifeTime;
        _navMeshAgent.speed = _baseSpeed;
        _pointsBringAmount = 0;
        _points = 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Bank bank) && bank == _target)
        {
            _lastBank = bank;
            Bank newTarget;
            if (bank == HomeBank)
            {
                HomeBank.AddPoints(_points);
                _pointsBringAmount += _points;
                _points = 0;
                int roll = Random.Range(1, 101);
                if (roll <= StillProbability.Value)
                {
                    newTarget = HouseStill.Value;
                }
                else
                {
                    newTarget = CommonBank;
                }
                _navMeshAgent.speed = _baseSpeed;
            }
            else
            {
                _points += bank.GetPoints(PointsGrabSize.Value);
                _navMeshAgent.speed = _baseSpeed - PointsSpeedCoefficient * _points;
                newTarget = HomeBank;
            }
            _navMeshAgent.SetDestination(newTarget.transform
                .position);
            _target = newTarget;
        }
    }

    private void Die()
    {
        if (_lastBank != HomeBank)
        {
            _lastBank.AddPoints(_points);
        }
        gameObject.SetActive(false);
        _selector.Persons.Add(this);
    }
}