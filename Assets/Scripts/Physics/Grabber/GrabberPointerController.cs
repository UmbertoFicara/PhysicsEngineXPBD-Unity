using System.Collections.Generic;
using System.Linq;
using Physics;
using Physics.Grabber.Interfaces;
using UnityEngine;

namespace Grabber
{
    public class GrabberPointerController : MonoBehaviour
    {
        public Texture2D cursorTexture;
        public PhysicalWorld physicalWorld;
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
                var temp = new List<IGrabbable>(physicalWorld.GetSoftBodies().ToList());
        
                _grabber.StartGrab(temp);
            }

            if (Input.GetMouseButtonUp(0))
            {
                _grabber.EndGrab(Input.mousePosition);
            }
        }
    }
}
