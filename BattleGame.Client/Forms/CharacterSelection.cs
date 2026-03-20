using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BattleGame.Client.Forms
{
    public partial class CharacterSelection : Form
    {
        Size originalSize;
        Point originalLocation;
        public CharacterSelection()
        {
            InitializeComponent();
        }

        private void CharacterSelection_Load(object sender, EventArgs e)
        {
            originalSize = selectKen.Size;
            originalLocation = selectKen.Location;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


        class Character
        {
            public string Name { get; set; }
            public int Power { get; set; }
            public int Speed { get; set; }
            public string Skill { get; set; }
            public Image Avatar { get; set; }
        }

        Character ken = new Character()
        {
            Name = "Ken",
            Power = 85,
            Speed = 80,
            Skill = "Hadouken, Shoryuken",
            Avatar = Properties.Resources.ken_full
        };

        Character ryu = new Character()
        {
            Name = "Ryu",
            Power = 85,
            Speed = 80,
            Skill = "Hazada, ShortKick",
            Avatar = Properties.Resources.ryu_full
        };
        void ShowCharacter(Character c)
        {
            characterReviewPic.Image = c.Avatar;

            txtInfCharac.Text = $"Tên: {c.Name}\r\n" +
                                $"Sức mạnh: {c.Power}\r\n" +
                                $"Tốc độ: {c.Speed}\r\n" +
                                $"Kỹ năng: {c.Skill}";
        }

        //Nhan vat Ken
        private void selectKen_Click_1(object sender, EventArgs e)
        {
            ShowCharacter(ken);
            selectKen.BorderStyle = BorderStyle.Fixed3D;
        }


        private void selectKen_MouseEnter_1(object sender, EventArgs e)
        {
            // Phóng to 12px và lùi lại 6px để giữ tâm không đổi
            selectKen.Size = new Size(selectKen.Width + 12, selectKen.Height + 12);
            selectKen.Location = new Point(selectKen.Location.X - 6, selectKen.Location.Y - 6);
        }

        private void selectKen_MouseLeave_1(object sender, EventArgs e)
        {
            // Thu nhỏ lại đúng 12px và tiến lên 6px
            selectKen.Size = new Size(selectKen.Width - 12, selectKen.Height - 12);
            selectKen.Location = new Point(selectKen.Location.X + 6, selectKen.Location.Y + 6);
        }
        //Nhan vat Ryu
        private void selectRyu_Click(object sender, EventArgs e)
        {
            ShowCharacter(ryu);
            selectKen.BorderStyle = BorderStyle.Fixed3D;
        }
        private void selectRyu_MouseEnter(object sender, EventArgs e)
        {
            selectRyu.Size = new Size(selectRyu.Width + 12, selectRyu.Size.Height + 12);
            selectRyu.Location = new Point(selectRyu.Location.X - 6, selectRyu.Location.Y - 6);
        }

        private void selectRyu_MouseLeave(object sender, EventArgs e)
        {
            selectRyu.Size = new Size(selectRyu.Width - 12, selectRyu.Height - 12);
            selectRyu.Location = new Point(selectRyu.Location.X + 6, selectRyu.Location.Y + 6);
        }

        private void txtInfCharac_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
