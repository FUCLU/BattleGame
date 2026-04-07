using BattleGame.Shared.Security;
using Xunit;

namespace BattleGame.Test.Shared
{
    public class AesEncryptionTests
    {
        [Fact]
        public void Encrypt_ThenDecrypt_ReturnsSameString()
        {
            string original = "{\"Type\":1,\"Username\":\"FUCLU\",\"Password\":\"123456\"}";
            string encrypted = AesEncryption.Encrypt(original);
            string decrypted = AesEncryption.Decrypt(encrypted);
            Assert.Equal(original, decrypted);
        }

        [Fact]
        public void Encrypt_SameInput_DifferentOutput()
        {
            string input = "test";
            string enc1 = AesEncryption.Encrypt(input);
            string enc2 = AesEncryption.Encrypt(input);
            Assert.NotEqual(enc1, enc2);
        }

        [Fact]
        public void Decrypt_InvalidBase64_ThrowsException()
        {
            Assert.Throws<FormatException>(() => AesEncryption.Decrypt("not_base64!!!"));
        }
    }
}