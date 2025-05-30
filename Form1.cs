using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game1
{
    public partial class Form1 : Form
    {
        // Состояния игры
        private bool _isDragging;
        private bool _isGameOver;
        private int _coinsCollected;
        private Point _dragStartPosition;

        // Это настройки игры, здесь я числа заменила на константы
        private const int BackgroundSpeed = 10;
        private const int CarSpeed = 7;
        private const int PlayerSpeed = 10;
        private readonly Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }


        private void InitializeGame()
        {
            bg1.MouseDown += MouseClickDown;
            bg1.MouseUp += MouseClickUp;
            bg1.MouseMove += MouseClickMove;

            bg2.MouseDown += MouseClickDown;
            bg2.MouseUp += MouseClickUp;
            bg2.MouseMove += MouseClickMove;

           

            ResetGameState();
            KeyPreview = true;
        }

        private void ResetGameState()
        {
            _isGameOver = false;
            _coinsCollected = 0;

            // сброс элементов интерфейса при перезапуске
            labelLose.Visible = false;
            btnRestart.Visible = false;
            labelCoins.Text = "Монеты: 0";

            // вражеские машинки и монетки возвращаюся на первоначальное место(за экраном)
            enemy1.Top = -130;
            enemy1.Left = _random.Next(150, 300);

            enemy2.Top = -400;
            enemy2.Left = _random.Next(300, 500);

            coin.Top = -500;
            coin.Left = _random.Next(150, 500);

           
            timer.Start(); // заменила Enabled = true на Start(), для лучшей читаемости
        }

        // добавила регионы для логического разделения кода
        #region Mouse Drag Handling
        private void MouseClickDown(object sender, MouseEventArgs e)
        {
            _isDragging = true;
            _dragStartPosition = e.Location;
        }

        private void MouseClickUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void MouseClickMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentPoint = PointToScreen(e.Location);
                Location = new Point(
                    currentPoint.X - _dragStartPosition.X,
                    currentPoint.Y - _dragStartPosition.Y + bg1.Top);
            }
        }
        #endregion

        #region Keyboard Handling
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_isGameOver) return;

            if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && player.Left > 150)
                player.Left -= PlayerSpeed;
            else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && player.Right < 600)
                player.Left += PlayerSpeed;
        }
        #endregion

        #region Game Logic
        private void timer_Tick(object sender, EventArgs e)
        {
            if (_isGameOver) return;

            MoveBackground();
            MoveGameObjects();
            CheckCollisions();
        }

        private void MoveBackground()
        {
            bg1.Top += BackgroundSpeed;
            bg2.Top += BackgroundSpeed;

            if (bg1.Top >= 500)
            {
                bg1.Top = 0;
                bg2.Top = -500;
            }
        }

        private void MoveGameObjects()
        {
            // движение вражеских машинок
            enemy1.Top += CarSpeed;
            enemy2.Top += CarSpeed;

            // движение монетки
            coin.Top += BackgroundSpeed;

            // сбрасывание позиции, когла элемент выходи за экран
            if (enemy1.Top >= 650) ResetEnemy(enemy1, minX: 150, maxX: 300, y: -130);
            if (enemy2.Top >= 650) ResetEnemy(enemy2, minX: 300, maxX: 500, y: -400);
            if (coin.Top >= 650) ResetCoin();
        }

        private void ResetEnemy(PictureBox enemy, int minX, int maxX, int y)
        {
            enemy.Top = y;
            enemy.Left = _random.Next(minX, maxX);
        }

        private void ResetCoin()
        {
            coin.Top = -50;
            coin.Left = _random.Next(150, 500);
        }

        private void CheckCollisions()
        {
            if (player.Bounds.IntersectsWith(enemy1.Bounds) ||
                player.Bounds.IntersectsWith(enemy2.Bounds))
            {
                GameOver();
            }

            if (player.Bounds.IntersectsWith(coin.Bounds))
            {
                CollectCoin();
            }
        }

        private void GameOver()
        {
            _isGameOver = true;
            timer.Stop(); // замениал Enabled = false на Stop, для более удобной читаемости
            labelLose.Visible = true;
            btnRestart.Visible = true;
        }

        private void CollectCoin()
        {
            _coinsCollected++;
            labelCoins.Text = $"Монеты: {_coinsCollected}"; // тут теперь f-сторка стала
            ResetCoin();
        }
        #endregion

        private void btnRestart_Click(object sender, EventArgs e)
        {
            ResetGameState();
        }
    }
}
