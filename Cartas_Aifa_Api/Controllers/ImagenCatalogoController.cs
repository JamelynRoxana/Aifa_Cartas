using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cartas_Aifa_Api.Models;

namespace Cartas_Aifa_Api.Controllers
{
    public class ImagenCatalogoController : Controller
    {
        private Model1 db = new Model1();

        // GET: ImagenCatalogo
        public ActionResult Index()
        {
            return View(db.ImagenesCatalogo.ToList());
        }

        // GET: ImagenCatalogo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenCatalogo imagenCatalogo = db.ImagenesCatalogo.Find(id);
            if (imagenCatalogo == null)
            {
                return HttpNotFound();
            }
            return View(imagenCatalogo);
        }

        // GET: ImagenCatalogo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ImagenCatalogo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NombreArchivo")] ImagenCatalogo imagenCatalogo, HttpPostedFileBase archivoImagen)
        {
            if (ModelState.IsValid)
            {
                if (archivoImagen != null && archivoImagen.ContentLength > 0)
                {
                    try
                    {
                        string carpeta = "~/Uploads/Logos/";
                        string rutaFisica = Server.MapPath(carpeta);

                        if (!Directory.Exists(rutaFisica))
                        {
                            Directory.CreateDirectory(rutaFisica);
                        }

                        string extension = Path.GetExtension(archivoImagen.FileName);
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + extension;
                        string rutaCompleta = Path.Combine(rutaFisica, nombreArchivoUnico);

                        archivoImagen.SaveAs(rutaCompleta);

                        imagenCatalogo.UrlImagen = "/Uploads/Logos/" + nombreArchivoUnico;
                        imagenCatalogo.FechaCarga = DateTime.Now;

                        db.ImagenesCatalogo.Add(imagenCatalogo);
                        db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error al guardar el archivo: " + ex.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Por favor seleccione una imagen.");
                }
            }

            return View(imagenCatalogo);
        }

        // GET: ImagenCatalogo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenCatalogo imagenCatalogo = db.ImagenesCatalogo.Find(id);
            if (imagenCatalogo == null)
            {
                return HttpNotFound();
            }
            return View(imagenCatalogo);
        }

        // POST: ImagenCatalogo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ACTUALIZADO: Agregamos HttpPostedFileBase archivoImagen para recibir el archivo nuevo
        public ActionResult Edit([Bind(Include = "Id,NombreArchivo,UrlImagen,FechaCarga")] ImagenCatalogo imagenCatalogo, HttpPostedFileBase archivoImagen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Si el usuario seleccionó un archivo nuevo para reemplazar el anterior
                    if (archivoImagen != null && archivoImagen.ContentLength > 0)
                    {
                        string carpeta = "~/Uploads/Logos/";
                        string rutaFisicaCarpeta = Server.MapPath(carpeta);

                        // 1. Borrar el archivo viejo de la carpeta para no dejar basura
                        if (!string.IsNullOrEmpty(imagenCatalogo.UrlImagen))
                        {
                            string rutaViejaFisica = Server.MapPath(imagenCatalogo.UrlImagen);
                            if (System.IO.File.Exists(rutaViejaFisica))
                            {
                                System.IO.File.Delete(rutaViejaFisica);
                            }
                        }

                        // 2. Guardar el archivo nuevo
                        string extension = Path.GetExtension(archivoImagen.FileName);
                        string nombreArchivoUnico = Guid.NewGuid().ToString() + extension;
                        string rutaCompleta = Path.Combine(rutaFisicaCarpeta, nombreArchivoUnico);

                        archivoImagen.SaveAs(rutaCompleta);

                        // 3. Actualizar la URL en el modelo que se guarda en la BD
                        imagenCatalogo.UrlImagen = "/Uploads/Logos/" + nombreArchivoUnico;
                    }

                    db.Entry(imagenCatalogo).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar: " + ex.Message);
                }
            }
            return View(imagenCatalogo);
        }

        // GET: ImagenCatalogo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ImagenCatalogo imagenCatalogo = db.ImagenesCatalogo.Find(id);
            if (imagenCatalogo == null)
            {
                return HttpNotFound();
            }
            return View(imagenCatalogo);
        }

        // POST: ImagenCatalogo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ImagenCatalogo imagenCatalogo = db.ImagenesCatalogo.Find(id);

            if (imagenCatalogo != null)
            {
                // Borrar el archivo físico de la carpeta
                if (!string.IsNullOrEmpty(imagenCatalogo.UrlImagen))
                {
                    string rutaFisica = Server.MapPath(imagenCatalogo.UrlImagen);
                    if (System.IO.File.Exists(rutaFisica))
                    {
                        System.IO.File.Delete(rutaFisica);
                    }
                }

                db.ImagenesCatalogo.Remove(imagenCatalogo);
                db.SaveChanges();
            }

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