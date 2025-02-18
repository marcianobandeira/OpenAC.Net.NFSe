using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenAC.Net.NFSe.Providers;

internal static class ISSMapHelper
{
    public static bool IsValidIssMapCode(this int code)
    {
        return Enum.GetValues<ISSMapCode>().Count(p => (int)p == code) > 0;
    }

    public static ISSMapCode ToIssMapCode(this int code)
    {
        if (!code.IsValidIssMapCode())
            throw new Exception("Não é um Código ISSMap válido: " + code);

        return (ISSMapCode)code;
    }

    public static ISSMapCode? TryToIssMapCode(this int code)
    {
        if (code.IsValidIssMapCode())
            return (ISSMapCode)code;

        return null;
    }

    public static string EncryptIssMap(this string plainText, string chave)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(chave);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            var encryptor = aesAlg.CreateEncryptor();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
            return Convert.ToBase64String(cipherBytes);
        }
    }

    public static string DecryptIssMap(this string cipherText, string chave)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Convert.FromBase64String(chave);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            var encryptor = aesAlg.CreateDecryptor();
            var cipherTextBytes = Convert.FromBase64String(cipherText);
            var uncipherBytes = encryptor.TransformFinalBlock(cipherTextBytes, 0, cipherTextBytes.Length);
            return Encoding.UTF8.GetString(uncipherBytes);
        }
    }
}