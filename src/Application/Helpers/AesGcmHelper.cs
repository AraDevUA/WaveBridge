using System.Security.Cryptography;
using System.Text;


namespace Application.Helpers;

public static class AesGcmHelper
{
    private const int TagSizeInBytes = 16;

    public static string Encrypt(string plainText, string keyBase64)
    {
        if (string.IsNullOrEmpty(plainText)) throw new ArgumentException(nameof(plainText));
        if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentException(nameof(keyBase64));

        var key = Convert.FromBase64String(keyBase64);
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("Invalid key length");

        var plaintextBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = new byte[12];

        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagSizeInBytes];
        var aad = Encoding.UTF8.GetBytes("refresh_token_v1");

        using (var aes = new AesGcm(key, TagSizeInBytes))
        {
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag, aad);
        }

        var combined = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, combined, nonce.Length + tag.Length, ciphertext.Length);

        Array.Clear(plaintextBytes, 0, plaintextBytes.Length);
        Array.Clear(ciphertext, 0, ciphertext.Length);
        Array.Clear(tag, 0, tag.Length);
        Array.Clear(key, 0, key.Length);

        return Convert.ToBase64String(combined);
    }

    public static string Decrypt(string cipherTextBase64, string keyBase64)
    {
        if (string.IsNullOrEmpty(cipherTextBase64)) throw new ArgumentException(nameof(cipherTextBase64));
        if (string.IsNullOrEmpty(keyBase64)) throw new ArgumentException(nameof(keyBase64));

        var combined = Convert.FromBase64String(cipherTextBase64);
        var key = Convert.FromBase64String(keyBase64);
        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            throw new ArgumentException("Invalid key length");
        if (combined.Length < 12 + TagSizeInBytes)
            throw new ArgumentException("Invalid ciphertext");

        var nonce = new byte[12];
        var tag = new byte[TagSizeInBytes];
        var ciphertextLength = combined.Length - nonce.Length - tag.Length;
        var ciphertext = new byte[ciphertextLength];

        Buffer.BlockCopy(combined, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(combined, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(combined, nonce.Length + tag.Length, ciphertext, 0, ciphertextLength);

        var plaintextBytes = new byte[ciphertextLength];
        var aad = Encoding.UTF8.GetBytes("refresh_token_v1");

        using (var aes = new AesGcm(key, TagSizeInBytes))
        {
            aes.Decrypt(nonce, ciphertext, tag, plaintextBytes, aad);
        }

        var result = Encoding.UTF8.GetString(plaintextBytes);

        Array.Clear(plaintextBytes, 0, plaintextBytes.Length);
        Array.Clear(ciphertext, 0, ciphertext.Length);
        Array.Clear(tag, 0, tag.Length);
        Array.Clear(key, 0, key.Length);

        return result;
    }
}
