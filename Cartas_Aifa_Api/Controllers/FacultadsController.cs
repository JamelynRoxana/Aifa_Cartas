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
    public class FacultadsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Facultads
        public ActionResult Index()
        {
            var facultades = db.Facultades.Include(f => f.Universidad);
            return View(facultades.ToList());
        }

        // GET: Facultads/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facultad facultad = db.Facultades.Find(id);
            if (facultad == null)
            {
                return HttpNotFound();
            }
            return View(facultad);
        }

        // GET: Facultads/Create
        public ActionResult Create()
        {
            ViewBag.IdU = new SelectList(db.Universidades, "Id", "NombreU");
            return View();
        }

        // POST: Facultads/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdU,NombreF")] Facultad facultad)
        {
            if (ModelState.IsValid)
            {
                db.Facultades.Add(facultad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdU = new SelectList(db.Universidades, "Id", "NombreU", facultad.IdU);
            return View(facultad);
        }

        // GET: Facultads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facultad facultad = db.Facultades.Find(id);
            if (facultad == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdU = new SelectList(db.Universidades, "Id", "NombreU", facultad.IdU);
            return View(facultad);
        }

        // POST: Facultads/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdU,NombreF")] Facultad facultad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(facultad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdU = new SelectList(db.Universidades, "Id", "NombreU", facultad.IdU);
            return View(facultad);
        }

        // GET: Facultads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Facultad facultad = db.Facultades.Find(id);
            if (facultad == null)
            {
                return HttpNotFound();
            }
            return View(facultad);
        }

        // POST: Facultads/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Facultad facultad = db.Facultades.Find(id);
            db.Facultades.Remove(facultad);
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
