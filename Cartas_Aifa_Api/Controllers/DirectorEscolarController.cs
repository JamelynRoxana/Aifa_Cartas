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
    public class DirectorEscolarController : Controller
    {
        private Model1 db = new Model1();

        // GET: DirectorEscolar
        public ActionResult Index()
        {
            var directoresEscolares = db.DirectoresEscolares.Include(d => d.Facultad);
            return View(directoresEscolares.ToList());
        }

        // GET: DirectorEscolar/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectorEscolar directorEscolar = db.DirectoresEscolares.Find(id);
            if (directorEscolar == null)
            {
                return HttpNotFound();
            }
            return View(directorEscolar);
        }

        // GET: DirectorEscolar/Create
        public ActionResult Create()
        {
            ViewBag.IdF = new SelectList(db.Facultades, "Id", "NombreF");
            return View();
        }

        // POST: DirectorEscolar/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IdF,Nombre,Rango,Puesto")] DirectorEscolar directorEscolar)
        {
            if (ModelState.IsValid)
            {
                db.DirectoresEscolares.Add(directorEscolar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdF = new SelectList(db.Facultades, "Id", "NombreF", directorEscolar.IdF);
            return View(directorEscolar);
        }

        // GET: DirectorEscolar/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectorEscolar directorEscolar = db.DirectoresEscolares.Find(id);
            if (directorEscolar == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdF = new SelectList(db.Facultades, "Id", "NombreF", directorEscolar.IdF);
            return View(directorEscolar);
        }

        // POST: DirectorEscolar/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IdF,Nombre,Rango,Puesto")] DirectorEscolar directorEscolar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(directorEscolar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdF = new SelectList(db.Facultades, "Id", "NombreF", directorEscolar.IdF);
            return View(directorEscolar);
        }

        // GET: DirectorEscolar/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DirectorEscolar directorEscolar = db.DirectoresEscolares.Find(id);
            if (directorEscolar == null)
            {
                return HttpNotFound();
            }
            return View(directorEscolar);
        }

        // POST: DirectorEscolar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DirectorEscolar directorEscolar = db.DirectoresEscolares.Find(id);
            db.DirectoresEscolares.Remove(directorEscolar);
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
