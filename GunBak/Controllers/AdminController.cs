using GunBak.Helpers;
using GunBak.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GunBak.Controllers
{
	public class AdminController : Controller
	{
		private Admin lgn = new Admin();

		public ActionResult Index()
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			base.ViewBag.User = base.Session["panel"];
			return View();
		}

		public ActionResult Logout()
		{
			base.Session["panel"] = null;
			base.Session.Abandon();
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(FormCollection Giris)
		{
			gundembakEntities db = new gundembakEntities();
			string email = Giris["ePosta"];
			List<user> login = db.users.Where((user r) => r.eMail == email).ToList();
			using (List<user>.Enumerator enumerator = login.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					user item = enumerator.Current;
					if (item.eMail == Giris["ePosta"] && item.Password == lgn.getMd5Hash(Giris["Sifre"]))
					{
						base.Session["panel"] = item.userName;
						base.Session["rol"] = item.role;
						return RedirectToAction("Index", "Admin");
					}
					base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
					return View();
				}
			}
			return View();
		}

		public ActionResult HaberSite()
		{
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				List<haber> list = db.habers.ToList();
				return View(list);
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult HaberSiteDuzenle(int id)
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				List<haber> haberlist = db.habers.Where((haber x) => x.haberRssId == id).ToList();
				List<kategori> KategoriAdi = db.kategoris.ToList();
				List<string> katrgoriSelect = db.kategoris.Select((kategori x) => x.kategoriBaslik).ToList();
				base.ViewBag.Kategori = katrgoriSelect;
				base.ViewBag.KategoriSecilen = katrgoriSelect;
				return View(haberlist);
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		[HttpPost]
		public ActionResult HaberSiteDuzenle(FormCollection duzenleHaberData)
		{
			gundembakEntities db = new gundembakEntities();
			string kateegoriAdi = duzenleHaberData["KategoriBaslik"];
			int katId = (from x in db.kategoris
				where x.kategoriBaslik == kateegoriAdi
				select x into y
				select y.kategoriId).FirstOrDefault();
			int siteID = int.Parse(duzenleHaberData["id"]);
			int siteKatID = katId;
			haber haberLinkUpdate = db.habers.Where((haber x) => x.haberRssId == siteID).First();
			haberLinkUpdate.haberRssAdi = duzenleHaberData["site"];
			haberLinkUpdate.haberRssLink = duzenleHaberData["siteRSS"];
			haberLinkUpdate.kategoriId = siteKatID;
			db.SaveChanges();
			List<haber> haberlist = db.habers.Where((haber x) => x.haberRssId == siteID).ToList();
			base.ViewBag.Drm = "Başarıyla Güncellendi";
			return RedirectToAction("HaberSite", "Admin");
		}

		public ActionResult HaberSiteEkle()
		{
			gundembakEntities db = new gundembakEntities();
			if (base.Session["panel"] != null)
			{
				List<string> katrgoriSelect = db.kategoris.Select((kategori x) => x.kategoriBaslik).ToList();
				base.ViewBag.Kategori = katrgoriSelect;
				return View();
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		[HttpPost]
		public ActionResult HaberSiteEkle(FormCollection siteEkle)
		{
			gundembakEntities db = new gundembakEntities();
			haber nesne = new haber();
			string kategoriAdi = siteEkle["KategoriBaslik"];
			int kategoriNumara = (from x in db.kategoris
				where x.kategoriBaslik == kategoriAdi
				select x into y
				select y.kategoriId).FirstOrDefault();
			if (kategoriNumara == 0)
			{
				kategoriNumara = 2;
			}
			int ID = db.habers.Select((haber x) => x.haberRssId).Max();
			nesne.haberRssId = ID++;
			nesne.haberRssAdi = siteEkle["siteAdi"];
			nesne.haberRssLink = siteEkle["siteRSS"];
			nesne.kategoriId = kategoriNumara;
			db.habers.Add(nesne);
			db.SaveChanges();
			base.ViewBag.Drm = "Başarıyla Eklendi";
			return RedirectToAction("HaberSite", "Admin");
		}

		public ActionResult HaberSiteSil(int id)
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				haber haberlist = db.habers.Where((haber x) => x.haberRssId == id).FirstOrDefault();
				db.habers.Remove(haberlist);
				db.SaveChanges();
				base.ViewBag.Drm = "Başarıyla Silindi";
				return RedirectToAction("HaberSite", "Admin");
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult KategoriListele()
		{
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				List<kategori> list = db.kategoris.ToList();
				return View(list);
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult KategoriDuzenle(int id)
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				List<kategori> haberlist = db.kategoris.Where((kategori x) => x.kategoriId == id).ToList();
				List<kategori> KategoriAdi = db.kategoris.ToList();
				return View(haberlist);
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		[HttpPost]
		public ActionResult KategoriDuzenle(FormCollection kategoriDuzenle)
		{
			gundembakEntities db = new gundembakEntities();
			int numara = int.Parse(kategoriDuzenle["kategoriNumara"]);
			kategori haberLinkUpdate = db.kategoris.Where((kategori x) => x.kategoriId == numara).First();
			haberLinkUpdate.kategoriAd = kategoriDuzenle["kategoriAdı"];
			haberLinkUpdate.kategoriBaslik = kategoriDuzenle["kategoriBaslik"];
			haberLinkUpdate.kategoriId = numara;
			List<kategori> haberlist = db.kategoris.Where((kategori x) => x.kategoriId == numara).ToList();
			db.SaveChanges();
			base.ViewBag.Drm = "Başarıyla Güncellendi";
			return RedirectToAction("KategoriListele", "Admin");
		}

		public ActionResult KategoriEkle()
		{
			if (base.Session["panel"] != null)
			{
				return View();
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		[HttpPost]
		public ActionResult KategoriEkle(FormCollection kategoriEkle)
		{
			gundembakEntities db = new gundembakEntities();
			kategori nesne = new kategori();
			int ID = db.kategoris.Select((kategori x) => x.kategoriId).Max();
			nesne.kategoriId = ID++;
			foreach (kategori item in db.kategoris.ToList())
			{
				if (!(item.kategoriAd != kategoriEkle["kategoriAdi"]) || !(item.kategoriBaslik != kategoriEkle["kategoriBaslik"]))
				{
					base.ViewBag.Drm = "Aynı kategori isim ve başlık var!";
				}
				else
				{
					nesne.kategoriAd = kategoriEkle["kategoriAdi"];
					nesne.kategoriBaslik = kategoriEkle["kategoriBaslik"];
				}
			}
			db.kategoris.Add(nesne);
			db.SaveChanges();
			base.ViewBag.Drm = "Başarıyla Eklendi";
			return View();
		}

		public ActionResult KategoriSil(int id)
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				kategori kategorilist = db.kategoris.Where((kategori x) => x.kategoriId == id).FirstOrDefault();
				db.kategoris.Remove(kategorilist);
				db.SaveChanges();
				if (db.SaveChanges() == 1)
				{
					base.ViewBag.Drm = "Başarıyla Silindi";
				}
				return RedirectToAction("KategoriListele", "Admin");
			}
			base.ViewBag.Uyari = "Kullanıcı Adı veya Şifreyi Kontrol Ediniz / Yeni Kayıt Olduysanız ONAY Verilmesini Beklemelisiniz!";
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult Kullanici()
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			gundembakEntities db = new gundembakEntities();
			List<user> user = db.users.ToList();
			return View(user);
		}

		public ActionResult KullaniciEkle()
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			return View();
		}

		[HttpPost]
		public ActionResult KullaniciEkle(FormCollection kullaniciEkle)
		{
			gundembakEntities db = new gundembakEntities();
			int id = db.users.Select((user x) => x.id).Max();
			user kullanici = new user();
			kullanici.id = id++;
			List<user> usr = db.users.ToList();
			foreach (user item in usr)
			{
				if (!(item.userName != kullaniciEkle["userName"]))
				{
					return RedirectToAction("KullaniciEkle", "Admin");
				}
				if (item.eMail != kullaniciEkle["eMail"])
				{
					kullanici.userName = kullaniciEkle["userName"];
					kullanici.eMail = kullaniciEkle["eMail"];
				}
			}
			kullanici.userName = kullaniciEkle["kullaniciAdi"];
			kullanici.eMail = kullaniciEkle["eMail"];
			if (kullaniciEkle["Pass"] == kullaniciEkle["PassTekrar"])
			{
				kullanici.Password = lgn.getMd5Hash(kullaniciEkle["Pass"]);
			}
			kullanici.role = kullaniciEkle["Rol"];
			db.users.Add(kullanici);
			db.SaveChanges();
			base.ViewBag.Drm = "Ekleme başarılı.";
			return RedirectToAction("Kullanici", "Admin");
		}

		public ActionResult KullaniciDuzenle(int id)
		{
			if (id == 0)
			{
				return RedirectToAction("Kullanici", "Admin");
			}
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			gundembakEntities db = new gundembakEntities();
			List<user> user = db.users.Where((user r) => r.id == id).ToList();
			List<string> userSelect = db.users.Select((user x) => x.role).ToList();
			base.ViewBag.Rol = userSelect;
			return View(user);
		}

		[HttpPost]
		public ActionResult KullaniciDuzenle(FormCollection kullaniciDuzenle)
		{
			gundembakEntities db = new gundembakEntities();
			int numara = int.Parse(kullaniciDuzenle["id"]);
			user kullanici = db.users.Where((user x) => x.id == numara).First();
			if (kullaniciDuzenle["Pass"] != null)
			{
				kullanici.userName = kullaniciDuzenle["userName"];
				kullanici.eMail = kullaniciDuzenle["eMail"];
				kullanici.Password = lgn.getMd5Hash(kullaniciDuzenle["Pass"]);
				kullanici.role = kullaniciDuzenle["Rol"];
				db.SaveChanges();
			}
			else
			{
				kullanici.userName = kullaniciDuzenle["userName"];
				kullanici.eMail = kullaniciDuzenle["eMail"];
				kullanici.role = kullaniciDuzenle["Rol"];
				db.SaveChanges();
			}
			base.ViewBag.Drm = "Başarıyla güncellendi";
			return RedirectToAction("Kullanici", "Admin");
		}

		public ActionResult KullaniciSil(int id)
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			if (base.Session["panel"] != null)
			{
				gundembakEntities db = new gundembakEntities();
				user user = db.users.Where((user x) => x.id == id).FirstOrDefault();
				db.users.Remove(user);
				db.SaveChanges();
				base.ViewBag.Drm = "Başarıyla Silindi";
				return RedirectToAction("KategoriListele", "Admin");
			}
			return RedirectToAction("Login", "Admin");
		}

		public ActionResult Reklam()
		{
			if (base.Session["panel"] == null)
			{
				return RedirectToAction("Login", "Admin");
			}
			gundembakEntities db = new gundembakEntities();
			IQueryable<reklam> veri = db.reklams.Where((reklam r) => r.id == 1);
			return View(veri);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Reklam(FormCollection Reklam)
		{
			gundembakEntities db = new gundembakEntities();
			reklam rkl = new reklam();
			rkl.yanreklam = Reklam["yanreklam"];
			rkl.ustreklam = Reklam["ustreklam"];
			db.reklams.Add(rkl);
			db.SaveChanges();
			IQueryable<reklam> veri = db.reklams.Where((reklam r) => r.id == 1);
			return View(veri);
		}
	}
}
