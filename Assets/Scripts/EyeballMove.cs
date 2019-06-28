using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

[AddComponentMenu("Camera-Control/Eyeball Look")]
public class EyeballMove : MonoBehaviour
{
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
    Quaternion _originalRotation;

    // Autoaim Vars
    Vector3 _vecToGoal;
    public float AutoAimTolerance = 0.1f;
    private float _autoAimStepper;
    public float StickAmount = 20.0f;
    public float AutoAimSpeed = 0.00025f;
    private float _autoAimResetTimer = 0.5f;
    private Coroutine ResettingAutoAim;
    private Vector3 _tempOldForward;

    private bool _lockEye;
    public bool LockEye
    {
        get => _lockEye;
        set
        {
            if (!value)
            {
                _autoAimStepper = 0.0f;
                _autoAimResetTimer = 0.75f;
                if(ResettingAutoAim == null) ResettingAutoAim = StartCoroutine(ResetAutoAim());
                _tempOldForward = Vector3.zero;
                Debug.Log("Reset");
            }
            _lockEye = value;
        }
    }

    void Start()
    {
        _originalRotation = transform.localRotation;
    }

    public void ControlEye()
    {
        //Resets the average rotation
        _rotAverageY = 0f;
        _rotAverageX = 0f;

        //Gets rotational input from the mouse
        _rotationX += Input.GetAxis(_inputMethodX) * SensitivityX;
        _rotationY += Input.GetAxis(_inputMethodY) * SensitivityY;

        Debug.Log(_rotationX);
        if(_rotationX * _rotationX > StickAmount || _rotationY * _rotationY > StickAmount)
        {
            Debug.Log("Unlocked");
            LockEye = false;
        }
        if (LockEye) return;

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

    public void AimCorrection()
    {
        float angleToGoal = Vector3.Dot(transform.forward, _vecToGoal);

        // bake these into the if statement later
        angleToGoal = ((angleToGoal + 1.0f) / 2.0f);
        angleToGoal = 1.0f - angleToGoal;

        //Debug.Log(angleToGoal + " < " + AutoAimTolerance);

        // if we are within autoAimTolernace% angle of the AND we have not moved the camera
        if (angleToGoal < AutoAimTolerance || _autoAimResetTimer < 0)
        {
            //Debug.Log("Locked: " + this.gameObject.name + "'s angleToGoal = " + angleToGoal);
            LockEye = true;

            if (_tempOldForward == Vector3.zero)
            {
                _tempOldForward = this.transform.forward;
                Debug.Log("Set old forward vector to " + _tempOldForward);

            }

            // Engage auto aim

            transform.forward = Vector3.Lerp(_tempOldForward, _vecToGoal, _autoAimStepper);

            _autoAimStepper += AutoAimSpeed * Time.deltaTime;
        }
    }

    public void GenerateAutoAimCoords(Transform goalToLookAt)
    {
        _vecToGoal = goalToLookAt.position - this.transform.position;
        _vecToGoal.Normalize();

        float angleToGoal = Vector3.Dot(transform.forward, _vecToGoal);
        angleToGoal = ((angleToGoal + 1.0f) / 2.0f);
        angleToGoal = 1.0f - angleToGoal;
        //Debug.Log("angleToGoal = " + angleToGoal);
    }

    public void SetInputs(string Xinput, string Yinput)
    {
        _inputMethodX = Xinput;
        _inputMethodY = Yinput;
    }

    IEnumerator ResetAutoAim()
    {
        while (_autoAimResetTimer > 0.0f)
        {
            _autoAimResetTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ResettingAutoAim = null;
    }

}