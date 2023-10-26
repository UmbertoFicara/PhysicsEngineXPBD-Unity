using System.Collections.Generic;
using System.Linq;
using Physics;
using UnityEngine;

namespace Grabber
{
    public class GrabberSphereController : MonoBehaviour
    {
        public PhysicalWorld physicalWorld;
        public float colliderRadius;
        private GrabberSphere _grabber;
        public float velocityGrabber=50f;

        // Start is called before the first frame update
        private void Start()
        {
            _grabber = new GrabberSphere(transform.position,colliderRadius); 
        }

        private void Update()
        {
            float movX = Input.GetAxis("Horizontal");
            float movZ = Input.GetAxis("Vertical");
            bool movYpos = Input.GetKey(KeyCode.E);
            bool movYneg = Input.GetKey(KeyCode.Q);
            float movY = 0;
            if (movYpos)
                movY += 1;
            if(movYneg)
                movY -= 1;
            Vector3 mov = new Vector3(movX, movY, movZ) * (velocityGrabber * Time.deltaTime);
            transform.Translate(mov);
            
            _grabber.MoveGrab(transform.position);
        }
        
        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var temp = new List<IGrabbable>(physicalWorld.GetSoftBodies().ToList());
                _grabber.StartGrab(temp);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                _grabber.EndGrab(transform.position);
            }
        }
    }
}
