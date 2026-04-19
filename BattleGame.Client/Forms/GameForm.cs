using System;
using System.Windows.Forms;
using BattleGame.Client.Managers;
using BattleGame.Client.Game;

namespace BattleGame.Client.Forms
{
    public partial class GameForm : Form
    {
        private readonly GameEngine _engine;
        private System.Windows.Forms.Timer gameTimer;

        public GameForm(string characterId)
        {
            try
            {
                InitializeComponent();


                this.AutoScaleMode = AutoScaleMode.None;
                this.Font = new Font(this.Font.FontFamily, this.Font.Size);
                gameTimer = new System.Windows.Forms.Timer();
                gameTimer.Interval = 16;
                gameTimer.Tick += gameTimer_Tick;

                this.DoubleBuffered = true;
                this.KeyPreview = true;

                InputManager.Clear();

                _engine = new GameEngine(characterId);
                gameTimer.Start();

                // Đảm bảo form hiển thị
                this.Visible = true;
                this.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khởi tạo GameForm:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            InputManager.SetKey(e.KeyCode, true);
            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            InputManager.SetKey(e.KeyCode, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _engine.Draw(e.Graphics);
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            _engine.Update();
            this.Invalidate();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            InputManager.Clear();
            gameTimer.Stop();
            gameTimer.Dispose();
            base.OnFormClosed(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);
            InputManager.Clear();
        }
    }
}