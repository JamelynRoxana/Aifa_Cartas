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
    public class ConfiguracionVisualController : Controller
    {
        private Model1 db = new Model1();

        // ── GET: Index ────────────────────────────────────────────────────
        public ActionResult Index()
        {
            var configuracionesVisuales = db.ConfiguracionesVisuales.Include(c => c.Imagen);
            return View(configuracionesVisuales.ToList());
        }

        // ── GET: Details ──────────────────────────────────────────────────
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ConfiguracionVisual configuracionVisual = db.ConfiguracionesVisuales.Find(id);
            if (configuracionVisual == null) return HttpNotFound();
            return View(configuracionVisual);
        }

        // ── GET: Create ───────────────────────────────────────────────────
        public ActionResult Create()
        {
            ViewBag.ImagenId = new SelectList(db.ImagenesCatalogo, "Id", "NombreArchivo");
            return View();
        }

        // ── GET: Edit ─────────────────────────────────────────────────────
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ConfiguracionVisual configuracionVisual = db.ConfiguracionesVisuales.Find(id);
            if (configuracionVisual == null) return HttpNotFound();
            ViewBag.ImagenId = new SelectList(db.ImagenesCatalogo, "Id", "NombreArchivo", configuracionVisual.ImagenId);
            return View(configuracionVisual);
        }

        // ── POST: Edit ────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ImagenId,CoordX,CoordY,Ancho,Alto,AnioAplicacion,MostrarEn,FechaInicio,FechaFin")] ConfiguracionVisual configuracionVisual)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configuracionVisual).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ImagenId = new SelectList(db.ImagenesCatalogo, "Id", "NombreArchivo", configuracionVisual.ImagenId);
            return View(configuracionVisual);
        }

        // ── GET: Delete ───────────────────────────────────────────────────
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ConfiguracionVisual configuracionVisual = db.ConfiguracionesVisuales.Find(id);
            if (configuracionVisual == null) return HttpNotFound();
            return View(configuracionVisual);
        }

        // ── POST: DeleteConfirmed ─────────────────────────────────────────
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConfiguracionVisual configuracionVisual = db.ConfiguracionesVisuales.Find(id);
            db.ConfiguracionesVisuales.Remove(configuracionVisual);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ═════════════════════════════════════════════════════════════════
        // AJAX — Obtener URL de imagen por Id
        // ═════════════════════════════════════════════════════════════════
        [HttpGet]
        public JsonResult GetImagenUrl(int id)
        {
            var imagen = db.ImagenesCatalogo.Find(id);
            if (imagen != null)
                return Json(new { url = Url.Content(imagen.UrlImagen) }, JsonRequestBehavior.AllowGet);

            return Json(new { url = "" }, JsonRequestBehavior.AllowGet);
        }

        // ═════════════════════════════════════════════════════════════════
        // AJAX — Obtener configuraciones existentes que se solapen en fechas
        //        Devuelve dbId e imagenId para poder hacer UPDATE después
        // ═════════════════════════════════════════════════════════════════
        [HttpGet]
        public JsonResult GetConfiguracionesExistentes(int anio, string mostrarEn, string inicio, string fin)
        {
            if (string.IsNullOrEmpty(inicio) || string.IsNullOrEmpty(fin))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            try
            {
                DateTime fInicio = DateTime.Parse(inicio);
                DateTime fFin = DateTime.Parse(fin);

                var existentes = db.ConfiguracionesVisuales
                    .Where(c => c.AnioAplicacion == anio && c.MostrarEn == mostrarEn)
                    .Where(c => fInicio <= c.FechaFin && fFin >= c.FechaInicio)
                    .Select(c => new
                    {
                        dbId = c.Id,                    // ID real en BD (para UPDATE)
                        imagenId = c.ImagenId,              // para re-identificar la imagen
                        url = c.Imagen.UrlImagen,
                        nombre = c.Imagen.NombreArchivo,
                        x = c.CoordX,
                        y = c.CoordY,
                        w = c.Ancho,
                        h = c.Alto
                    }).ToList();

                // Resolver Url.Content fuera del query de EF
                var resultado = existentes.Select(e => new
                {
                    e.dbId,
                    e.imagenId,
                    url = Url.Content(e.url),
                    e.nombre,
                    e.x,
                    e.y,
                    e.w,
                    e.h
                });

                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = "Formato de fecha o datos inválidos" }, JsonRequestBehavior.AllowGet);
            }
        }

        // ═════════════════════════════════════════════════════════════════
        // AJAX — Guardar sesión completa del lienzo
        //        nuevos     → INSERT
        //        existentes → UPDATE (solo coordenadas/tamaño)
        // ═════════════════════════════════════════════════════════════════
        [HttpPost]
        public JsonResult GuardarSesion(SesionPayload payload)
        {
            try
            {
                if (payload == null)
                    return Json(new { success = false, message = "Payload vacío." });

                // INSERT — elementos nuevos
                if (payload.Nuevos != null)
                {
                    foreach (var item in payload.Nuevos)
                    {
                        if (item.ImagenId > 0)
                            db.ConfiguracionesVisuales.Add(item);
                    }
                }

                // UPDATE — solo reposicionamiento de los existentes
                if (payload.Existentes != null)
                {
                    foreach (var item in payload.Existentes)
                    {
                        var original = db.ConfiguracionesVisuales.Find(item.Id);
                        if (original == null) continue;

                        original.CoordX = item.CoordX;
                        original.CoordY = item.CoordY;
                        original.Ancho = item.Ancho;
                        original.Alto = item.Alto;
                        // Intencionalmente NO se modifican: FechaInicio, FechaFin,
                        // AnioAplicacion, MostrarEn, ImagenId — solo la posición cambia
                    }
                }

                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ── Dispose ───────────────────────────────────────────────────────
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ═════════════════════════════════════════════════════════════════════
    // DTO — Payload del lienzo completo
    // ═════════════════════════════════════════════════════════════════════
    public class SesionPayload
    {
        public List<ConfiguracionVisual> Nuevos { get; set; }
        public List<ConfiguracionVisual> Existentes { get; set; }
    }
}