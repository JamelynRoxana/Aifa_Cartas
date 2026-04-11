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
    public class DireccionAifaController : Controller
    {
        private Model1 db = new Model1();

        // GET: DireccionAifa
        public ActionResult Index()
        {
            return View(db.DireccionesAifa.ToList());
        }

        // GET: DireccionAifa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DireccionAifa direccionAifa = db.DireccionesAifa.Find(id);
            if (direccionAifa == null)
            {
                return HttpNotFound();
            }
            return View(direccionAifa);
        }

        // GET: DireccionAifa/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DireccionAifa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NombreDir")] DireccionAifa direccionAifa)
        {
            if (ModelState.IsValid)
            {
                db.DireccionesAifa.Add(direccionAifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(direccionAifa);
        }

        // GET: DireccionAifa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DireccionAifa direccionAifa = db.DireccionesAifa.Find(id);
            if (direccionAifa == null)
            {
                return HttpNotFound();
            }
            return View(direccionAifa);
        }

        // POST: DireccionAifa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NombreDir")] DireccionAifa direccionAifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(direccionAifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(direccionAifa);
        }

        // GET: DireccionAifa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DireccionAifa direccionAifa = db.DireccionesAifa.Find(id);
            if (direccionAifa == null)
            {
                return HttpNotFound();
            }
            return View(direccionAifa);
        }

        // POST: DireccionAifa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DireccionAifa direccionAifa = db.DireccionesAifa.Find(id);
            db.DireccionesAifa.Remove(direccionAifa);
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
