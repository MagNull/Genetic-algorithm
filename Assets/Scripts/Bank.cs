using System;
using TMPro;
using UnityEngine;

public class Bank : MonoBehaviour
{
    [SerializeField] private int _points;
    private TextMeshProUGUI _pointsText;

    private void Awake()
    {
        _pointsText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _pointsText.text = _points.ToString();
    }

    public int GetPoints(int amount)
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

        _pointsText.text = _points.ToString();
        return point;
    }

    public void AddPoints(int amount)
    {
        _points += amount;
        _pointsText.text = _points.ToString();
    }
}