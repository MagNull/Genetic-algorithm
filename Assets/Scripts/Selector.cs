using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PersonSpawner))]
public class Selector : MonoBehaviour
{
    public ReactiveCollection<Person> Persons = new ReactiveCollection<Person>();

    [SerializeField] private int _probabilityFault = 2;
    [Range(0,1)][SerializeField] private float _mutationProbability;
    private PersonSpawner _spawner;
    private List<Bank> _otherHouses;

    private void Awake()
    {
        _spawner = GetComponent<PersonSpawner>();
    }

    private void Start()
    {
        _otherHouses = _spawner.OtherHouses;
        Persons
            .ObserveAdd()
            .Where(x => Persons.Count == 2)
            .Subscribe(_ => Select());
    }


    private float GetPersonProbability(Person person1, Person person2)
    {
        return (person1.PointsBringAmount + _probabilityFault) /
               (person1.PointsBringAmount + person2.PointsBringAmount + 2 * _probabilityFault);
    }
    private void Select()
    {
        var person1 = Persons[0];
        var person2 = Persons[1];
        float probability1 = GetPersonProbability(person1, person2);
        float probability2 = GetPersonProbability(person2, person1);
        
        Person bestPerson;
        Person worstPerson;
        float minProbability;
        if (probability1 > probability2)
        {
            bestPerson = person1;
            worstPerson = person2;
            minProbability = probability2;
        }
        else
        {
            bestPerson = person2;
            worstPerson = person1;
            minProbability = probability1;
        }
        
        RollGenotype(person1, minProbability, worstPerson, bestPerson);
        RollGenotype(person2, minProbability, worstPerson, bestPerson);
        
        _spawner.SpawnPerson(person1);
        _spawner.SpawnPerson(person2);
        Persons.Clear();
    }

    private void RollGenotype(Person person, float minProb, Person worstPerson, Person bestPerson)
    {
        person.PointsGrabSize = TestRoll(minProb) ? worstPerson.PointsGrabSize : bestPerson.PointsGrabSize;
        person.StillProbability = TestRoll(minProb) ? worstPerson.StillProbability : bestPerson.StillProbability;
        person.HouseStill = TestRoll(minProb) ? worstPerson.HouseStill : bestPerson.HouseStill;
        TryMutate(person);
    }

    private void TryMutate(Person person)
    {
        if (TestRoll(_mutationProbability))
        {
            int mutationRoll = Random.Range(1, 4);
            switch (mutationRoll)
            {
                case 1:
                    person.PointsGrabSize.Value = Random.Range(1, 6);
                    break;
                case 2:
                    person.StillProbability.Value = Random.Range(0, 101);
                    break;
                case 3:
                    person.HouseStill.Value = _otherHouses[Random.Range(0, 3)];
                    break;
            }
        }
    }

    private bool TestRoll(float probability)
    {
        float roll = Random.value;
        return roll <= probability;
    }
}
