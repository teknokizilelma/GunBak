using GunBak.Helpers;
using GunBak.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GunBak.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			gundembakEntities gundemHaber = new gundembakEntities();
			HaberData hbr = new HaberData();
			RSSHaberView Anasayfa = new RSSHaberView();
			IList<RSSHaber> MansetData = hbr.RssData((from r in gundemHaber.habers
				where r.kategoriId == 2
				select r into x
				select x.haberRssLink).ToList());
			IList<RSSHaber> SonDakikaData = hbr.RssData((from r in gundemHaber.habers
				where r.kategoriId == 1
				select r into x
				select x.haberRssLink).ToList());
			Anasayfa.Manset = MansetData;
			Anasayfa.SonDakika = SonDakikaData;
			Anasayfa.Kategori = gundemHaber.kategoris.ToList();
			return View(Anasayfa);
		}

		public ActionResult Kategori(string KategoriAdi)
		{
			gundembakEntities gundemHaber = new gundembakEntities();
			HaberData fnk = new HaberData();
			base.ViewBag.Baslik = (from x in gundemHaber.kategoris
				where x.kategoriAd == KategoriAdi
				select x into r
				select r.kategoriBaslik).FirstOrDefault();
			int KategoriId = (from x in gundemHaber.kategoris
				where x.kategoriAd == KategoriAdi
				select x.kategoriId).FirstOrDefault();
			IQueryable<string> kategoriLink = from x in gundemHaber.habers
				where x.kategoriId == KategoriId
				select x.haberRssLink;
			IList<RSSHaber> KategoriHaber = fnk.RssData(kategoriLink.ToList());
			return View(KategoriHaber);
		}

		[HttpPost]
		public ActionResult Ara(FormCollection aramaForm)
		{
			HaberData ara = new HaberData();
			IEnumerable<RSSHaber> asilSonuc = ara.Ara(aramaForm["ara"].ToString()).ToList();
			return View(asilSonuc);
		}
	}
}
