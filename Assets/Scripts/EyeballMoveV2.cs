using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class EyeballMoveV2 : MonoBehaviour
{
    private bool _debugMode = false;

    // Input Rotation Vars
    string _inputMethodX = "Mouse X";
    string _inputMethodY = "Mouse Y"; // move to scriptable object

    float SensitivityX = 300f;
    float SensitivityY = 300f;

    float _rotationX = 0F;
    float _rotationY = 0F;

    // Goal AutoAim Vars
    Vector3 _vecToGoal;
    public float AutoAimTolerance = 0.005f;
    public float AutoAimSpeed = 0.15f;
    private float _autoAimStepper;
    private Vector3 _tempOldForward;

    private bool _lockEye = false;

    void Start()
    {
    }

    public void ControlEye()
    {
        if (_lockEye) return;

        // Gets rotational input from the mouse
        _rotationX = Input.GetAxis(_inputMethodX) * SensitivityX;
        _rotationY = Input.GetAxis(_inputMethodY) * SensitivityY;

        //Debug.Log(_inputMethodX + ": " + _rotationX + " \n" + _inputMethodY + ": " + _rotationY);

        // Get the rotation you will be at next as a Quaternion
        Quaternion yQuaternion = Quaternion.AngleAxis(_rotationY, -transform.right);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);

        Quaternion rotToApply = yQuaternion * xQuaternion;

        Debug.Log("Rotation to apply " + rotToApply.eulerAngles);

        //Rotate
        transform.rotation = rotToApply * transform.rotation;  //Quaternion.Slerp(transform.rotation, rotToApply * transform.rotation, 0.5f);
    }

    public bool CheckIfCooperating()
    {
        if (_debugMode)
        {
            return false;
        }
        if (_lockEye)
        {
            return true;
        }

        var angleToGoal = Vector3.Dot(transform.forward, _vecToGoal);

        // Scale for a percentage // TO-DO: bake this into auto aim tolerance later
        angleToGoal = ((angleToGoal + 1.0f) / 2.0f);
        angleToGoal = 1.0f - angleToGoal;

        //Debug.Log(angleToGoal + " < " + AutoAimTolerance);
        // if we are within autoAimTolernace% angle
        return angleToGoal < AutoAimTolerance;
    }

    public void MoveToFinalGoal()
    {
        _lockEye = true;

        _tempOldForward = this.transform.forward;

        var herm = Mathf.Lerp(0.0f, 1.0f, _autoAimStepper * _autoAimStepper * (3.0f - 2.0f * _autoAimStepper));
        transform.rotation = Quaternion.LookRotation(Vector3.Lerp(_tempOldForward, _vecToGoal, herm), Vector3.up);
        _autoAimStepper += (AutoAimSpeed * Time.deltaTime);
        //Debug.Log("step " + AutoAimSpeed * Time.deltaTime);


    }

    public void SetGoalCoords(Transform goalToLookAt)
    {
        //Debug.Log("Final pos " + goalToLookAt.position);
        _vecToGoal = goalToLookAt.position - this.transform.position;
        _vecToGoal.Normalize();
    }

    public void SetInputs(string Xinput, string Yinput)
    {
        _inputMethodX = Xinput;
        _inputMethodY = Yinput;
    }
}