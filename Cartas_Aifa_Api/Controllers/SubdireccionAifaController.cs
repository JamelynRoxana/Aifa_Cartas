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
    public class SubdireccionAifaController : Controller
    {
        private Model1 db = new Model1();

        // GET: SubdireccionAifa
        public ActionResult Index()
        {
            var subdireccionesAifa = db.SubdireccionesAifa.Include(s => s.Direccion);
            return View(subdireccionesAifa.ToList());
        }

        // GET: SubdireccionAifa/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubdireccionAifa subdireccionAifa = db.SubdireccionesAifa.Find(id);
            if (subdireccionAifa == null)
            {
                return HttpNotFound();
            }
            return View(subdireccionAifa);
        }

        // GET: SubdireccionAifa/Create
        public ActionResult Create()
        {
            ViewBag.IdDir = new SelectList(db.DireccionesAifa, "Id", "NombreDir");
            return View();
        }

        // POST: SubdireccionAifa/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdDir,NombreSub")] SubdireccionAifa subdireccionAifa)
        {
            if (ModelState.IsValid)
            {
                db.SubdireccionesAifa.Add(subdireccionAifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdDir = new SelectList(db.DireccionesAifa, "Id", "NombreDir", subdireccionAifa.IdDir);
            return View(subdireccionAifa);
        }

        // GET: SubdireccionAifa/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubdireccionAifa subdireccionAifa = db.SubdireccionesAifa.Find(id);
            if (subdireccionAifa == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdDir = new SelectList(db.DireccionesAifa, "Id", "NombreDir", subdireccionAifa.IdDir);
            return View(subdireccionAifa);
        }

        // POST: SubdireccionAifa/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdDir,NombreSub")] SubdireccionAifa subdireccionAifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subdireccionAifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdDir = new SelectList(db.DireccionesAifa, "Id", "NombreDir", subdireccionAifa.IdDir);
            return View(subdireccionAifa);
        }

        // GET: SubdireccionAifa/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubdireccionAifa subdireccionAifa = db.SubdireccionesAifa.Find(id);
            if (subdireccionAifa == null)
            {
                return HttpNotFound();
            }
            return View(subdireccionAifa);
        }

        // POST: SubdireccionAifa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubdireccionAifa subdireccionAifa = db.SubdireccionesAifa.Find(id);
            db.SubdireccionesAifa.Remove(subdireccionAifa);
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
