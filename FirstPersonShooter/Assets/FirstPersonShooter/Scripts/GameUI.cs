using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FirstPersonShooter
{
    public class GameUI : MonoBehaviour
    {
        public Text playerHealthText;
        public Text scoreText;
        public Text countdownText;
        public Text messageText;
        public Image damageEffect;
        public GameObject victoryPanel;
        public GameObject defeatPanel;

        private bool showDamage;
        private float damageEffectAlpha;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (this.showDamage)
            {
                if (this.damageEffectAlpha > 0f)
                {
                    this.damageEffectAlpha -= 0.2f * Time.deltaTime;
                }
                this.damageEffect.color = new Color(1, 0f, 0f, this.damageEffectAlpha);
                if (this.damageEffectAlpha < 0f)
                {
                    this.showDamage = false;
                }
            }
        }

        public void ShowWelcome(int seconds)
        {
            var time = System.TimeSpan.FromSeconds(seconds);
            this.messageText.text = $"Reach the helicopter in {seconds}";
            this.messageText.gameObject.SetActive(true);
        }

        public void HideWelcome()
        {
            this.messageText.gameObject.SetActive(false);
        }

        public void SetPlayerHealth(int health)
        {
            this.playerHealthText.text = $"HEALTH: {health}";
        }

        public void SetCountdown(int seconds)
        {
            var time = System.TimeSpan.FromSeconds(seconds);
            this.countdownText.text = time.ToString(@"hh\:mm\:ss");
        }

        public void SetScore(int score)
        {
            this.scoreText.text = $"SCORE: {score}";
        }

        public void ShowDamage(int damage)
        {
            this.showDamage = true;
            this.damageEffectAlpha = 1f;
        }

        public void ShowVictory()
        {
            this.victoryPanel.SetActive(true);
        }

        public void ShowDefeat()
        {
            this.defeatPanel.SetActive(true);
        }
    }
}