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
    public class AutoridadCartasController : Controller
    {
        private Model1 db = new Model1();

        // GET: AutoridadCartas
        public ActionResult Index()
        {
            return View(db.AutoridadesAifa.ToList());
        }

        // GET: AutoridadCartas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoridadAifa autoridadAifa = db.AutoridadesAifa.Find(id);
            if (autoridadAifa == null)
            {
                return HttpNotFound();
            }
            return View(autoridadAifa);
        }

        // GET: AutoridadCartas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AutoridadCartas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Rango,Puesto")] AutoridadAifa autoridadAifa)
        {
            if (ModelState.IsValid)
            {
                db.AutoridadesAifa.Add(autoridadAifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(autoridadAifa);
        }

        // GET: AutoridadCartas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoridadAifa autoridadAifa = db.AutoridadesAifa.Find(id);
            if (autoridadAifa == null)
            {
                return HttpNotFound();
            }
            return View(autoridadAifa);
        }

        // POST: AutoridadCartas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Rango,Puesto")] AutoridadAifa autoridadAifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(autoridadAifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(autoridadAifa);
        }

        // GET: AutoridadCartas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AutoridadAifa autoridadAifa = db.AutoridadesAifa.Find(id);
            if (autoridadAifa == null)
            {
                return HttpNotFound();
            }
            return View(autoridadAifa);
        }

        // POST: AutoridadCartas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AutoridadAifa autoridadAifa = db.AutoridadesAifa.Find(id);
            db.AutoridadesAifa.Remove(autoridadAifa);
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
