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
    public class UniversidadController : Controller
    {
        private Model1 db = new Model1();

        // GET: Universidad
        public ActionResult Index()
        {
            return View(db.Universidades.ToList());
        }

        // GET: Universidad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Universidad universidad = db.Universidades.Find(id);
            if (universidad == null)
            {
                return HttpNotFound();
            }
            return View(universidad);
        }

        // GET: Universidad/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Universidad/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NombreU,DireccionU")] Universidad universidad)
        {
            if (ModelState.IsValid)
            {
                db.Universidades.Add(universidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(universidad);
        }

        // GET: Universidad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Universidad universidad = db.Universidades.Find(id);
            if (universidad == null)
            {
                return HttpNotFound();
            }
            return View(universidad);
        }

        // POST: Universidad/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NombreU,DireccionU")] Universidad universidad)
        {
            if (ModelState.IsValid)
            {
                db.Entry(universidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(universidad);
        }

        // GET: Universidad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Universidad universidad = db.Universidades.Find(id);
            if (universidad == null)
            {
                return HttpNotFound();
            }
            return View(universidad);
        }

        // POST: Universidad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Universidad universidad = db.Universidades.Find(id);
            db.Universidades.Remove(universidad);
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
