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
            Paused,
            End,
        }

        public int countdownTime;
        public GameUI gameUI;
        public FPSCharacterController player;
        public List<Enemy> enemies;
        public Transform startPosition;
        public TriggerEnter winGameZone;

        public float disableEnemyTime;

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
            switch (this.state)
            {
                case State.Ready:
                    break;
                case State.Playing:
                    this.player.OnUpdate();
                    for (var i = 0; i < this.enemies.Count; i++)
                    {
                        this.enemies[i].OnUpdate();
                    }

                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        this.PauseGame(true);
                    }
                    break;
                case State.Paused:
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {
                        this.PauseGame(false);
                    }
                    break;
                case State.End:
                    break;
            }
        }

        private void Init()
        {
            this.player.onDie += this.LoseGame;
            this.player.onTakeDamage += (damage) =>
            {
                this.gameUI.SetPlayerHealth(this.player.health.CurrentHealth);
                this.gameUI.ShowDamage(damage);
            };

            for (var i = 0; i < this.enemies.Count; i++)
            {
                this.enemies[i].onDie += this.OnEnemyDie;
            }

            this.gameUI.onReplayClicked += this.Replay;
            this.gameUI.onResumeClicked += () =>
            {
                this.PauseGame(false);
            };

            this.winGameZone.onEnter += (enteringObject) =>
            {
                if (enteringObject == this.player.gameObject)
                {
                    this.WinGame();
                }
            };

            InputManager.LockMouse(true);
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
                if (this.state != State.Playing)
                {
                    yield return null;
                    continue;
                }
                this.currentTime--;
                this.gameUI.SetCountdown(this.currentTime);
                
                yield return waitOneSec;
            }

            this.LoseGame();
        }

        private void LoseGame()
        {
            this.state = State.End;
            this.gameUI.ShowDefeat();
            InputManager.LockMouse(false);
            SoundManager.PlayBackgroundMusic(SoundManager.Sound.GameLose);
            SoundManager.Play(SoundManager.Sound.GameLose);
        }

        private void WinGame()
        {
            this.state = State.End;
            this.gameUI.ShowVictory();
            InputManager.LockMouse(false);
            SoundManager.PlayBackgroundMusic(SoundManager.Sound.GameWin);
            SoundManager.Play(SoundManager.Sound.GameWin);
        }

        private void PauseGame(bool isPaused)
        {
            InputManager.LockMouse(!isPaused);
            this.state = isPaused ? State.Paused : State.Playing;
            this.gameUI.SetPaused(isPaused);
        }

        private void Replay()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        private void OnEnemyDie(Enemy enemy)
        {
            this.score += 100;
            this.gameUI.SetScore(this.score);
            enemy.gameObject.SetActive(false);
            StartCoroutine(IeReactiveEnemy());
            IEnumerator IeReactiveEnemy()
            {
                yield return new WaitForSeconds(this.disableEnemyTime);
                enemy.Init();
                enemy.gameObject.SetActive(true);
            }
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