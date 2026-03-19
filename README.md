# NT106.Q23.ANTT — LẬP TRÌNH MẠNG CĂN BẢN

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL_15-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## I. Đồ Án Môn Học
**Tên đề tài:**  Thiết kế và xây dựng game đối kháng 2D bằng C# trên nền tảng .NET Windows Forms 
---

## Danh Sách Thành Viên

| STT | Họ và tên | MSSV | Vai trò |
|---|---|---|---|
| 1 | Lưu Hồng Phúc | 24521382 | Game Architect / Server |
| 2 | Phan Thái Hưng | 24520624 | Gameplay Programmer |
| 3 | Nguyễn Tấn Phát | 24521306 | UI / UX Game Developer |
| 4 | Nguyễn Phan Hoàng Long | 24521006 | Network Programmer |

---

## Mô Tả Tổng Quan

BattleGame là game đối kháng 2D theo thời gian thực qua mạng cục bộ (LAN), được xây dựng bằng C# và Windows Forms. Hai người chơi kết nối tới server trung tâm, chọn nhân vật, được ghép cặp tự động và thi đấu thông qua giao tiếp TCP Socket.

### Các tính năng chính

- Đăng nhập / xác thực tài khoản người chơi
- Chọn nhân vật với các chỉ số và kỹ năng riêng biệt
- Hệ thống matchmaking tự động ghép 2 người chơi vào cùng một phòng
- Trận đấu real-time: di chuyển, tấn công, sử dụng kỹ năng
- Đồng bộ trạng thái game liên tục giữa server và các client (50ms/tick)
- Hiển thị animation nhân vật và hiệu ứng chiến đấu
- Hệ thống âm thanh BGM và SFX

### Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# (.NET 8) |
| Giao diện | Windows Forms |
| Giao tiếp mạng | TCP Socket (`System.Net.Sockets`) |
| Serialization | Custom `PacketSerializer` |
| Cơ sở dữ liệu | PostgreSQL 15 |
| Container | Docker + Docker Compose |
| Kiểm thử | xUnit (`BattleGame.Test`) |

---

## Yêu Cầu Hệ Thống

| Công cụ | Phiên bản |
|---|---|
| Docker Desktop | 4.x trở lên |
| .NET SDK | 8.0 (chỉ cần nếu chạy Client hoặc build tay) |
| Visual Studio | 2022 (để phát triển) |
| Windows | 10/11 64-bit (để chạy Client WinForms) |

---

## Hướng Dẫn Cài Đặt & Chạy

### 1. Clone dự án

```bash
git clone https://github.com/FUCLU/BattleGameSolution.git
cd BattleGameSolution
```

### 2. Tạo file .env

> File `.env` chứa mật khẩu DB và không được commit lên git.
> Phải tạo thủ công sau khi clone.

```bash
# Copy template
cp .env.example .env
```

Sau đó mở file `.env` và điền `POSTGRES_PASSWORD`:

```env
POSTGRES_DB=battlegame
POSTGRES_USER=bguser
POSTGRES_PASSWORD=your_password_here   ← điền vào đây
SERVER_PORT=9000
```

### 3. Chạy Server bằng Docker

> Docker tự động khởi động PostgreSQL và Game Server.
> Không cần cài .NET SDK hay PostgreSQL trên máy.

```bash
# Lần đầu hoặc sau khi thay đổi code
docker compose up --build

# Các lần sau (không đổi code)
docker compose up
```

Kiểm tra server đang chạy:
```bash
docker compose logs server    # xem log game server
docker compose logs db        # xem log database
```

Dừng server:
```bash
docker compose down           # dừng, giữ data
docker compose down -v        # dừng và xóa toàn bộ data DB
```

### 4. Chạy Client (WinForms — chạy trên Windows)

> Client **không** chạy trong Docker. Chạy trực tiếp trên máy Windows.

**Cách 1 — Visual Studio:**
1. Mở `BattleGameSolution.slnx`
2. Chuột phải `BattleGame.Client` → Set as Startup Project
3. Nhấn **F5**

**Cách 2 — Terminal:**
```bash
cd BattleGame.Client
dotnet run
```

> Mở 2 cửa sổ Client để thử nghiệm matchmaking.
> Client mặc định kết nối `127.0.0.1:9000`.
> Nếu server chạy máy khác, sửa địa chỉ trong `Config/ClientConfig.cs`.

### 5. Chạy Test

```bash
cd BattleGame.Test
dotnet test
```

---

## Cấu Trúc Dự Án

```
BattleGameSolution/
├── BattleGame.Client/          # Ứng dụng client (WinForms)
│   ├── Forms/                  # Các màn hình giao diện
│   │   ├── LoginForm           # Màn hình đăng nhập
│   │   ├── MenuForm            # Màn hình chính
│   │   ├── CharacterSelection  # Chọn nhân vật
│   │   ├── GameForm            # Màn hình trận đấu
│   │   └── GameOverForm        # Màn hình kết thúc trận
│   ├── Game/                   # Logic hiển thị và điều khiển
│   │   ├── GameEngine          # Vòng lặp game (60fps)
│   │   ├── GameStateManager    # Quản lý trạng thái game
│   │   ├── AnimationManager    # Quản lý animation spritesheet
│   │   └── CharacterRenderer   # Render nhân vật lên màn hình
│   ├── Managers/               # Các manager hỗ trợ
│   │   ├── InputManager        # Xử lý input bàn phím
│   │   ├── SoundManager        # Quản lý âm thanh BGM/SFX
│   │   ├── AssetManager        # Tải và cache tài nguyên
│   │   └── NetworkManager      # Trung gian gửi/nhận packet
│   ├── Network/                # Giao tiếp mạng phía client
│   │   ├── ClientSocket        # Kết nối TCP tới server
│   │   └── MatchmakingClient   # Gửi yêu cầu tìm trận
│   ├── Config/
│   │   └── ClientConfig        # Cấu hình địa chỉ server, âm lượng
│   └── Assets/                 # Tài nguyên game
│       ├── Background/         # Hình nền các màn chơi
│       ├── Characters/         # Spritesheet nhân vật
│       ├── Sounds/BGM/         # Nhạc nền
│       ├── Sounds/SFX/         # Hiệu ứng âm thanh
│       └── UI/                 # Ảnh giao diện
│
├── BattleGame.Server/          # Ứng dụng server (chạy trong Docker)
│   ├── Network/
│   │   ├── GameServer          # Lắng nghe kết nối TCP port 9000
│   │   └── ClientHandler       # Xử lý từng client trên thread riêng
│   ├── Services/
│   │   ├── AuthService         # Xác thực đăng nhập, BCrypt
│   │   └── MatchmakingService  # Ghép cặp người chơi
│   ├── Game/
│   │   ├── BattleEngine        # Vòng lặp server tick (50ms)
│   │   ├── CombatSystem        # Tính toán damage, skill
│   │   ├── GameManager         # Quản lý danh sách phòng
│   │   ├── GameRoom            # Phòng chứa 2 người chơi
│   │   └── Match               # Kết quả trận đấu
│   ├── Database/
│   │   ├── DbInitializer       # Tạo bảng khi khởi động
│   │   ├── UserRepository      # CRUD tài khoản
│   │   └── MatchRepository     # Lưu lịch sử trận
│   ├── Config/
│   │   └── ServerConfig        # Port, DB connection string
│   ├── Logging/
│   │   └── ServerLogger        # Ghi log ra file theo ngày
│   ├── Dockerfile              # Build Docker image
│   └── appsettings.json        # Cấu hình runtime
│
├── BattleGame.Shared/          # Thư viện dùng chung
│   ├── Models/                 # Class dữ liệu thuần
│   │   ├── Character / CharacterState
│   │   ├── Player / PlayerState
│   │   ├── GameState
│   │   └── Skill
│   ├── Packets/                # Gói tin giao tiếp mạng
│   │   ├── Packet / PacketType
│   │   ├── LoginPacket / LoginResultPacket
│   │   ├── MatchRequestPacket / MatchFoundPacket
│   │   ├── SelectionCharacterPacket
│   │   ├── MovePacket / AttackPacket
│   │   ├── GameStatePacket / HealthUpdatePacket
│   │   ├── GameOverPacket
│   │   └── DisconnectPacket
│   ├── Network/
│   │   ├── BaseSocket          # Lớp socket nền tảng
│   │   └── PacketSerializer    # Serialize / deserialize packet
│   ├── Config/
│   │   └── GameConstants       # Hằng số dùng chung (port, tickrate...)
│   └── Utils/
│       └── Logger              # Logger dùng chung
│
├── BattleGame.Test/            # Dự án kiểm thử (xUnit)
│   ├── Shared/
│   │   └── PacketSerializerTests
│   ├── Server/
│   │   ├── CombatSystemTests
│   │   └── AuthServiceTests
│   └── Integration/
│       └── MatchmakingIntegrationTests
│
├── scripts/
│   └── init.sql                # Tạo bảng PostgreSQL tự động
├── docker-compose.yml          # Dựng server + database
├── .env                        # Biến môi trường (KHÔNG commit lên git)
├── .env.example                # Template biến môi trường (commit bình thường)
├── .gitignore
├── .dockerignore
└── README.md
```

---

## Luồng Hoạt Động

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
   │══════════════ Vòng lặp trận đấu (50ms/tick) ══════════ │
   │──── MovePacket ──────────►│                            │
   │                           │───── GameStatePacket ────► │
   │──── AttackPacket ────────►│                            │
   │◄─── HealthUpdatePacket ───│                            │
   │◄─── GameStatePacket ──────│                            │
   │                           │───── GameOverPacket ──────►│
   │══════════════════════════════════════════════════════  │
   │──── DisconnectPacket ────►│◄─── DisconnectPacket ───── │
```

---

## Giao Thức Mạng

| Packet | Hướng | Mô tả |
|---|---|---|
| `LoginPacket` | Client → Server | Gửi thông tin đăng nhập |
| `LoginResultPacket` | Server → Client | Kết quả xác thực |
| `MatchRequestPacket` | Client → Server | Yêu cầu tìm trận |
| `MatchFoundPacket` | Server → Client | Thông báo đã ghép cặp |
| `SelectionCharacterPacket` | Client → Server | Chọn nhân vật |
| `MovePacket` | Client → Server | Di chuyển nhân vật |
| `AttackPacket` | Client → Server | Thực hiện tấn công |
| `GameStatePacket` | Server → Client | Đồng bộ trạng thái game (mỗi tick) |
| `HealthUpdatePacket` | Server → Client | Cập nhật HP ngay lập tức |
| `GameOverPacket` | Server → Client | Kết thúc trận đấu |
| `DisconnectPacket` | Client ↔ Server | Ngắt kết nối có chủ ý |

---

## Cấu Hình

| File | Mục đích |
|---|---|
| `.env` | Mật khẩu DB, port — **KHÔNG commit lên git** |
| `.env.example` | Template cho thành viên mới clone về |
| `BattleGame.Server/appsettings.json` | Port server, connection string DB |
| `BattleGame.Client/Config/ClientConfig.cs` | Địa chỉ server, cài đặt client |
| `BattleGame.Shared/Config/GameConstants.cs` | Hằng số dùng chung (port, tickrate) |
| `docker-compose.yml` | Cấu hình Docker: port, biến môi trường |

---

## Xử Lý Sự Cố

**Quên tạo file .env:**
```bash
cp .env.example .env
# Điền POSTGRES_PASSWORD vào .env rồi chạy lại
docker compose up --build
```

**Server không kết nối được DB:**
```bash
docker compose logs db
# Chờ dòng: database system is ready to accept connections
```

**Port 9000 bị chiếm:**
```bash
# Đổi SERVER_PORT trong .env: SERVER_PORT=9001
# Cập nhật ClientConfig.cs: ServerPort = 9001
docker compose up --build
```

**Reset toàn bộ (xóa data DB):**
```bash
docker compose down -v
docker compose up --build
```

**VS báo project unloaded:**
```
Chuột phải từng project → Reload Project
```