using UnityEngine;


//[ExecuteInEditMode]
public class EyeballBecomeOne : MonoBehaviour
{
    // Each camera, used for its world position.
    public GameObject LeftEye, RightEye;
    private EyeballMoveV2 LeftEyeMove, RightEyeMove;

    // This value is the "base" distance between eyes // eye distance from their local zero should be around -0.25 and 0.25 respectively at start, based on their randomized rotation
    [Range(0.0f, 0.5f)] public float DistanceBetween = 0.25f;

    public Transform CurrentGoal;

    // Start is called before the first frame update
    void Start()
    {
        LeftEyeMove = LeftEye.GetComponent<EyeballMoveV2>();
        RightEyeMove = RightEye.GetComponent<EyeballMoveV2>();

        // Apply inputs // To-Do: move to scriptable object
        LeftEyeMove.SetInputs("Left Analog X", "Left Analog Y");
        RightEyeMove.SetInputs("Right Analog X", "Right Analog Y");

        // Set current goals
        LeftEyeMove.SetGoalCoords(CurrentGoal);
        RightEyeMove.SetGoalCoords(CurrentGoal);

        // Randomize Eye Rotation
        LeftEyeMove.Spin(Random.rotation, 2.5f);
        RightEyeMove.Spin(Random.rotation, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // Focus the eyes
        ScaleEyeDistance();
        // Move them
        LeftEyeMove.ControlEye();
        RightEyeMove.ControlEye();

        // If we have a goal
        if (CurrentGoal != null) {
            // Check if Completed Level
            if (LeftEyeMove.CheckIfCooperating() && RightEyeMove.CheckIfCooperating())
            {
                LeftEyeMove.MoveToFinalGoal();
                RightEyeMove.MoveToFinalGoal();
            }
        }
    }

    // The distance between the eye's scales with their dot product's closeness to 1 e.g. if both eyes are looking at the same thing, they should be identical and "focused"
    private void ScaleEyeDistance()
    {
        // [-1, 1] // 1 if same dir and parallel, 0 if orthogonal, -1 if opposite dir and parallel
        float scale = Vector3.Dot(LeftEye.transform.forward, RightEye.transform.forward);
        //Debug.Log("Dot product = " + scale);

        // normalize to [0, 1] and make inversely proportional
        scale = ((scale + 1.0f) / 2.0f);
        scale = 1.0f - scale;

        // scale the default distance and set
        var thisFramesDistance = scale * DistanceBetween;

        //Debug.Log("\nDistance to set = " + thisFramesDistance);

        LeftEye.transform.localPosition = new Vector3(-thisFramesDistance, 0.0f, 0.0f);
        RightEye.transform.localPosition = new Vector3(thisFramesDistance, 0.0f, 0.0f);
    }


}
