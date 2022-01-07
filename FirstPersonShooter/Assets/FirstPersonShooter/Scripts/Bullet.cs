using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class Bullet : MonoBehaviour
    {
        public float speed;
        public int damage;
        public Rigidbody rigid;
        public int teamIndex;
        public float maxLiveTime;

        public GameObject explosionPrefab;

        public float affectByPlayerMove;

        private bool isInitialized;
        private float liveTime;
        private Vector3 moveDirection;

        public event System.Action<Bullet> onDestroyed;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void OnUpdate()
        {
            if (this.isInitialized == false)
            {
                return;
            }

            this.transform.position += this.moveDirection * this.speed * Time.fixedDeltaTime;

            this.liveTime += Time.fixedDeltaTime;
            if (this.liveTime > this.maxLiveTime)
            {
                this.SelfDestroy();
            }
        }

        public void Init(Vector3 lookDirection)
        {
            this.isInitialized = true;
            this.transform.forward = lookDirection;
            this.moveDirection = lookDirection;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Damageable"))
            {
                var health = collision.gameObject.GetComponent<Health>();
                if (health != null && health.teamIndex != this.teamIndex)
                {
                    //Debug.Log("Damage: " + collision.gameObject.name, collision.transform);
                    health.TakeDamage(this.damage);
                }
            }

            if (this.explosionPrefab != null)
            {
                var explosion = Instantiate(this.explosionPrefab, this.transform.position, Quaternion.identity);
                explosion.SetActive(true);
            }

            this.SelfDestroy();
        }

        private void SelfDestroy()
        {
            this.onDestroyed?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}