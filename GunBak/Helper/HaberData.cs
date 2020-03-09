using GunBak.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GunBak.Helpers
{
	public class HaberData
	{
		public gundembakEntities gundem = new gundembakEntities();

		public IList<RSSHaber> RssData(IList<string> lnk)
		{
			gundembakEntities gunbak = new gundembakEntities();
			IList<RSSHaber> RSSFeedData2 = new List<RSSHaber>();
			IList<RSSHaber> Haberler = new List<RSSHaber>();
			WebClient wclient = new WebClient();
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			string RSSData = "";
			foreach (string item in lnk)
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.Encoding = Encoding.UTF8;
					RSSData = webClient.DownloadString(item.ToString());
				}
				XDocument xml = XDocument.Parse(RSSData);
				XmlDocument xmldoc = new XmlDocument();
				xmldoc.Load(item);
				XmlNamespaceManager xnm = new XmlNamespaceManager(xmldoc.NameTable);
				xnm.AddNamespace("media", "http://search.yahoo.com/mrss/");
				XmlNodeList nList = xmldoc.SelectNodes("//item/enclosure/@url", xnm);
				XmlNodeList mediacont = xmldoc.SelectNodes("//item/media:content/@url", xnm);
				XmlNodeList mediathumb = xmldoc.SelectNodes("//item/media:thumbnail/@url", xnm);
				int r = 0;
				RSSFeedData2 = (from x in xml.Descendants("item")
					select new RSSHaber
					{
						Title = (string)x.Element("title"),
						Link = (string)x.Element("link"),
						Description = (string)x.Element("description"),
						PubDate = (string)x.Element("pubDate"),
						Media = (string)x.Element("image")
					}).ToList();
				foreach (RSSHaber oge in RSSFeedData2)
				{
					if (oge.Media == null && r < nList.Count)
					{
						oge.Media = nList[r].InnerText;
						r++;
					}
					if (oge.Media == null && r < mediacont.Count)
					{
						oge.Media = mediacont[r].InnerText;
						r++;
					}
					if (oge.Media == null && r < mediathumb.Count)
					{
						oge.Media = mediathumb[r].InnerText;
						r++;
					}
					if (oge.Media == null)
					{
						oge.Media = "https://ichef.bbci.co.uk/images/ic/480xn/p0427rjm.jpg";
					}
				}
				foreach (RSSHaber veri in RSSFeedData2)
				{
					Haberler.Add(veri);
				}
			}
			return Haberler;
		}

		public IList<RSSHaber> Ara(string ifade)
		{
			IList<RSSHaber> HaberAra = new List<RSSHaber>();
			List<string> haberler = gundem.habers.Select((haber x) => x.haberRssLink).ToList();
			IList<RSSHaber> haberlerData = RssData(haberler);
			foreach (RSSHaber item in haberlerData)
			{
				int c = item.Title.ToLower().IndexOf(ifade.ToLower());
				if (c > -1)
				{
					HaberAra.Add(item);
				}
			}
			return HaberAra;
		}
	}
}
