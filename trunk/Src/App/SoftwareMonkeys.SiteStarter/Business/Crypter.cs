using System;
using System.Text;
using System.Security.Cryptography;

namespace SoftwareMonkeys.SiteStarter.Business
{
	/// <summary>
	/// Provides encryption functions for the application.
	/// </summary>
	public class Crypter
	{
		/// <summary>
		/// Encrypts the provided password.
		/// </summary>
		/// <param name="password">The password to encrypt.</param>
		/// <returns>The encrypted password.</returns>
		static public string EncryptPassword(string password)
		{
			// Declarations
			Byte[] originalBytes;
			Byte[] encodedBytes;
			MD5 md5;

			// Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
			md5 = new MD5CryptoServiceProvider();
			originalBytes = ASCIIEncoding.Default.GetBytes(password);
			encodedBytes = md5.ComputeHash(originalBytes);

			// Convert encoded bytes back to a 'readable' string
			return BitConverter.ToString(encodedBytes);
		}
	}
}
