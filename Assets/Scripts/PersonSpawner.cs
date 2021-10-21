using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Bank))]
public class PersonSpawner : MonoBehaviour
{
    public List<Bank> OtherHouses => _otherHouses;
    
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Person _personPrefab;
    [SerializeField] private Color _personColor;

    [SerializeField] private List<Bank> _otherHouses;
    [SerializeField] private Bank _commonBank;
    [SerializeField] private int _spawnSize;
    
    private void Awake()
    {
        _otherHouses = FindObjectsOfType<Bank>()
            .Except(_commonBank)
            .Except(GetComponent<Bank>())
            .ToList();
    }

    private void Start()
    {
        for (int i = 0; i < _spawnSize; i++)
        {
            var person = Instantiate(_personPrefab, _spawnPoint.position, Quaternion.identity);
            InitPerson(person);
            person.gameObject.SetActive(true);
        }
    }

    private void InitPerson(Person person)
    {
        person.CommonBank = _commonBank;
        person.HomeBank = GetComponent<Bank>();
        person.StillProbability.Value = Random.Range(0, 100);
        person.PointsGrabSize.Value = Random.Range(1, 6);
        person.HouseStill.Value = OtherHouses[Random.Range(0,3)];
        person.GetComponent<MeshRenderer>().material.color = _personColor;
    }

    public void SpawnPerson(Person person)
    {
        person.transform.position = _spawnPoint.position;
        person.gameObject.SetActive(true);
    }
}