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
        Disconnect = 16,
        CreateRoom = 17,
        CreateRoomResult = 18,
        GetRoom = 19,
        GetRoomResult = 20,
        JoinRoom = 21,
        JoinRoomResult = 22,
        Ready = 23,
        SelectMap = 24,
        GetLeaderboard = 25,
        GetLeaderboardResult = 26,
        RemoveRoom = 27,
        RemoveRoomResult = 28,
        LeaveRoom = 29
    }
}
