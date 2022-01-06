using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstPersonShooter
{
    public class GameplayManager : MonoBehaviour
    {
        private enum State
        {
            Ready,
            Playing,
            End,
        }

        public int countdownTime;
        public GameUI gameUI;
        public FPSCharacterController player;
        public Transform startPosition;

        private State state;
        private int score;
        private int currentTime;

        private Coroutine countdownCor;

        // Start is called before the first frame update
        void Start()
        {
            this.Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Init()
        {
            this.player.onDie += this.LoseGame;
            this.player.onTakeDamage += (damage) =>
            {
                this.gameUI.ShowDamage(damage);
            };

            this.Ready();
        }

        private void Ready()
        {
            this.state = State.Ready;
            this.score = 0;
            this.currentTime = countdownTime;
            this.player.Init();
            this.player.transform.position = this.startPosition.position;
            this.gameUI.SetPlayerHealth(this.player.health.CurrentHealth);
            this.gameUI.SetScore(this.score);
            this.gameUI.ShowWelcome(this.countdownTime);

            this.Delay(3f, this.StartPlaying);
        }

        private void StartPlaying()
        {
            this.state = State.Playing;
            this.player.Play();
            this.gameUI.HideWelcome();
            this.countdownCor = StartCoroutine(this.IeCountdown());
        }

        private IEnumerator IeCountdown()
        {
            var waitOneSec = new WaitForSeconds(1f);
            while (this.currentTime > 0)
            {
                this.gameUI.SetCountdown(this.currentTime);
                this.currentTime--;
                yield return waitOneSec;
            }

            this.LoseGame();
        }

        private void LoseGame()
        {
            this.state = State.End;
            this.gameUI.ShowDefeat();
        }

        private void WinGame()
        {
            this.state = State.End;
            this.gameUI.ShowVictory();
        }

        private void Delay(float time, System.Action onDone)
        {
            StartCoroutine(IeDelay());
            IEnumerator IeDelay()
            {
                yield return new WaitForSeconds(time);
                onDone();
            }
        }
    }
}