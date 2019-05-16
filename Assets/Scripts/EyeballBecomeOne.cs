using UnityEngine;

namespace Assets.Scripts
{
    public class EyeballBecomeOne : MonoBehaviour
    {
        public GameObject LeftEye, RightEye;
        [Range(0.0f, 0.5f)] public float DistanceBetween = 0.25f;


        // Start is called before the first frame update
        void Start()
        {
            // eye distance from their local zero should be -0.25 and 0.25 respectively
        }

        // Update is called once per frame
        void Update()
        {
            // The distance between the eye's scales with their dot product's closeness to 1 e.g. if both eyes are looking at the same thing, they should be identical

            // [-1, 1] // 1 if same dir and parallel, 0 if perpindicular, -1 if opposite dir and parallel
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
}
