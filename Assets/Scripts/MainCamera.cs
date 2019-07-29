using UnityEngine;

namespace Assets.Scripts
{
    public class MainCamera : MonoBehaviour
    {
        [HideInInspector] public Transform target;
        [HideInInspector] public Settings settings;

        public Vector3 offset;
        public float smoothSpeed;

        void LateUpdate()
        {
            if (target)
            {
                FollowTarget();
            }
        }

        private void FollowTarget()
        {
            var desiredPosition = target.position + offset;

            var axisZLimeter = GetComponent<Camera>().orthographicSize;
            var axisXLimeter = axisZLimeter * GetComponent<Camera>().aspect;

            if (desiredPosition.x < axisXLimeter)
            {
                desiredPosition.x = axisXLimeter;
            }
            else if (desiredPosition.x > settings.labirintSize - axisXLimeter)
            {
                desiredPosition.x = settings.labirintSize - axisXLimeter;
            }
        
            if (desiredPosition.z < axisZLimeter)
            {
                desiredPosition.z = axisZLimeter;
            }
            else if (desiredPosition.z > settings.labirintSize - axisZLimeter)
            {
                desiredPosition.z = settings.labirintSize - axisZLimeter;
            }


            var smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothPosition;
        }
    }
}
