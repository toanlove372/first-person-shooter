using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{

    public class Enemy : MonoBehaviour
    {
        [Header("Shooting")]
        public float fireCooldown;
        public Transform bulletStartPos;
        public Bullet bulletPrefab;

        [Header("Health")]
        public Health health;
        public SpriteRenderer healthBar;

        [Header("Animation")]
        public Animator animator;

        private Transform target;
        private float lastTimeFire;
        private List<Bullet> bullets = new List<Bullet>();

        public event System.Action<Enemy> onDie;

        // Start is called before the first frame update
        void Start()
        {
            this.health.onDamaged += HealthOnDamaged;
            this.health.onDestroyed += HealthOnDestroyed;
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            if (this.gameObject.activeInHierarchy && this.target != null)
            {
                this.transform.LookAt(new Vector3(this.target.position.x, this.transform.position.y, this.target.position.z));
                this.ShootAtTarget(this.target.position);
            }

            for (var i = 0; i < this.bullets.Count; i++)
            {
                this.bullets[i].OnUpdate();
            }
        }

        public void Init()
        {
            this.health.Init();
            this.SetHealthBar();
        }

        private void HealthOnDestroyed()
        {
            this.onDie?.Invoke(this);
        }

        private void HealthOnDamaged(int damage)
        {
            this.SetHealthBar();
        }

        private void SetHealthBar()
        {
            var percent = (float)this.health.CurrentHealth / this.health.maxHealth;
            this.healthBar.size = new Vector2(percent * 1f, 0.2f);
        }

        private void ShootAtTarget(Vector3 targetPos)
        {
            if (Time.time - this.lastTimeFire < this.fireCooldown)
            {
                return;
            }
            this.lastTimeFire = Time.time;

            var direction = (targetPos - this.bulletStartPos.position).normalized;
            Bullet bullet = Instantiate(this.bulletPrefab, this.bulletStartPos.position, Quaternion.identity);
            bullet.Init(direction);
            bullet.onDestroyed += (destroyedBullet) =>
            {
                this.bullets.Remove(destroyedBullet);
            };
            this.bullets.Add(bullet);

            this.SetFireAnim();
        }    

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Trigger enter: " + other.gameObject.name, other.gameObject);
            this.target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            //Debug.Log("Trigger exit: " + other.gameObject.name, other.gameObject);
            this.target = null;
        }

        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private void SetFireAnim()
        {
            this.animator.SetTrigger(AttackHash);
        }
    }
}