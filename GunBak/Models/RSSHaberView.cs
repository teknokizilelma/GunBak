using System.Collections.Generic;

namespace GunBak.Models
{
	public class RSSHaberView
	{
		public IEnumerable<RSSHaber> SonDakika
		{
			get;
			set;
		}

		public IEnumerable<RSSHaber> Manset
		{
			get;
			set;
		}

		public IEnumerable<kategori> Kategori
		{
			get;
			set;
		}
	}
}
