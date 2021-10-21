using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BankUIController))]
public class Bank : MonoBehaviour
{
    [SerializeField] private int _points;
    private BankUIController _bankUIController;

    private void Awake()
    {
        _bankUIController = GetComponentInChildren<BankUIController>();
    }

    private void Start()
    {
        _bankUIController.ChangePoints(_points);
    }

    public int TakePoints(int amount)
    {
        int point;
        if (_points - amount < 0)
        {
            point = _points;
            _points = 0;
        }
        else
        {
            point = amount;
            _points -= amount;
        }

        _bankUIController.ChangePoints(_points);
        return point;
    }

    public void AddPoints(int amount)
    {
        _points += amount;
        _bankUIController.ChangePoints(_points);
    }
}