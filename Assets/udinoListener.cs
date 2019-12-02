using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;
public class udinoListener : MonoBehaviour
{
    UduinoManager u;

    int test = 0;

    void Start()
    {
        UduinoManager.Instance.pinMode(2, PinMode.Input);
    }
    void Update()
    {
        Debug.Log("Uduino val: " + UduinoManager.Instance.digitalRead(2));
    }
}