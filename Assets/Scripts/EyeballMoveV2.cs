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

    float SensitivityX = 60f;
    float SensitivityY = 60f;

    float _rotationX = 0F;
    float _rotationY = 0F;

    private static int fidelity = 11;
    float[] _rotXArray = new float[fidelity];
    float[] _rotYArray = new float[fidelity];
    int counter = 0;

    // Goal AutoAim Vars
    Vector3 _vecToGoal;
    public float AutoAimTolerance = 0.005f;
    public float AutoAimSpeed = 0.15f;
    private float _autoAimStepper;
    private Vector3 _tempOldForward;

    private bool _lockEye = false;

    Coroutine RandomSpinning;

    #region Init
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
    #endregion

    public void ControlEye()
    {
        if (_lockEye) return;

        // Gets rotational input from the mouse
        _rotationX = Input.GetAxis(_inputMethodX) * SensitivityX;
        _rotationY = Input.GetAxis(_inputMethodY) * SensitivityY;
        // Put into array
        _rotXArray[counter] = _rotationX;
        _rotYArray[counter] = _rotationY;
        if (++counter > _rotXArray.Length - 1) counter = 0;
        // Get average for smoothness
        _rotationX = AverageArray(_rotXArray);
        _rotationY = AverageArray(_rotYArray);

        // Get the rotation you will be at next as a Quaternion
        Quaternion yQuaternion = Quaternion.AngleAxis(_rotationY, -transform.right);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotationX, Vector3.up);

        Quaternion rotToApply = yQuaternion * xQuaternion;

        //Debug.Log("Rotation to apply " + rotToApply.eulerAngles);

        //Rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToApply * transform.rotation, 0.5f);
    }

    public void Spin(Quaternion spin, float time)
    {
        if (RandomSpinning != null) return;
        // Spin for x seconds
        RandomSpinning = StartCoroutine(RandomSpin(spin, time));
    }

    IEnumerator RandomSpin(Quaternion spin, float totalTime)
    {
        _lockEye = true;
        float timer = 0;
        // Scale back our random rotation a little bit so it isnt super duper fast
        spin = Quaternion.Slerp(spin, Quaternion.identity, 0.8f);
        // Main loop of applying a damping rotation
        while(timer < totalTime)
        {
            var curScale = timer / totalTime;
            var toSpin = Quaternion.Slerp(spin, Quaternion.identity, curScale);
            // dont look at all these magic numbers
            if(curScale < 0.5f) toSpin =  Quaternion.Slerp(toSpin, Quaternion.Slerp(Random.rotation, Quaternion.identity, 0.9f), 0.2f);
            this.transform.rotation = toSpin * this.transform.rotation;
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        // Reset
        _lockEye = false;
        RandomSpinning = null;
    }

    public bool CheckIfCooperating()
    {
        if (_debugMode)
        {
            return false;
        }
        /*if (_lockEye) // To-Do: make this "found goal"
        {
            return true;
        }*/

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

    #region Utility
    private float AverageArray(float[] inArray)
    {
        float result = 0;

        for (int i = 0; i < inArray.Length; i++)
        {
            result += inArray[i];
        }

        return result / inArray.Length;
    }
    #endregion
}