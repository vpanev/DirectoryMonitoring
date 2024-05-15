using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MoveIT.Transfer.Task.Application.Helpers
{
	public static class EncryptorHelper
	{
		public static async Task<string> EncryptStringAsync(string key, string plainText)
		{
			var iv = new byte[16];
			byte[] array;

			using (var aes = Aes.Create())
			{
				aes.Key = Encoding.UTF8.GetBytes(key);
				aes.IV = iv;

				var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				await using (var memoryStream = new MemoryStream())
				{
					await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
					{
						await using (var streamWriter = new StreamWriter(cryptoStream))
						{
							await streamWriter.WriteAsync(plainText);
						}

						array = memoryStream.ToArray();
					}
				}
			}

			return Convert.ToBase64String(array);
		}
	}
}
