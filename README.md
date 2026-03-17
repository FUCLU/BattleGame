# NT106.Q23.ANTT — LẬP TRÌNH MẠNG CĂN BẢN
---

## I. Đồ Án Môn Học
**Tên đề tài:**  Thiết kế và xây dựng game đối kháng 2D bằng C# trên nền tảng .NET Windows Forms 
---

## II. Danh Sách Thành Viên

| STT | Họ và tên | MSSV |
|---|---|---|
| 1 | Lưu Hồng Phúc | 24521382 |
| 2 | Phan Thái Hưng | 24520624 |
| 3 | Nguyễn Tấn Phát | 24521306 |
| 4 | Nguyễn Phan Hoàng Long | 24521006 |

---

## III. Thông Tin Đồ Án

### Mô tả tổng quan

BattleGame là game đối kháng 2D theo lượt chơi qua mạng cục bộ (LAN), được xây dựng bằng C# và Windows Forms. Hai người chơi kết nối tới server trung tâm, chọn nhân vật, được ghép cặp tự động và thi đấu theo thời gian thực thông qua giao tiếp TCP Socket.

### Các tính năng chính

- Đăng nhập / xác thực tài khoản người chơi
- Chọn nhân vật với các chỉ số và kỹ năng riêng biệt
- Hệ thống matchmaking tự động ghép 2 người chơi vào cùng một phòng
- Trận đấu real-time: di chuyển, tấn công, sử dụng kỹ năng
- Đồng bộ trạng thái game liên tục giữa server và các client
- Hiển thị animation nhân vật và hiệu ứng chiến đấu
- Hệ thống âm thanh và quản lý tài nguyên (assets)

### Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# (.NET 10) |
| Giao diện | Windows Forms |
| Giao tiếp mạng | TCP Socket (`System.Net.Sockets`) |
| Serialization | Custom `PacketSerializer` |
| Cơ sở dữ liệu | SQLite (qua `UserRepository`) |
| Kiểm thử | Dự án `BattleGame.Test` độc lập |

---

## IV. Cấu Trúc Dự Án

```
BattleGameSolution/
├── BattleGame.Client/          # Ứng dụng client (WinForms)
│   ├── Forms/                  # Các màn hình giao diện
│   │   ├── LoginForm           # Màn hình đăng nhập
│   │   ├── MenuForm            # Màn hình chính
│   │   ├── CharacterSelection  # Chọn nhân vật
│   │   └── GameForm            # Màn hình trận đấu
│   ├── Game/                   # Logic hiển thị và điều khiển game
│   │   ├── GameEngine          # Vòng lặp game chính
│   │   ├── GameStateManager    # Quản lý trạng thái game
│   │   ├── AnimationManager    # Quản lý animation
│   │   └── CharacterRenderer   # Render nhân vật
│   ├── Managers/               # Các manager hỗ trợ
│   │   ├── InputManager        # Xử lý input bàn phím
│   │   ├── SoundManager        # Quản lý âm thanh
│   │   └── AssetManager        # Tải và cache tài nguyên
│   ├── Network/                # Giao tiếp mạng phía client
│   │   ├── ClientSocket        # Kết nối TCP tới server
│   │   └── MatchmakingClient   # Gửi yêu cầu tìm trận
│   └── Assets/                 # Tài nguyên game
│       ├── Background/
│       ├── Characters/
│       └── UI/
│
├── BattleGame.Server/          # Ứng dụng server (Console)
│   ├── Network/                # Xử lý kết nối mạng
│   │   ├── GameServer          # Lắng nghe kết nối TCP
│   │   └── ClientHandler       # Xử lý từng client riêng biệt
│   ├── Services/               # Các dịch vụ nghiệp vụ
│   │   ├── AuthService         # Xác thực đăng nhập
│   │   └── MatchmakingService  # Ghép cặp người chơi
│   ├── Game/                   # Logic game phía server
│   │   ├── BattleEngine        # Điều phối trận đấu
│   │   ├── CombatSystem        # Tính toán chiến đấu
│   │   ├── GameManager         # Quản lý các phòng
│   │   ├── GameRoom            # Phòng chứa một trận đấu
│   │   └── Match               # Thông tin một trận
│   └── Database/
│       └── UserRepository      # Truy vấn dữ liệu người dùng
│
├── BattleGame.Shared/          # Thư viện dùng chung
│   ├── Models/                 # Các model dữ liệu
│   │   ├── Player / PlayerState
│   │   ├── Character / CharacterState
│   │   ├── GameState
│   │   └── Skill
│   ├── Packets/                # Các gói tin giao tiếp mạng
│   │   ├── Packet / PacketType
│   │   ├── LoginPacket / LoginResultPacket
│   │   ├── MatchRequestPacket / MatchFoundPacket
│   │   ├── SelectionCharacterPacket
│   │   ├── MovePacket / AttackPacket
│   │   ├── GameStatePacket
│   │   └── DisconnectPacket
│   └── Network/
│       ├── ClientSocket        # Wrapper TCP dùng chung
│       └── PacketSerializer    # Serialize / deserialize packet
│
└── BattleGame.Test/            # Dự án kiểm thử
```

### Sơ đồ kiến trúc tổng quan

```
┌─────────────────────────────────────────────────────────┐
│                   BattleGame.Client                     │
│  LoginForm ─► CharacterSelection ─► GameForm            │
│  GameEngine  │  InputManager  │  AnimationManager       │
│              └──── ClientSocket (TCP) ────┐             │
└───────────────────────────────────────────┼─────────────┘
                                            │ TCP Socket
                   ┌────────────────────────┘
                   ▼
┌──────────────────────────────────────────────────────────┐
│                   BattleGame.Server                      │
│  GameServer ─► ClientHandler ─► MatchmakingService       │
│                                      │                   │
│                                      ▼                   │
│                               GameRoom / Match           │
│                          BattleEngine / CombatSystem     │
│                               UserRepository (DB)        │
└──────────────────────────────────────────────────────────┘

         Cả Client và Server đều tham chiếu:
┌──────────────────────────────────────────┐
│           BattleGame.Shared              │
│   Models │ Packets │ PacketSerializer    │
└──────────────────────────────────────────┘
```

---

## V. Hướng Dẫn Cài Đặt & Chạy

### Yêu cầu hệ thống

- Windows 10/11 (64-bit)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 (hoặc Rider / VS Code với C# extension)

### Clone dự án

```bash
git clone https://github.com/<your-repo>/BattleGameSolution.git
cd BattleGameSolution
```

### Build toàn bộ solution

```bash
dotnet build BattleGameSolution.slnx
```

### Chạy Server

```bash
cd BattleGame.Server
dotnet run
```

> Server mặc định lắng nghe tại `0.0.0.0:5000`. Có thể thay đổi trong `Program.cs`.

### Chạy Client

```bash
cd BattleGame.Client
dotnet run
```

> Mở 2 cửa sổ Client để thử nghiệm matchmaking. Nhập địa chỉ IP của máy chạy server khi đăng nhập.

### Chạy Test

```bash
cd BattleGame.Test
dotnet run
```

---

## VI. Luồng Hoạt Động

```
Client A                    Server                      Client B
   │                           │                            │
   │──── LoginPacket ─────────►│◄──── LoginPacket ───────── │
   │◄─── LoginResultPacket ────│───── LoginResultPacket ───►│
   │                           │                            │
   │──── MatchRequestPacket ──►│◄─── MatchRequestPacket ─── │
   │                    [Ghép cặp thành công]               │
   │◄─── MatchFoundPacket ─────│───── MatchFoundPacket ────►│
   │                           │                            │
   │──── SelectionCharPacket ─►│◄── SelectionCharPacket ─── │
   │                           │                            │
   │═══════════════ Vòng lặp trận đấu ════════════════════  │
   │──── MovePacket ──────────►│                            │
   │                           │───── GameStatePacket ────► │
   │──── AttackPacket ────────►│                            │
   │◄─── GameStatePacket ──────│                            │
   │═══════════════════════════════════════════════════════ │
   │                           │                            │
   │──── DisconnectPacket ────►│◄─── DisconnectPacket ───── │
```

---

## VII. Giao Thức Mạng

Tất cả giao tiếp sử dụng **TCP Socket** với định dạng packet tùy chỉnh, được serialize/deserialize qua `PacketSerializer` trong `BattleGame.Shared`.

| Packet | Hướng | Mô tả |
|---|---|---|
| `LoginPacket` | Client → Server | Gửi thông tin đăng nhập |
| `LoginResultPacket` | Server → Client | Kết quả xác thực |
| `MatchRequestPacket` | Client → Server | Yêu cầu tìm trận |
| `MatchFoundPacket` | Server → Client | Thông báo đã ghép cặp |
| `SelectionCharacterPacket` | Client → Server | Chọn nhân vật |
| `MovePacket` | Client → Server | Di chuyển nhân vật |
| `AttackPacket` | Client → Server | Thực hiện tấn công |
| `GameStatePacket` | Server → Client | Đồng bộ trạng thái game |
| `DisconnectPacket` | Client → Server | Ngắt kết nối |

---
