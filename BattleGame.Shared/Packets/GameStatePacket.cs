using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public class GameStatePacket : Packet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public bool FacingRight { get; set; }
        public bool IsGrounded { get; set; }
        public int Hp { get; set; }
        public int Mana { get; set; }
        public int EnemyHp { get; set; }
        public int EnemyMana { get; set; }
        public bool IsProtecting { get; set; }
        public bool IsAttacking { get; set; }
        public bool IsUsingSkill { get; set; }
        public bool IsHurt { get; set; }
        public bool IsDead { get; set; }
        public string CurrentAnimation { get; set; } = "Idle";
        public int CurrentFrame { get; set; }

        public GameStatePacket() : base(PacketType.GameState)
        {
        }
    }
}
