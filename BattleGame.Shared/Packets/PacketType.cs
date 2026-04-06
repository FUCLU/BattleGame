namespace BattleGame.Shared.Packets
{
    public enum PacketType
    {
        Login = 1,
        LoginResult = 2,
        Register = 3,
        OtpSent = 4,
        OtpVerify = 5,
        ForgotPassword = 6,
        ResetPassword = 7,
        MatchRequest = 8,
        MatchFound = 9,
        SelectCharacter = 10,
        Move = 11,
        Attack = 12,
        GameState = 13,
        HealthUpdate = 14,
        GameOver = 15,
        Disconnect = 16
    }
}
