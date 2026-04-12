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
    public class DetallesTipoCartaController : Controller
    {
        private Model1 db = new Model1();

        // GET: DetallesTipoCarta
        public ActionResult Index()
        {
            var detallesTipoCartas = db.DetallesTipoCartas.Include(d => d.DetalleEtapa);
            return View(detallesTipoCartas.ToList());
        }

        // GET: DetallesTipoCarta/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallesTipoCarta detallesTipoCarta = db.DetallesTipoCartas.Find(id);
            if (detallesTipoCarta == null)
            {
                return HttpNotFound();
            }
            return View(detallesTipoCarta);
        }

        // GET: DetallesTipoCarta/Create
        public ActionResult Create()
        {
            ViewBag.IdEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa");
            return View();
        }

        // POST: DetallesTipoCarta/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NombreCarta,IdEtapa")] DetallesTipoCarta detallesTipoCarta)
        {
            if (ModelState.IsValid)
            {
                db.DetallesTipoCartas.Add(detallesTipoCarta);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", detallesTipoCarta.IdEtapa);
            return View(detallesTipoCarta);
        }

        // GET: DetallesTipoCarta/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallesTipoCarta detallesTipoCarta = db.DetallesTipoCartas.Find(id);
            if (detallesTipoCarta == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", detallesTipoCarta.IdEtapa);
            return View(detallesTipoCarta);
        }

        // POST: DetallesTipoCarta/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NombreCarta,IdEtapa")] DetallesTipoCarta detallesTipoCarta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detallesTipoCarta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", detallesTipoCarta.IdEtapa);
            return View(detallesTipoCarta);
        }

        // GET: DetallesTipoCarta/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallesTipoCarta detallesTipoCarta = db.DetallesTipoCartas.Find(id);
            if (detallesTipoCarta == null)
            {
                return HttpNotFound();
            }
            return View(detallesTipoCarta);
        }

        // POST: DetallesTipoCarta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetallesTipoCarta detallesTipoCarta = db.DetallesTipoCartas.Find(id);
            db.DetallesTipoCartas.Remove(detallesTipoCarta);
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
