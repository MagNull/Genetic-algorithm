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
        Persons.ObserveAdd()
            .Where(x => Persons.Count == 2)
            .Subscribe(_ => Select());
    }

    private void Select()
    {
        Person person1 = Persons[0];
        Person person2 = Persons[1];
        float probability1 = (float)(person1.PointsBringAmount + _probabilityFault) /
                             (person1.PointsBringAmount + person2.PointsBringAmount + 2 * _probabilityFault);
        float probability2 = (float)(person2.PointsBringAmount + _probabilityFault) /
                           (person1.PointsBringAmount + person2.PointsBringAmount + 2 * _probabilityFault);
        
        Person bestPerson;
        Person worstPerson;
        float minProb;
        if (probability1 > probability2)
        {
            bestPerson = person1;
            worstPerson = person2;
            minProb = probability2;
        }
        else
        {
            bestPerson = person2;
            worstPerson = person1;
            minProb = probability1;
        }
        
        RollGenotype(person1, minProb, worstPerson, bestPerson);
        RollGenotype(person2, minProb, worstPerson, bestPerson);
        
        _spawner.SpawnPerson(person1);
        _spawner.SpawnPerson(person2);
        Persons.Clear();
    }

    private void RollGenotype(Person person, float minProb, Person worstPerson, Person bestPerson)
    {
        person.PointsGrabSize = TestRoll(minProb) ? worstPerson.PointsGrabSize : bestPerson.PointsGrabSize;
        person.StillProbability = TestRoll(minProb) ? worstPerson.StillProbability : bestPerson.StillProbability;
        person.HouseStill = TestRoll(minProb) ? worstPerson.HouseStill : bestPerson.HouseStill;
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
