using System;
using System.Collections.Generic;
using DefaultNamespace;
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

    [HideInInspector] public Bank HomeBank;
    [HideInInspector] public Bank CommonBank;

    [Header("Constant")]
    public float PointsSpeedCoefficient = 1;
    [SerializeField] private float _baseSpeed = 10;
    [SerializeField] private int _points;
    [SerializeField] private Bank _target;
    private float _timer;
    private int _generation;
    private MeshRenderer _meshRenderer;
    private string _teamName;
    private Bank _lastBank;
    private NavMeshAgent _navMeshAgent;
    private int _pointsBringAmount;
    private Selector _selector;
    private Timer _lifeTimer;

    public int PointsBringAmount => _pointsBringAmount;

    public string TeamName => _teamName;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _selector = HomeBank.GetComponent<Selector>();
        DefineTeam();
    }

    private void DefineTeam()
    {
        var _color = _meshRenderer.sharedMaterial.color;
        if (_color == Color.blue) _teamName = "Blue";
        else if (_color == Color.green) _teamName = "Green";
        else if (_color == Color.red) _teamName = "Red";
        else if (_color == new Color(1,1,0,1)) _teamName = "Yellow";
    }

    private void Update()
    {
        _lifeTimer.Tick(Time.deltaTime);
    }

    private void OnEnable()
    {
        _generation++;
        _target = HomeBank;
        _lastBank = HomeBank;
        name = "Person " + TeamName + " " + _generation;
        _lifeTimer = new Timer(Random.Range(5, 15) + Random.value, Die);
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
                BringPointsToHome();
                newTarget = GetNextBank();
                _navMeshAgent.speed = _baseSpeed;
            }
            else
            {
                _points += bank.TakePoints(PointsGrabSize.Value);
                _navMeshAgent.speed = _baseSpeed - PointsSpeedCoefficient * _points;
                
                newTarget = HomeBank;
            }
            _navMeshAgent.SetDestination(newTarget.transform
                .position);
            _target = newTarget;
        }
    }

    private Bank GetNextBank()
    {
        Bank newTarget;
        int roll = Random.Range(1, 101);
        if (roll <= StillProbability.Value)
        {
            newTarget = HouseStill.Value;
        }
        else
        {
            newTarget = CommonBank;
        }

        return newTarget;
    }

    private void BringPointsToHome()
    {
        HomeBank.AddPoints(_points);
        _pointsBringAmount += _points;
        _points = 0;
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