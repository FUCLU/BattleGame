using System;
using System.Collections.Generic;
using System.Text;

namespace BattleGame.Shared.Packets
{
    public enum PacketTypes
    {
        Login,
        LoginResult,

        Register,
        RegisterResult,

        SelectCharacter,

        MatchRequest,
        MatchFound,

        Move,
        Attack,

        GameState,

        Disconnect,
    }
}
