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
    public class DetalleEtapaController : Controller
    {
        private Model1 db = new Model1();

        // GET: DetalleEtapa
        public ActionResult Index()
        {
            return View(db.DetallesEtapas.ToList());
        }

        // GET: DetalleEtapa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleEtapa detalleEtapa = db.DetallesEtapas.Find(id);
            if (detalleEtapa == null)
            {
                return HttpNotFound();
            }
            return View(detalleEtapa);
        }

        // GET: DetalleEtapa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetalleEtapa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TipoEtapa")] DetalleEtapa detalleEtapa)
        {
            if (ModelState.IsValid)
            {
                db.DetallesEtapas.Add(detalleEtapa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(detalleEtapa);
        }

        // GET: DetalleEtapa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleEtapa detalleEtapa = db.DetallesEtapas.Find(id);
            if (detalleEtapa == null)
            {
                return HttpNotFound();
            }
            return View(detalleEtapa);
        }

        // POST: DetalleEtapa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TipoEtapa")] DetalleEtapa detalleEtapa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalleEtapa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(detalleEtapa);
        }

        // GET: DetalleEtapa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetalleEtapa detalleEtapa = db.DetallesEtapas.Find(id);
            if (detalleEtapa == null)
            {
                return HttpNotFound();
            }
            return View(detalleEtapa);
        }

        // POST: DetalleEtapa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetalleEtapa detalleEtapa = db.DetallesEtapas.Find(id);
            db.DetallesEtapas.Remove(detalleEtapa);
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
