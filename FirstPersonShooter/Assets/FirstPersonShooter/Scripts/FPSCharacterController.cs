using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class FPSCharacterController : MonoBehaviour
    {
        Transform cameraTransform;
        public CharacterController controller;
        public float jumpForce;
        public float gravity;
        public float moveSpeed;
        public float rotateSpeed;

        private float rotation;
        private bool jump;
        private float ySpeed;
        private bool prevGrounded;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (InputManager.Jump)
            {
                Jumping();
            }

            if (InputManager.Fire)
            {
                PlayerFiring();
            }

            Vector2 look = InputManager.Rotate;
            PlayerRotation(look.x, look.y);

            Vector2 move = InputManager.Move;
            PlayerMovement(move.x, move.y);
        }

        private void Jumping()
        {
            if (controller.isGrounded)
            {
                jump = true;
            }
        }

        private void PlayerMovement(float horizontal, float vertical)
        {
            bool grounded = controller.isGrounded;

            Vector3 moveDirection = this.transform.forward * vertical;
            moveDirection += this.transform.right * horizontal;

            moveDirection.y += this.ySpeed;

            if (jump)
            {
                jump = false;
                this.ySpeed = this.jumpForce;
            }

            moveDirection *= this.moveSpeed;
            if (grounded)
            {
                
            }
            else
            {
                this.ySpeed -= this.gravity;
            }

            controller.Move(moveDirection * Time.fixedDeltaTime);

            if (!prevGrounded && grounded)
                moveDirection.y = 0f;

            prevGrounded = grounded;

            if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f))
            {
                this.SetMoveAnim(false);
            }
            else
            {
                this.SetMoveAnim(true);
            }
        }

        public void PlayerRotation(float horizontal, float vertical)
        {
            this.transform.Rotate(0f, horizontal * this.rotateSpeed, 0f);
            rotation += vertical * 12f;
            rotation = Mathf.Clamp(rotation, -60f, 60f);
            cameraTransform.localEulerAngles = new Vector3(-rotation, cameraTransform.localEulerAngles.y, 0f);
        }

        public void PlayerFiring()
        {
            Debug.Log("Fire");
            this.SetFireAnim();
        }

        #region Animation

        private readonly int MoveAnimHash = Animator.StringToHash("IsMoving");
        private readonly int FireAnimHash = Animator.StringToHash("Fire");

        public Animator animator;

        private void SetMoveAnim(bool isMoving)
        {
            this.animator.SetBool(MoveAnimHash, isMoving);
        }

        private void SetFireAnim()
        {
            this.animator.SetTrigger(FireAnimHash);
        }

        #endregion
    }
}