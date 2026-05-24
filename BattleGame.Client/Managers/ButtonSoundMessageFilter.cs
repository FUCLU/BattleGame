using System;
using System.Windows.Forms;

namespace BattleGame.Client.Managers
{
    internal sealed class ButtonSoundMessageFilter : IMessageFilter
    {
        private const int WmLButtonUp = 0x0202;
        private const int WmKeyUp = 0x0101;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WmLButtonUp)
            {
                if (Control.FromHandle(m.HWnd) is ButtonBase)
                    SoundManager.PlayButtonClick();
            }
            else if (m.Msg == WmKeyUp && (Keys)m.WParam == Keys.Space)
            {
                if (Control.FromHandle(m.HWnd) is ButtonBase)
                    SoundManager.PlayButtonClick();
            }

            return false;
        }
    }
}
