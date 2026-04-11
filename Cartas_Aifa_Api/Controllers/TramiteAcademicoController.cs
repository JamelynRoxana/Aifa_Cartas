using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cartas_Aifa_Api.Models;
using Rotativa; // <--- Agrega esta línea

namespace Cartas_Aifa_Api.Controllers
{
    public class TramiteAcademicoController : Controller
    {
        private Model1 db = new Model1();

        // GET: TramiteAcademico
        public ActionResult Index()
        {
            var tramitesAcademicos = db.TramitesAcademicos.Include(t => t.Autoridad).Include(t => t.DetalleEtapa).Include(t => t.Estudiante).Include(t => t.TipoCarta);
            return View(tramitesAcademicos.ToList());
        }

        // GET: TramiteAcademico/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TramiteAcademico tramiteAcademico = db.TramitesAcademicos.Find(id);
            if (tramiteAcademico == null)
            {
                return HttpNotFound();
            }
            return View(tramiteAcademico);
        }

        // GET: TramiteAcademico/Create
        public ActionResult Create()
        {
            ViewBag.IdAut = new SelectList(db.AutoridadesAifa, "Id", "Nombre");
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa");
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto");
            ViewBag.IdTipoCarta = new SelectList(db.DetallesTipoCartas, "Id", "NombreCarta");
            return View();
        }

        // POST: TramiteAcademico/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Folio,IdEstudiante,IdDetalleEtapa,IdAut,IdTipoCarta,FechaRegistro")] TramiteAcademico tramiteAcademico)
        {
            if (ModelState.IsValid)
            {
                db.TramitesAcademicos.Add(tramiteAcademico);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAut = new SelectList(db.AutoridadesAifa, "Id", "Nombre", tramiteAcademico.IdAut);
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", tramiteAcademico.IdDetalleEtapa);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", tramiteAcademico.IdEstudiante);
            ViewBag.IdTipoCarta = new SelectList(db.DetallesTipoCartas, "Id", "NombreCarta", tramiteAcademico.IdTipoCarta);
            return View(tramiteAcademico);
        }

        // GET: TramiteAcademico/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TramiteAcademico tramiteAcademico = db.TramitesAcademicos.Find(id);
            if (tramiteAcademico == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAut = new SelectList(db.AutoridadesAifa, "Id", "Nombre", tramiteAcademico.IdAut);
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", tramiteAcademico.IdDetalleEtapa);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", tramiteAcademico.IdEstudiante);
            ViewBag.IdTipoCarta = new SelectList(db.DetallesTipoCartas, "Id", "NombreCarta", tramiteAcademico.IdTipoCarta);
            return View(tramiteAcademico);
        }

        // POST: TramiteAcademico/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Folio,IdEstudiante,IdDetalleEtapa,IdAut,IdTipoCarta,FechaRegistro")] TramiteAcademico tramiteAcademico)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tramiteAcademico).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAut = new SelectList(db.AutoridadesAifa, "Id", "Nombre", tramiteAcademico.IdAut);
            ViewBag.IdDetalleEtapa = new SelectList(db.DetallesEtapas, "Id", "TipoEtapa", tramiteAcademico.IdDetalleEtapa);
            ViewBag.IdEstudiante = new SelectList(db.Estudiantes, "Id", "NombreCompleto", tramiteAcademico.IdEstudiante);
            ViewBag.IdTipoCarta = new SelectList(db.DetallesTipoCartas, "Id", "NombreCarta", tramiteAcademico.IdTipoCarta);
            return View(tramiteAcademico);
        }

        // GET: TramiteAcademico/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TramiteAcademico tramiteAcademico = db.TramitesAcademicos.Find(id);
            if (tramiteAcademico == null)
            {
                return HttpNotFound();
            }
            return View(tramiteAcademico);
        }


        // GET: TramiteAcademico/ImprimirCarta/5
        public ActionResult ImprimirCarta(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // 1. Obtener el trámite con todos sus datos relacionados
            var tramite = db.TramitesAcademicos
                .Include(t => t.Estudiante)
                .Include(t => t.Autoridad)
                .Include(t => t.DetalleEtapa)
                .Include(t => t.TipoCarta)
                .FirstOrDefault(t => t.Id == id);

            if (tramite == null) return HttpNotFound();

            // 2. Buscar la etapa AIFA activa para este alumno (necesaria para Dirección/Subdirección)
            var etapa = db.EtapasAifa
                .Include(e => e.Direccion)
                .Include(e => e.Subdireccion)
                .FirstOrDefault(e => e.IdEstudiante == tramite.IdEstudiante
                                  && e.IdDetalleEtapa == tramite.IdDetalleEtapa
                                  && e.Activa);

            // 3. Buscar imágenes de configuración visual válidas para la fecha de hoy
            var hoy = DateTime.Now;
            var imagenesConfig = db.ConfiguracionesVisuales
                .Include(c => c.Imagen)
                .Where(c => hoy >= c.FechaInicio && hoy <= c.FechaFin)
                .ToList();

            // Pasar datos adicionales a la vista mediante ViewBag
            ViewBag.Etapa = etapa;
            ViewBag.Imagenes = imagenesConfig;

            // Retornar el PDF usando Rotativa
            return new Rotativa.ViewAsPdf("VistaPdf", tramite)
            {
                FileName = "Carta_" + tramite.Folio + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(0, 0, 0, 0)
            };
        }



        // POST: TramiteAcademico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TramiteAcademico tramiteAcademico = db.TramitesAcademicos.Find(id);
            db.TramitesAcademicos.Remove(tramiteAcademico);
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
