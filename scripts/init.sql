-- Bảng tài khoản người dùng
CREATE TABLE IF NOT EXISTS users (
    id            SERIAL PRIMARY KEY,
    username      VARCHAR(50)  UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email         VARCHAR(255) UNIQUE NOT NULL,
    created_at    TIMESTAMP    NOT NULL DEFAULT NOW()
);

-- Bảng lịch sử trận đấu
CREATE TABLE IF NOT EXISTS matches (
    id         SERIAL PRIMARY KEY,
    winner     VARCHAR(50),
    loser      VARCHAR(50),
    duration   INTEGER,
    played_at  TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Bảng OTP tokens
CREATE TABLE IF NOT EXISTS otp_tokens (
    id         SERIAL PRIMARY KEY,
    email      VARCHAR(255) NOT NULL,
    code_hash  VARCHAR(255) NOT NULL,
    expires_at TIMESTAMP    NOT NULL,
    used       BOOLEAN      NOT NULL DEFAULT false,
    attempts   INTEGER      NOT NULL DEFAULT 0,
    created_at TIMESTAMP    NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_otp_email
    ON otp_tokens (email, used, expires_at);