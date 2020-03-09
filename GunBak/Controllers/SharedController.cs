using System.Web.Mvc;

namespace GunBak.Controllers
{
	public class SharedController : Controller
	{
		public ActionResult _Layout()
		{
			return View();
		}

		public ActionResult _LayoutAdmin()
		{
			return View();
		}
	}
}
