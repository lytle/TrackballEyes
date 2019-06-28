using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

[AddComponentMenu("Camera-Control/Eyeball Look")]
public class EyeballMove : MonoBehaviour
{
    private bool _debugMode = false;

    // Input Rotation Vars
    string _inputMethodX = "Mouse X";
    string _inputMethodY = "Mouse Y"; // this is bad code

    public float SensitivityX = 20f;
    public float SensitivityY = 20f;

    float _rotationX = 0F;
    float _rotationY = 0F;
    private List<float> _rotArrayX = new List<float>();
    float _rotAverageX = 0F;

    private List<float> _rotArrayY = new List<float>();
    float _rotAverageY = 0F;

    public float FrameCounter = 20;
    private Quaternion _originalRotation;

    // Goal AutoAim Vars
    Vector3 _vecToGoal;
    public float AutoAimTolerance = 0.005f;
    public float AutoAimSpeed = 0.15f;
    private float _autoAimStepper;
    private Vector3 _tempOldForward;

    private bool _lockEye = false;

    void Start()
    {
        _originalRotation = transform.localRotation;
    }

    public void ControlEye()
    {
        if (_lockEye) return;

        //Resets the average rotation
        _rotAverageY = 0f;
        _rotAverageX = 0f;

        //Gets rotational input from the mouse
        _rotationX += Input.GetAxis(_inputMethodX) * SensitivityX;
        _rotationY += Input.GetAxis(_inputMethodY) * SensitivityY;

        //Adds the rotation values to their relative array
        _rotArrayY.Add(_rotationY);
            _rotArrayX.Add(_rotationX);

            //If the arrays length is bigger or equal to the value of frameCounter remove the first value in the array
            if (_rotArrayY.Count >= FrameCounter)
            {
                _rotArrayY.RemoveAt(0);
            }
            if (_rotArrayX.Count >= FrameCounter)
            {
                _rotArrayX.RemoveAt(0);
            }

            //Adding up all the rotational input values from each array
            for (int j = 0; j < _rotArrayY.Count; j++)
            {
                _rotAverageY += _rotArrayY[j];
            }
            for (int i = 0; i < _rotArrayX.Count; i++)
            {
                _rotAverageX += _rotArrayX[i];
            }

            //Standard maths to find the average
            _rotAverageY /= _rotArrayY.Count;
            _rotAverageX /= _rotArrayX.Count;

            //Get the rotation you will be at next as a Quaternion
            Quaternion yQuaternion = Quaternion.AngleAxis(_rotAverageY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(_rotAverageX, Vector3.up);

            //Rotate
            transform.localRotation = _originalRotation * xQuaternion * yQuaternion;
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
       Debug.Log("step " + AutoAimSpeed * Time.deltaTime);


    }

    public void SetGoalCoords(Transform goalToLookAt)
    {
        Debug.Log("Final pos " + goalToLookAt.position);
        _vecToGoal = goalToLookAt.position - this.transform.position;
        _vecToGoal.Normalize();
    }

    public void SetInputs(string Xinput, string Yinput)
    {
        _inputMethodX = Xinput;
        _inputMethodY = Yinput;
    }
}