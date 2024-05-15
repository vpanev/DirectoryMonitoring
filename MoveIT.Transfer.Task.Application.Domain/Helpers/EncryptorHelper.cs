using System.Security.Cryptography;
using System.Text;

namespace MoveIT.Transfer.Task.Application.Domain.Helpers
{
	public static class EncryptorHelper
	{
		public static async Task<string> DecryptStringAsync(string key, string cipherText)
		{
			var iv = new byte[16];
			var buffer = Convert.FromBase64String(cipherText);

			using var aes = Aes.Create();
			aes.Key = Encoding.UTF8.GetBytes(key);
			aes.IV = iv;
			var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

			await using var memoryStream = new MemoryStream(buffer);
			await using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			using var streamReader = new StreamReader(cryptoStream);
			return await streamReader.ReadToEndAsync();
		}
	}
}
