using System.Drawing;
using BattleGame.Client.Game.Characters;

namespace BattleGame.Client.Game
{
    internal class CharacterRenderer
    {
        public void Draw(Graphics g, Soldier character)
        {
            character.Draw(g); 
        }
    }
}