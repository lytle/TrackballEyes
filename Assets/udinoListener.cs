using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
public class udinoListener : MonoBehaviour
{
    UduinoManager u;

    #region singleton
    public static udinoListener instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("udinoListener : Singleton Failure");
        }
    }
    #endregion

    [SerializeField]
    public Vector3 leftAxes;
    [SerializeField]
    public Vector3 rightAxes;

    int test = 0;

    void Start()
    {
        UduinoManager.Instance.pinMode(2, PinMode.Input);
    }
    void Update()
    {
        Debug.Log("Uduino val: " + UduinoManager.Instance.digitalRead(2));


        // DEBUG
        if (Input.GetKey(KeyCode.A)) leftAxes.x = -1.0f;
        else if (Input.GetKey(KeyCode.D)) leftAxes.x = 1.0f;
        else leftAxes.x = 0.0f;

        if (Input.GetKey(KeyCode.W)) leftAxes.y = 1.0f;
        else if (Input.GetKey(KeyCode.S)) leftAxes.y = -1.0f;
        else leftAxes.y = 0.0f;

        if (Input.GetKey(KeyCode.Q)) leftAxes.z = -1.0f;
        else if (Input.GetKey(KeyCode.E)) leftAxes.z = 1.0f;
        else leftAxes.z = 0.0f;

    }

}