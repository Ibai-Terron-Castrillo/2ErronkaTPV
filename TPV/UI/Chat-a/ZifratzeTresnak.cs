using System;
using System.Security.Cryptography;
using System.Text;

namespace TPV
{
    public static class ZifratzeTresnak
    {
        private const string AES_GAKOA = "1234567890abcdef";
        private const int IV_LUZERA = 12;
        private const int ETIKETA_LUZERA = 16;

        public static string Zifratu(string testua)
        {
            byte[] key = Encoding.UTF8.GetBytes(AES_GAKOA);
            byte[] iv = RandomNumberGenerator.GetBytes(IV_LUZERA);
            byte[] plain = Encoding.UTF8.GetBytes(testua);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[ETIKETA_LUZERA];

            using var aes = new AesGcm(key, ETIKETA_LUZERA);
            aes.Encrypt(iv, plain, cipher, tag);

            byte[] output = new byte[iv.Length + cipher.Length + tag.Length];
            Buffer.BlockCopy(iv, 0, output, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, output, iv.Length, cipher.Length);
            Buffer.BlockCopy(tag, 0, output, iv.Length + cipher.Length, tag.Length);
            return Convert.ToBase64String(output);
        }

        public static string Deszifratu(string testuZifratua)
        {
            byte[] edukia = Convert.FromBase64String(testuZifratua);
            if (edukia.Length < IV_LUZERA + ETIKETA_LUZERA)
            {
                throw new ArgumentException("Mezu zifratua ez da baliozkoa.");
            }

            byte[] iv = new byte[IV_LUZERA];
            Buffer.BlockCopy(edukia, 0, iv, 0, IV_LUZERA);

            int cipherLength = edukia.Length - IV_LUZERA - ETIKETA_LUZERA;
            byte[] cipher = new byte[cipherLength];
            byte[] tag = new byte[ETIKETA_LUZERA];

            Buffer.BlockCopy(edukia, IV_LUZERA, cipher, 0, cipherLength);
            Buffer.BlockCopy(edukia, IV_LUZERA + cipherLength, tag, 0, ETIKETA_LUZERA);

            byte[] plain = new byte[cipherLength];
            using var aes = new AesGcm(Encoding.UTF8.GetBytes(AES_GAKOA), ETIKETA_LUZERA);
            aes.Decrypt(iv, cipher, tag, plain);
            return Encoding.UTF8.GetString(plain);
        }
    }
}
