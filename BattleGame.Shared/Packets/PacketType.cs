namespace BattleGame.Shared.Packets
{
    public enum PacketTypes
    {
        Login = 1,
        LoginResult = 2,

        MatchRequest = 3,
        MatchFound = 4,

        SelectionCharacter = 5,
        Move = 6,
        Attack = 7,
        GameState = 8,
        HealthUpdate = 9,
        GameOver = 10,
        Disconnect = 11,

        Register = 12,  // Client → Server: đăng ký tài khoản
        OtpSend = 13,  // Server → Client: báo đã gửi OTP
        OtpVerify = 14,  // Client → Server: gửi mã xác minh
        ForgotPassword = 15,  // Client → Server: yêu cầu reset mật khẩu
    }
}
