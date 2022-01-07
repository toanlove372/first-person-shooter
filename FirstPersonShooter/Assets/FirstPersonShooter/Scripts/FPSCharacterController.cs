using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FirstPersonShooter
{
    public class FPSCharacterController : MonoBehaviour
    {
        private enum State
        {
            Waiting,
            Playing,
            Death,
        }

        [Header("Movement")]
        public Transform cameraTransform;
        public CharacterController controller;
        public float jumpForce;
        public float gravity;
        public float moveSpeed;
        public float rotateSpeed;

        [Header("Shooting")]
        public float fireCooldown;
        public Transform bulletStartPos;
        public Bullet bulletPrefab;

        [Header("Health")]
        public Health health;

        private float rotation;
        private bool jump;
        private float ySpeed;
        private bool prevGrounded;

        private float lastTimeFire;

        private State state;

        private List<Bullet> bullets = new List<Bullet>();

        public event Action<int> onTakeDamage;
        public event Action onDie;

        // Start is called before the first frame update
        void Start()
        {
            this.health.onDamaged += HealthOnDamage;
            this.health.onDestroyed += HealthOnDestroyed;
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            switch (this.state)
            {
                case State.Waiting:
                    break;
                case State.Playing:
                    this.UpdatePlaying();
                    break;
                case State.Death:
                    break;
            }
        }

        public void Init()
        {
            this.health.Init();
        }

        private void HealthOnDestroyed()
        {
            this.Die();
        }

        private void HealthOnDamage(int damage)
        {
            this.onTakeDamage?.Invoke(damage);

            //Debug.Log("Player take damage");
        }

        #region Playing

        public void Play()
        {
            this.state = State.Playing;
        }

        private void UpdatePlaying()
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

            for (var i = 0; i < this.bullets.Count; i++)
            {
                this.bullets[i].OnUpdate();
            }
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
            if (grounded == false)
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
            if (Time.time - this.lastTimeFire < this.fireCooldown)
            {
                return;
            }
            this.lastTimeFire = Time.time;

            Bullet bullet = Instantiate(this.bulletPrefab, this.bulletStartPos.position, this.cameraTransform.rotation);
            bullet.Init(this.cameraTransform.forward);
            bullet.onDestroyed += (destroyedBullet) =>
            {
                this.bullets.Remove(bullet);
            };
            this.bullets.Add(bullet);

            this.SetFireAnim();
        }

        #endregion

        #region Death

        private void Die()
        {
            this.state = State.Death;
            this.onDie?.Invoke();
        }

        #endregion

        #region Animation

        private readonly int MoveAnimHash = Animator.StringToHash("IsMoving");
        private readonly int FireAnimHash = Animator.StringToHash("Fire");

        [Header("Animation")]
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

        private void OnDrawGizmosSelected()
        {
            if (this.cameraTransform == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.bulletStartPos.position, this.bulletStartPos.position + this.cameraTransform.forward * 100f);
        }
    }
}
