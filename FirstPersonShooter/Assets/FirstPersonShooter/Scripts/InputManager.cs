using System.Collections;
using System.Collections.Generic;
using TouchControlsKit;
using UnityEngine;

namespace FirstPersonShooter
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private bool useTouch;

        [Header("Touch constrol settings")]
        [SerializeField] private GameObject touchCanvas;

        [Header("Mouse and keys settings")]
        [SerializeField] private float sensitivityX;
        [SerializeField] private float sensitivityY;

        private bool jump;
        private bool fire;
        private Vector2 move;
        private Vector2 rotate;

        public static bool Jump => instance.jump;
        public static bool Fire => instance.fire;
        public static Vector2 Rotate => instance.rotate;
        public static Vector2 Move => instance.move;

        static InputManager instance;
        static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<InputManager>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            this.touchCanvas.SetActive(this.useTouch);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (this.useTouch)
            {
                this.UpdateTouch();
            }
            else
            {
                this.UpdateMouseAndKeyboards();
            }
        }

        private void UpdateTouch()
        {
            this.jump = TCKInput.GetAction("jumpBtn", EActionEvent.Press);

            this.fire = TCKInput.GetAction("fireBtn", EActionEvent.Press);

            this.rotate = TCKInput.GetAxis("Touchpad");

            this.move = TCKInput.GetAxis("Joystick");
        }

        private void UpdateMouseAndKeyboards()
        {
            this.jump = Input.GetKey(KeyCode.Space);
            this.fire = Input.GetMouseButton(0);
            this.move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
            float rotationY = transform.localEulerAngles.x + Input.GetAxis("Mouse Y") * sensitivityY;//Mathf.Clamp(rotationY, minimumY, maximumY);
            this.rotate = new Vector2(rotationX, rotationY);

            if (Input.GetKey(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}