using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timer;

    private GameObject[] possibleGoals;
    private GameObject currentGoal;



    // Start is called before the first frame update
    void Start()
    {
        if (possibleGoals == null)
            possibleGoals = GameObject.FindGameObjectsWithTag("Goal");

        if (possibleGoals.Length < 1)
            throw new Exception("No goals in the scene.");

        currentGoal = possibleGoals[UnityEngine.Random.Range(0, possibleGoals.Length - 1)];
        Debug.Log("Current Goal:" + currentGoal.name);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
