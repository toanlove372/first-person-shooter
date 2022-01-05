using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class Bullet : MonoBehaviour
    {
        public float speed;
        public float damage;
        public Rigidbody rigid;
        public LayerMask hitLayers;
        public float maxLiveTime;

        public float affectByPlayerMove;

        private bool isInitialized;
        private float liveTime;
        private Vector3 moveDirection;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (this.isInitialized == false)
            {
                return;
            }

            this.rigid.velocity = this.moveDirection * this.speed * Time.fixedDeltaTime;

            this.liveTime += Time.fixedDeltaTime;
            if (this.liveTime > this.maxLiveTime)
            {
                this.SelfDestroy();
            }
        }

        public void Init(Vector3 lookDirection, Vector3 playerMoveDirection)
        {
            this.isInitialized = true;
            this.transform.forward = lookDirection;
            playerMoveDirection.y = 0f;
            this.moveDirection = lookDirection;// (lookDirection * this.speed * Time.fixedDeltaTime + playerMoveDirection.normalized * this.affectByPlayerMove).normalized;
        }

        

        private void SelfDestroy()
        {
            Destroy(this.gameObject);
        }
    }
}