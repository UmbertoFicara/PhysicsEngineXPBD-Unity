using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XPBD_Engine.Scripts.Physics.Grabber.Interfaces;

namespace XPBD_Engine.Scripts.Physics.Grabber
{
    public class GrabberPointerController : MonoBehaviour
    {
        public Texture2D cursorTexture;
        private GrabberPointer _grabber;

        // Start is called before the first frame update
        void Start()
        {
            _grabber = new GrabberPointer(Camera.main);
            Cursor.visible = true;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        }

        // Update is called once per frame
        void Update()
        {
            _grabber.MoveGrab(Input.mousePosition);
        }
        private void LateUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (PhysicsEngine.instance == null)
                {
                    Debug.LogError("There is no PhysicalWorld instance in the scene.");
                    return;
                }
                var temp = new List<IGrabbable>(PhysicsEngine.instance.GetSoftBodies().ToList());
        
                _grabber.StartGrab(temp);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _grabber.EndGrab(Input.mousePosition);
            }
        }
    }
}
