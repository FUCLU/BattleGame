using System;
using System.Windows.Forms;
using BattleGame.Client.Managers;
using BattleGame.Client.Game;
using BattleGame.Client.Game.Core.Components;

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
            UpdateUIBars();
            this.Invalidate();
        }

        private void UpdateUIBars()
        {
            try
            {
                // Update player HP and Mana
                var playerChar = _engine.Player.Get<CharacterComponent>();
                if (playerChar != null)
                {
                    lblHP.Text = $"{playerChar.Hp}/{playerChar.BaseStats.Hp}";
                    lblMana.Text = $"{playerChar.Mana}/{playerChar.BaseStats.Mana}";

                    // Calculate bar widths
                    int maxHPWidth = 301;
                    int maxManaWidth = 301;

                    if (playerChar.BaseStats.Hp > 0)
                    {
                        int hpWidth = (int)(maxHPWidth * playerChar.Hp / (float)playerChar.BaseStats.Hp);
                        panelHPFill.Width = Math.Max(0, hpWidth);
                    }
                    else
                    {
                        panelHPFill.Width = 0;
                    }

                    if (playerChar.BaseStats.Mana > 0)
                    {
                        int manaWidth = (int)(maxManaWidth * playerChar.Mana / (float)playerChar.BaseStats.Mana);
                        panelManaFill.Width = Math.Max(0, manaWidth);
                    }
                    else
                    {
                        panelManaFill.Width = 0;
                    }
                }

                // Update enemy HP and Mana
                var enemyChar = _engine.Enemy.Get<CharacterComponent>();
                if (enemyChar != null)
                {
                    label6.Text = $"{enemyChar.Hp}/{enemyChar.BaseStats.Hp}";
                    label5.Text = $"{enemyChar.Mana}/{enemyChar.BaseStats.Mana}";

                    // Calculate bar widths for enemy
                    int maxHPWidth = 301;
                    int maxManaWidth = 301;

                    // panel4 = HP (màu đỏ Firebrick)
                    if (enemyChar.BaseStats.Hp > 0)
                    {
                        int hpWidth = (int)(maxHPWidth * enemyChar.Hp / (float)enemyChar.BaseStats.Hp);
                        panel4.Width = Math.Max(0, hpWidth);
                    }
                    else
                    {
                        panel4.Width = 0;
                    }

                    // panel2 = Mana (màu xanh MidnightBlue)
                    if (enemyChar.BaseStats.Mana > 0)
                    {
                        int manaWidth = (int)(maxManaWidth * enemyChar.Mana / (float)enemyChar.BaseStats.Mana);
                        panel2.Width = Math.Max(0, manaWidth);
                    }
                    else
                    {
                        panel2.Width = 0;
                    }
                }
            }
            catch
            {
                // Silently handle any errors during UI update
            }
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