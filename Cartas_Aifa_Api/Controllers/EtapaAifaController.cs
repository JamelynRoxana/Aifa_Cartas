using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cartas_Aifa_Api.Models;

namespace Cartas_Aifa_Api.Controllers
{
    public class EtapaAifaController : Controller
    {
        private Model1 db = new Model1();

        // GET: EtapaAifa
        public ActionResult Index()
        {
            var etapasAifa = db.EtapasAifa.Include(e => e.DetalleEtapa).Include(e => e.Direccion).Include(e => e.Estudiante).Include(e => e.Subdireccion);
            return View(etapasAifa.ToList());
        }

        // GET: EtapaAifa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EtapaAifa etapaAifa = db.EtapasAifa.Find(id);
            if (etapaAifa == null)
            {
                return HttpNotFound();
            }
            return View(etapaAifa);
        }

        // GET: EtapaAifa/Create
        public ActionResult Create()
        {
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa");
            ViewBag.IdDireccion = new SelectList(db.DireccionesAifa, "Id", "NombreDir");
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto");
            ViewBag.IdSubdireccion = new SelectList(db.SubdireccionesAifa, "Id", "NombreSub");
            return View();
        }

        // POST: EtapaAifa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdEstudiante,IdDetalleEtapa,IdDireccion,IdSubdireccion,FechaInicio,FechaFin,Activa")] EtapaAifa etapaAifa)
        {
            if (ModelState.IsValid)
            {
                db.EtapasAifa.Add(etapaAifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", etapaAifa.IdDetalleEtapa);
            ViewBag.IdDireccion = new SelectList(db.DireccionesAifa, "Id", "NombreDir", etapaAifa.IdDireccion);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", etapaAifa.IdEstudiante);
            ViewBag.IdSubdireccion = new SelectList(db.SubdireccionesAifa, "Id", "NombreSub", etapaAifa.IdSubdireccion);
            return View(etapaAifa);
        }

        // GET: EtapaAifa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EtapaAifa etapaAifa = db.EtapasAifa.Find(id);
            if (etapaAifa == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", etapaAifa.IdDetalleEtapa);
            ViewBag.IdDireccion = new SelectList(db.DireccionesAifa, "Id", "NombreDir", etapaAifa.IdDireccion);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", etapaAifa.IdEstudiante);
            ViewBag.IdSubdireccion = new SelectList(db.SubdireccionesAifa, "Id", "NombreSub", etapaAifa.IdSubdireccion);
            return View(etapaAifa);
        }

        // POST: EtapaAifa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdEstudiante,IdDetalleEtapa,IdDireccion,IdSubdireccion,FechaInicio,FechaFin,Activa")] EtapaAifa etapaAifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(etapaAifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", etapaAifa.IdDetalleEtapa);
            ViewBag.IdDireccion = new SelectList(db.DireccionesAifa, "Id", "NombreDir", etapaAifa.IdDireccion);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", etapaAifa.IdEstudiante);
            ViewBag.IdSubdireccion = new SelectList(db.SubdireccionesAifa, "Id", "NombreSub", etapaAifa.IdSubdireccion);
            return View(etapaAifa);
        }

        // GET: EtapaAifa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EtapaAifa etapaAifa = db.EtapasAifa.Find(id);
            if (etapaAifa == null)
            {
                return HttpNotFound();
            }
            return View(etapaAifa);
        }

        // POST: EtapaAifa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EtapaAifa etapaAifa = db.EtapasAifa.Find(id);
            db.EtapasAifa.Remove(etapaAifa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
