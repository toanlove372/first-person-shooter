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

        private Transform target;
        private float lastTimeFire;


        // Start is called before the first frame update
        void Start()
        {
            this.health.onDamaged += HealthOnDamaged;
            this.health.onDestroyed += HealthOnDestroyed;
        }

        private void HealthOnDestroyed()
        {
            this.gameObject.SetActive(false);
        }

        private void HealthOnDamaged(int damage)
        {
            //Debug.Log("Enemy take damage, health remain: " + this.health.CurrentHealth);
        }

        // Update is called once per frame
        void Update()
        {
            if (this.target != null)
            {
                this.transform.LookAt(this.target);
                this.ShootAtTarget(this.target.position);
            }
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

            //this.SetFireAnim();
        }    

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger enter: " + other.gameObject.name, other.gameObject);
            this.target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Trigger exit: " + other.gameObject.name, other.gameObject);
            this.target = null;
        }
    }
}