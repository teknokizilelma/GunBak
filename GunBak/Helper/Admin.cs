using System.Security.Cryptography;
using System.Text;

namespace GunBak.Helpers
{
	public class Admin
	{
		public string getMd5Hash(string input)
		{
			MD5 md5Hasher = MD5.Create();
			byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}
	}
}
