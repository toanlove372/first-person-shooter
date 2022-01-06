using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class Health : MonoBehaviour
    {
        public int maxHealth;
        private int currentHealth;

        public int teamIndex;

        public int CurrentHealth => this.currentHealth;

        public event System.Action<int> onDamaged;
        public event System.Action onDestroyed;

        private void Awake()
        {
            this.currentHealth = this.maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (this.currentHealth == 0)
            {
                return;
            }

            this.currentHealth -= damage;
            this.currentHealth = Mathf.Max(0, this.currentHealth);

            this.onDamaged?.Invoke(damage);

            if (this.currentHealth == 0)
            {
                this.onDestroyed?.Invoke();
            }
        }
    }
}