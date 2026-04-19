
```markdown
# BATTLEGAME PROJECT CONTEXT

> **Lưu ý quan trọng cho LLM:**  
> Đây là tài liệu thiết kế và kiến trúc toàn bộ của dự án **BattleGame**.  
> Hãy sử dụng thông tin trong file này làm cơ sở chính xác để trả lời câu hỏi, viết code, sửa lỗi, phân tích và tư vấn giải pháp kỹ thuật.  
> **Không tự bịa đặt** cấu trúc thư mục, packet, tính năng hoặc công nghệ ngoài những gì đã được liệt kê trong file này trừ khi được yêu cầu rõ ràng.

---

# NT106.Q23.ANTT — LẬP TRÌNH MẠNG CĂN BẢN

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL_15-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## I. Thông Tin Đồ Án
**Tên đề tài:** Thiết kế và xây dựng game đối kháng 2D bằng C# trên nền tảng .NET Windows Forms

### Danh sách thành viên
| STT | Họ và tên              | MSSV      | Vai trò                        |
|-----|------------------------|-----------|--------------------------------|
| 1   | Lưu Hồng Phúc          | 24521382  | Game Architect / Server        |
| 2   | Phan Thái Hưng         | 24520624  | Gameplay Programmer            |
| 3   | Nguyễn Tấn Phát        | 24521306  | UI / UX Game Developer         |
| 4   | Nguyễn Phan Hoàng Long | 24521006  | Network Programmer             |

## II. Mô Tả Tổng Quan
**BattleGame** là game đối kháng 2D thời gian thực qua mạng, được xây dựng bằng **C# .NET 8** và **Windows Forms**.  

Hai người chơi kết nối tới server trung tâm qua **Load Balancer**, chọn nhân vật, được ghép cặp tự động và thi đấu thông qua giao tiếp **TCP Socket**.

### Các tính năng chính
- Đăng ký tài khoản với xác thực OTP qua Email (SMTP)
- Đăng nhập / xác thực tài khoản (BCrypt)
- Quên mật khẩu / đặt lại mật khẩu qua OTP Email
- Mã hóa toàn bộ gói tin bằng **AES-256-CBC**
- Load Balancer TCP (Round Robin) phân phối client vào các GameServer
- Health Check tự động loại bỏ server chết khỏi pool
- Hệ thống matchmaking tự động ghép 2 người chơi vào cùng một phòng
- Bot AI tự động thay thế khi không đủ người chơi (**NoPlayer mode**)
- Trận đấu real-time: di chuyển, tấn công, sử dụng kỹ năng
- Đồng bộ trạng thái game liên tục giữa server và client (**50ms/tick**)
- Hiển thị animation nhân vật và hiệu ứng chiến đấu
- Hệ thống âm thanh BGM và SFX
- Lịch sử trận đấu và bảng xếp hạng

### Công nghệ sử dụng
| Thành phần              | Công nghệ                                      |
|-------------------------|------------------------------------------------|
| Ngôn ngữ lập trình      | C# (.NET 8)                                    |
| Giao diện               | Windows Forms                                  |
| Giao tiếp mạng          | TCP Socket (`System.Net.Sockets`)              |
| Serialization           | Custom `PacketSerializer` (JSON)               |
| Mã hóa truyền tin       | AES-256-CBC (`System.Security.Cryptography`)   |
| Load Balancing          | TCP Round Robin (custom)                       |
| Cơ sở dữ liệu           | PostgreSQL 15                                  |
| Xác thực mật khẩu       | BCrypt (`BCrypt.Net-Next`)                     |
| Gửi Email OTP           | SMTP Gmail / Mailpit (dev)                     |
| Container               | Docker + Docker Compose                        |
| Kiểm thử                | xUnit                                          |

## III. Yêu Cầu Hệ Thống
| Công cụ            | Phiên bản          | Ghi chú                                              |
|--------------------|--------------------|------------------------------------------------------|
| Docker Desktop     | 4.x trở lên        | Bắt buộc để chạy Server + LB + DB + Mailpit          |
| .NET SDK           | 8.0                | Chỉ cần khi chạy Client hoặc build thủ công          |
| Visual Studio      | 2022               | Khuyến nghị để phát triển                            |
| Windows            | 10/11 64-bit       | Để chạy Client Windows Forms                         |

## IV. Hướng Dẫn Cài Đặt & Chạy

### 1. Clone dự án
```bash
git clone https://github.com/FUCLU/BattleGame.git
cd BattleGameSolution
```

### 2. Tạo file `.env`
```bash
cp .env.example .env
```
Mở file `.env` và điền thông tin (đặc biệt là `POSTGRES_PASSWORD`).

### 3. Cài NuGet packages
```bash
dotnet restore
```

### 4. Chạy Server + Load Balancer + Database
```bash
# Lần đầu hoặc sau khi thay đổi code
docker compose up --build

# Các lần sau
docker compose up -d
```

**Các service sau khi chạy:**
| Service              | Địa chỉ                     | Mô tả                              |
|----------------------|-----------------------------|------------------------------------|
| Load Balancer        | `localhost:9000`            | Entry point cho Client             |
| Game Server          | `localhost:9001`            | GameServer (chạy trong Docker)     |
| PostgreSQL           | `localhost:5433`            | Database                           |
| Mailpit Web UI       | `http://localhost:8025`     | Xem email OTP                      |
| Mailpit SMTP         | `localhost:1025`            | SMTP dev local                     |

### 5. Chạy Client (WinForms)
Client **không** chạy trong Docker. Chạy trực tiếp trên Windows.

- Mở file `BattleGameSolution.slnx`
- Chuột phải vào **BattleGame.Client** → **Set as Startup Project**
- Nhấn **F5**

Hoặc chạy bằng terminal:
```bash
dotnet run --project BattleGame.Client
```

> **Mẹo:** Mở 2 cửa sổ Client để test chế độ Online.

### 6. Debug Server bằng Visual Studio
```bash
docker compose up -d db mailpit
```
Sau đó set `BattleGame.Server` làm Startup Project và chạy với **F5**.

### 7. Chạy Test
```bash
dotnet test BattleGame.Test
```

## V. Cấu Trúc Dự Án
Chi tiết cấu trúc dự án (quan trọng):

```
BattleGameSolution/
│
├── .env
├── .env.example
├── .gitignore
├── docker-compose.yml
├── BattleGameSolution.slnx
├── README.md
│
├── BattleGame.Client/                          ← Client (WinForms + Custom Engine)
│   ├── BattleGame.Client.csproj
│   ├── Program.cs
│   │
│   ├── Assets/                                 ← Tài nguyên hình ảnh & âm thanh
│   │   ├── Background/
│   │   ├── Characters/
│   │   │   └── Soldier/                        ← (các nhân vật khác tương tự)
│   │   │       ├── Idle.png
│   │   │       ├── Run.png
│   │   │       ├── Attack.png
│   │   │       ├── Hurt.png
│   │   │       ├── Dead.png
│   │   │       └── ...
│   │   └── Sounds/
│   │       └── BGM/
│   │
│   ├── Configs/                                ← Data-driven (rất quan trọng)
│   │   ├── Characters/
│   │   │   └── soldier.json                    ← stats, skills list...
│   │   ├── Skills/
│   │   │   ├── shoot.json
│   │   │   ├── grenade.json
│   │   │   └── ...
│   │   └── Animations/
│   │
│   ├── Forms/                                  ← UI (WinForms)
│   │   ├── LoginForm.cs
│   │   ├── MenuForm.cs
│   │   ├── ModeForm.cs
│   │   ├── CharacterSelection.cs
│   │   ├── RoomForm.cs
│   │   ├── GameForm.cs
│   │   ├── GameOverForm.cs
│   │   ├── RoundStartForm.cs
│   │   ├── RoundEndForm.cs
│   │   └── ...
│   │
│   ├── Game/                                   ← Core Game Logic
│   │   ├── Engine/                             ← ECS-like System
│   │   │   ├── Components/
│   │   │   │   ├── IComponent.cs
│   │   │   │   ├── MovementComponent.cs
│   │   │   │   ├── CombatComponent.cs
│   │   │   │   ├── HealthComponent.cs
│   │   │   │   ├── SkillComponent.cs
│   │   │   │   └── StatusComponent.cs
│   │   │   │
│   │   │   ├── Entities/
│   │   │   │   └── Entity.cs
│   │   │   │
│   │   │   ├── Systems/
│   │   │   │   ├── ISystem.cs
│   │   │   │   ├── MovementSystem.cs
│   │   │   │   ├── CombatSystem.cs
│   │   │   │   ├── SkillsSystem.cs
│   │   │   │   ├── ProjectitleSystem.cs
│   │   │   │   ├── StatusSystem.cs
│   │   │   │   └── Gameloop.cs                 ← Main game loop (tick)
│   │   │   │
│   │   │   ├── Event/
│   │   │   │   └── EventBus.cs
│   │   │   │
│   │   │   └── Time/
│   │   │       └── GameTime.cs
│   │   │
│   │   ├── Gameplay/                           ← Game objects & logic
│   │   │   ├── Characters/
│   │   │   │   └── CharacterFactory.cs
│   │   │   │
│   │   │   ├── Entities/
│   │   │   │   └── Character.cs
│   │   │   │
│   │   │   ├── Projectitles/
│   │   │   │   ├── ProjectitleBase.cs
│   │   │   │   ├── Bullet.cs
│   │   │   │   └── Grenade.cs
│   │   │   │
│   │   │   ├── Skills
│   │   │   │   ├── ShootSkill.cs
│   │   │   │   ├── GrenadeSkill.cs
│   │   │   │   └── BasicAttackSkill.cs
│   │   │   │   
│   │   │   │
│   │   │   ├── Effects/
│   │   │   │   └── StunEffect.cs
│   │   │   │
│   │   │   └── Rules/
│   │   │       └── DamageCalculator.cs
│   │   │
│   │   ├── Rendering/                          ← Tách biệt render
│   │   │   ├── AnimationController.cs
│   │   │   ├── AnimationLoader.cs
│   │   │   ├── CharacterRenderer.cs
│   │   │   └── SpriteAnimation.cs
│   │   │
│   │   └── Runtime/
│   │       ├── Input/
│   │       │   └── PlayerController.cs
│   │       ├── Managers/
│   │       │   ├── GameManager.cs
│   │       │   └── EntityManager.cs
│   │       └── State/
│   │           └── GameState.cs
│   │
│   ├── Managers/                               ← Global managers
│   │   ├── AssetManager.cs
│   │   ├── InputManager.cs
│   │   ├── NetworkManager.cs
│   │   └── SoundManager.cs
│   │
│   ├── Network/                                ← Networking
│   │   ├── ClientSocket.cs
│   │   ├── MatchmakingClient.cs
│   │   └── PacketHandler.cs
│   │
│   ├── Config/
│   │   └── ClientConfig.cs
│   │
│   └── Security/
│       └── AesEncryption.cs
│
├── BattleGame.Shared/                          ← Code dùng chung Client & Server
│   ├── BattleGame.Shared.csproj
│   │
│   ├── Config/
│   │   └── GameConstants.cs
│   │
│   ├── Models/                                 ← Data models
│   │   ├── CharacterData.cs
│   │   ├── CharacterState.cs
│   │   ├── Player.cs
│   │   ├── PlayerState.cs
│   │   ├── GameState.cs
│   │   └── SkillData.cs
│   │
│   ├── Packets/                                ← Tất cả packet
│   │   ├── Packet.cs
│   │   ├── PacketType.cs
│   │   ├── LoginPacket.cs
│   │   ├── MovePacket.cs
│   │   ├── AttackPacket.cs
│   │   ├── GameStatePacket.cs
│   │   └── ...
│   │
│   ├── Network/
│   │   ├── BaseSocket.cs
│   │   └── PacketSerializer.cs
│   │
│   ├── Security/
│   │   └── AesEncryption.cs
│   │
│   └── Utils/
│       └── Logger.cs
│
├── BattleGame.Server/                          ← Server
│   ├── BattleGame.Server.csproj
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   │
│   ├── Config/
│   │   └── ServerConfig.cs
│   │
│   ├── Database/
│   │   ├── DbInitializer.cs
│   │   ├── UserRepository.cs
│   │   ├── OtpRepository.cs
│   │   └── MatchRepository.cs
│   │
│   ├── Game/                                   ← Server game logic
│   │   ├── BattleEngine.cs                     ← Core tick loop
│   │   ├── GameRoom.cs
│   │   ├── Match.cs
│   │   ├── GameManager.cs
│   │   ├── GameModeManager.cs
│   │   ├── CombatSystem.cs
│   │   ├── PacketProcessor.cs
│   │   └── BotAI.cs
│   │
│   ├── Network/
│   │   ├── GameServer.cs
│   │   ├── ServerSocket.cs
│   │   └── ClientHandler.cs
│   │
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── OtpService.cs
│   │   ├── EmailService.cs
│   │   └── MatchmakingService.cs
│   │
│   └── Logging/
│       └── ServerLogger.cs
│
├── BattleGame.LoadBalancer/                    ← (Optional nhưng đã có)
│   ├── BattleGame.LoadBalancer.csproj
│   ├── Program.cs
│   ├── Dockerfile
│   ├── appsettings.json
│   │
│   ├── Health/
│   │   └── HealthChecker.cs
│   │
│   ├── Network/
│   │   └── LoadBalancerSocket.cs
│   │
│   └── Routing/
│       ├── RoundRoubinRouter.cs
│       └── Redirect.cs
│
├── BattleGame.Test/                            ← Tests
│   ├── BattleGame.Test.csproj
│   ├── Integration/
│   ├── Server/
│   └── Shared/
│
└── scripts/
    └── init.sql
```

## VI. Chế Độ Chơi
| Chế độ      | Điều kiện          | Mô tả                                           |
|-------------|--------------------|-------------------------------------------------|
| Online      | 2 người chơi       | Matchmaking ghép 2 người chơi thật              |
| No Player   | 1 người chơi       | Bot AI tự động thay thế người chơi thứ hai      |

## VII. Giao Thức Mạng (Packets)
Tất cả packet (trừ redirect từ Load Balancer) đều được **mã hóa AES-256-CBC**.

| Packet                      | Hướng             | Type | Mô tả                                      |
|-----------------------------|-------------------|------|--------------------------------------------|
| LoginPacket                 | Client → Server   | 1    | Gửi username + password                    |
| LoginResultPacket           | Server → Client   | 2    | Kết quả xác thực                           |
| RegisterPacket              | Client → Server   | 3    | Đăng ký tài khoản                          |
| OtpPacket                   | Server → Client   | 4    | Thông báo trạng thái OTP                   |
| OtpVerifyPacket             | Client → Server   | 5    | Gửi mã OTP 6 số                            |
| ForgotPasswordPacket        | Client → Server   | 6    | Yêu cầu OTP để reset mật khẩu              |
| ResetPasswordPacket         | Client → Server   | 7    | Đặt mật khẩu mới                           |
| MatchRequestPacket          | Client → Server   | 8    | Yêu cầu tìm trận                           |
| MatchFoundPacket            | Server → Client   | 9    | Ghép cặp thành công                        |
| SelectionCharacterPacket    | Client → Server   | 10   | Chọn nhân vật                              |
| MovePacket                  | Client → Server   | 11   | Di chuyển                                  |
| AttackPacket                | Client → Server   | 12   | Tấn công                                   |
| GameStatePacket             | Server → Client   | 13   | Đồng bộ trạng thái game mỗi 50ms           |
| HealthUpdatePacket          | Server → Client   | 14   | Cập nhật HP                                |
| GameOverPacket              | Server → Client   | 15   | Kết thúc trận đấu                          |
| DisconnectPacket            | Client ↔ Server   | 16   | Ngắt kết nối                               |

## VIII. Cấu Hình Email OTP
- **Môi trường phát triển:** Sử dụng **Mailpit** (xem email tại `http://localhost:8025`)
- **Production:** Cấu hình Gmail SMTP trong file `appsettings.Production.json` và sử dụng App Password.

---

### Cấu trúc thư mục chính

```BattleGame.Client/
│
├── Program.cs                          // entry point, chọn test mode hay online
│
├── Config/
│   └── Characters/
│       └── samurai.json                // chỉ số, skills, frame count animation
│
├── Assets/
│   └── Characters/
│       └── Samurai/                    // spritesheet png của samurai
│
├── Forms/
│   ├── GameForm.cs                     // game loop, nhận input, gọi Draw
│   └── GameForm.Designer.cs           // UI: HP bar, Mana bar, round, timer
│
├── Managers/
│   └── InputManager.cs                // static, lưu trạng thái phím đang giữ
│
└── Game/
    │
    ├── GameEngine.cs                   // khởi tạo tất cả, điều phối Update/Draw
    │
    ├── Core/
    │   ├── Entity.cs                   // container chứa các component, id = string
    │   ├── IComponent.cs              // interface marker
    │   └── Components/
    │       ├── CharacterComponent.cs  // hp, mana, stats, skills, trạng thái hành động
    │       ├── MovementComponent.cs   // vị trí, vận tốc, hướng, groundY
    │       └── SpriteComponent.cs     // animation hiện tại, frame index, timer
    │
    ├── Systems/
    │   ├── CombatSystem.cs            // xử lý attack, skill, damage, stun, cooldown
    │   ├── MovementSystem.cs          // cập nhật vị trí, gravity, giới hạn map
    │   └── AnimationSystem.cs         // chọn animation theo state (dead>stun>attack>...)
    │
    ├── Gameplay/
    │   └── CharacterFactory.cs        // đọc json → tạo Entity với đủ component
    │
    ├── Input/
    │   └── PlayerController.cs        // đọc InputManager → gọi CombatSystem/Movement
    │
    └── Rendering/
        ├── AnimationLoader.cs         // đọc json + png → cắt frame → SpriteAnimation
        ├── AnimationController.cs     // (dự phòng mở rộng)
        ├── CharacterRenderer.cs       // advance frame, vẽ lên Graphics, flip khi trái
        └── SpriteAnimation.cs         // data: frames[], fps, loop, FrameDuration
```
### 2. Luồng Hoạt động Chính (Game Loop)
Game chạy theo Fixed Timestep (~60 FPS) kết hợp Server Authoritative.
Trên Client (GameForm + GameEngine)

Khởi tạo: Program.cs → GameForm(characterId)
Load Data: CharacterFactory.cs đọc Configs/Characters/{id}.json và Assets sprite.
Tạo Entity: Gắn các Components (MovementComponent, HealthComponent, SkillComponent...).
Game Loop (mỗi ~16ms):
Update: Gọi các Systems (MovementSystem, CombatSystem, SkillsSystem, ProjectileSystem...).
Client-side Prediction: Dự đoán vị trí và hành động ngay lập tức.
Render: CharacterRenderer.cs cắt sprite sheet và vẽ lên Graphics.
Network: Gửi InputPacket → Nhận StateUpdate từ server → Reconciliation.


Trên Server (BattleEngine.cs)

Nhận Input từ clients.
Validate + Simulate thế giới (Movement → Collision → Combat → Skill).
Broadcast StateUpdatePacket cho tất cả clients.
Kiểm tra điều kiện thắng/thua.

