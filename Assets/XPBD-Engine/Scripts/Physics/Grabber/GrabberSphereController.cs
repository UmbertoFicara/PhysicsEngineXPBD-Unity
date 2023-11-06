using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XPBD_Engine.Scripts.Physics.Grabber.Interfaces;

namespace XPBD_Engine.Scripts.Physics.Grabber
{
    public class GrabberSphereController : MonoBehaviour
    {
        public PhysicalWorld physicalWorld;
        public float colliderRadius;
        private GrabberSphere _grabber;
        public float velocityGrabber=50f;
        public bool isRightGrabber;
        // Start is called before the first frame update
        private void Start()
        {
            _grabber = new GrabberSphere(transform.position,colliderRadius); 
        }

        private void Update()
        {
            bool movXneg =isRightGrabber?Input.GetKey(KeyCode.J) :Input.GetKey(KeyCode.A);
            bool movXpos =isRightGrabber?Input.GetKey(KeyCode.L): Input.GetKey(KeyCode.D);
            float movX = 0;
            if (movXpos)
                movX += 1;
            if(movXneg)
                movX -= 1;
            bool movZneg =isRightGrabber?Input.GetKey(KeyCode.K) :Input.GetKey(KeyCode.S);
            bool movZpos =isRightGrabber?Input.GetKey(KeyCode.I): Input.GetKey(KeyCode.W);
            float movZ = 0;
            if (movZpos)
                movZ += 1;
            if(movZneg)
                movZ -= 1;
            bool movYneg =isRightGrabber?Input.GetKey(KeyCode.U) :Input.GetKey(KeyCode.Q);
            bool movYpos =isRightGrabber?Input.GetKey(KeyCode.O): Input.GetKey(KeyCode.E);
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
            if (isRightGrabber? Input.GetKeyDown(KeyCode.RightShift): Input.GetKeyDown(KeyCode.LeftShift))
            {
                var temp = new List<IGrabbable>(physicalWorld.GetSoftBodies().ToList());
                _grabber.StartGrab(temp);
            }

            if (isRightGrabber? Input.GetKeyUp(KeyCode.RightShift): Input.GetKeyUp(KeyCode.LeftShift))
            {
                _grabber.EndGrab(transform.position);
            }
        }
    }
}
