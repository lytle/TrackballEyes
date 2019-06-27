using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Eyeball Look")]
public class EyeballMove : MonoBehaviour
{
    string InputMethodX = "Mouse X";
    string InputMethodY = "Mouse Y"; // this is bad code

    public float sensitivityX = 20f;
    public float sensitivityY = 20f;

    float rotationX = 0F;
    float rotationY = 0F;
    private List<float> rotArrayX = new List<float>();
    float rotAverageX = 0F;

    private List<float> rotArrayY = new List<float>();
    float rotAverageY = 0F;

    public float frameCounter = 20;
    Quaternion originalRotation;

    // Autoaim Vars
    Vector3 vecToGoal;
    public float autoAimTolerance = 0.06f;
    private float autoAimTimer = 0.0f;

    bool lockEye = false;

    void Start()
    {
        originalRotation = transform.localRotation;
    }

    public void ControlEye()
    {
            //Resets the average rotation
            rotAverageY = 0f;
            rotAverageX = 0f;

            //Gets rotational input from the mouse
            rotationX += Input.GetAxis(InputMethodX) * sensitivityX;
            rotationY += Input.GetAxis(InputMethodY) * sensitivityY;

            /*if(rotationX * rotationX > 100f || rotationY * rotationY > 100f)
            {
                Debug.Log("Lock broke");
                lockEye = false;
            }
            if (lockEye) return;*/

            //Adds the rotation values to their relative array
            rotArrayY.Add(rotationY);
            rotArrayX.Add(rotationX);

            //If the arrays length is bigger or equal to the value of frameCounter remove the first value in the array
            if (rotArrayY.Count >= frameCounter)
            {
                rotArrayY.RemoveAt(0);
            }
            if (rotArrayX.Count >= frameCounter)
            {
                rotArrayX.RemoveAt(0);
            }

            //Adding up all the rotational input values from each array
            for (int j = 0; j < rotArrayY.Count; j++)
            {
                rotAverageY += rotArrayY[j];
            }
            for (int i = 0; i < rotArrayX.Count; i++)
            {
                rotAverageX += rotArrayX[i];
            }

            //Standard maths to find the average
            rotAverageY /= rotArrayY.Count;
            rotAverageX /= rotArrayX.Count;

            //Get the rotation you will be at next as a Quaternion
            Quaternion yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);

            //Rotate
            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

    public void AimCorrection()
    {
        float angleToGoal = Vector3.Dot(transform.forward, vecToGoal);

        // bake these into the if statement later
        angleToGoal = ((angleToGoal + 1.0f) / 2.0f);
        angleToGoal = 1.0f - angleToGoal;

        // if we are within autoAimTolernace% angle of the AND we have not moved the camera
        if (angleToGoal < autoAimTolerance)
        {
            //Debug.Log(angleToGoal);
            //lockEye = true;


            // Engage auto aim
            
        
            /*
            // The step size is equal to speed times frame time.
            float step = 4.0f * Time.deltaTime;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, vecToGoal, step, 0.0f);
            Debug.Log("newDir = " + newDir + "     vectoGoal = " + vecToGoal);

            // Move our position a step closer to the target.
            transform.rotation = Quaternion.LookRotation(newDir);
            Debug.Log("Set Rotation to: " + transform.forward);*/
        }
    }

    public void GenerateAutoAimCoords(Transform goalToLookAt)
    {
        vecToGoal = goalToLookAt.position - this.transform.position;
        vecToGoal.Normalize();

        float angleToGoal = Vector3.Dot(transform.forward, vecToGoal);
        angleToGoal = ((angleToGoal + 1.0f) / 2.0f);
        angleToGoal = 1.0f - angleToGoal;
        //Debug.Log("angleToGoal = " + angleToGoal);
    }

    public void SetInputs(string Xinput, string Yinput)
    {
        InputMethodX = Xinput;
        InputMethodY = Yinput;
    }

}