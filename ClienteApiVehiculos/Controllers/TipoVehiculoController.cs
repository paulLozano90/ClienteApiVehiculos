using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BaseServicios;
using ClienteApiVehiculos.Models;
using Microsoft.Practices.Unity;

namespace ClienteApiVehiculos.Controllers
{
    public class TipoVehiculoController : Controller
    {
        [Dependency]
        public IServiciosRest<TipoVehiculo> Servicios { get; set; }

        // GET: TipoVehiculo
        public ActionResult Index()
        {
            var data = Servicios.Get();
            return View(data);
        }

        public ActionResult Alta()
        {
            return View(new TipoVehiculo());
        }
        [HttpPost]
        public async Task<ActionResult> Alta(TipoVehiculo model)
        {
            var data = await Servicios.Add(model);
            return RedirectToAction("Index");
        }
    }
}