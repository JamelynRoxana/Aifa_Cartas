using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using cartas_aifa.Data;
using cartas_aifa.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace cartas_aifa.Controllers;

public class TramiteAcademicoController : Controller
{
    private readonly AppDbContext _db;

    public TramiteAcademicoController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var tramites = await _db.TramitesAcademicos
            .Include(t => t.Estudiante)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.Autoridad)
            .Include(t => t.TipoCarta)
            .ToListAsync();
        return View(tramites);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var tramite = await _db.TramitesAcademicos
            .Include(t => t.Estudiante)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.Autoridad)
            .Include(t => t.TipoCarta)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tramite == null) return NotFound();
        return View(tramite);
    }

    public async Task<IActionResult> Create()
    {
        await CargarViewBags();
        ViewBag.Estudiantes = await _db.Estudiantes
            .Include(e => e.Facultad).ThenInclude(f => f!.Universidad)
            .Include(e => e.DetalleEtapa)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TramiteAcademico tramite)
    {
        if (ModelState.IsValid)
        {
            tramite.FechaRegistro = DateTime.Now;
            _db.TramitesAcademicos.Add(tramite);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Registro creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        await CargarViewBags();
        ViewBag.Estudiantes = await _db.Estudiantes
            .Include(e => e.Facultad).ThenInclude(f => f!.Universidad)
            .Include(e => e.DetalleEtapa)
            .OrderBy(e => e.Nombre)
            .ToListAsync();
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View(tramite);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearMasivo(int IdAut, int IdTipoCarta, DateTime? FechaDocumento, DateTime? FechaInicioPeriodo, DateTime? FechaFinPeriodo, int? TotalHoras, List<int> Seleccionados, IFormCollection form)
    {
        if (Seleccionados == null || Seleccionados.Count == 0)
            return RedirectToAction(nameof(Create));

        var fechaDoc = FechaDocumento ?? DateTime.Now;

        foreach (var idEstudiante in Seleccionados)
        {
            var folio = form[$"Folios_{idEstudiante}"].ToString().Trim();
            if (string.IsNullOrEmpty(folio)) continue;

            // Obtener el IdDetalleEtapa del estudiante
            var estudiante = await _db.Estudiantes.FindAsync(idEstudiante);
            var idDetalleEtapa = estudiante?.IdDetalleEtapa ?? 1;

            var tramite = new TramiteAcademico
            {
                Folio = folio,
                IdEstudiante = idEstudiante,
                IdDetalleEtapa = idDetalleEtapa,
                IdAut = IdAut,
                IdTipoCarta = IdTipoCarta,
                FechaRegistro = fechaDoc,
                FechaInicioPeriodo = FechaInicioPeriodo,
                FechaFinPeriodo = FechaFinPeriodo,
                TotalHoras = TotalHoras
            };
            _db.TramitesAcademicos.Add(tramite);
        }

        await _db.SaveChangesAsync();
        TempData["Exito"] = "Registro creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var tramite = await _db.TramitesAcademicos.FindAsync(id);
        if (tramite == null) return NotFound();
        await CargarViewBags();
        return View(tramite);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TramiteAcademico tramite)
    {
        if (id != tramite.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _db.Update(tramite);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Registro actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        await CargarViewBags();
        return View(tramite);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var tramite = await _db.TramitesAcademicos
            .Include(t => t.Estudiante)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.Autoridad)
            .Include(t => t.TipoCarta)
            .FirstOrDefaultAsync(t => t.Id == id);
        if (tramite == null) return NotFound();
        return View(tramite);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var tramite = await _db.TramitesAcademicos.FindAsync(id);
        if (tramite != null)
        {
            _db.TramitesAcademicos.Remove(tramite);
            await _db.SaveChangesAsync();
        }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ImprimirCarta(int id)
    {
        var tramite = await _db.TramitesAcademicos
            .Include(t => t.Estudiante).ThenInclude(e => e!.Facultad).ThenInclude(f => f!.Universidad)
            .Include(t => t.Estudiante).ThenInclude(e => e!.Subdireccion)
            .Include(t => t.Estudiante).ThenInclude(e => e!.Direccion)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.Autoridad)
            .Include(t => t.TipoCarta)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tramite == null) return NotFound();

        // Obtener la etapa del estudiante para fechas y subdirección
        var etapa = await _db.EtapasAifa
            .Include(e => e.Direccion)
            .Include(e => e.Subdireccion)
            .Include(e => e.DetalleEtapa)
            .Where(e => e.IdEstudiante == tramite.IdEstudiante && e.IdDetalleEtapa == tramite.IdDetalleEtapa)
            .FirstOrDefaultAsync();

        // Obtener el director escolar de la facultad
        var director = await _db.DirectoresEscolares
            .Include(d => d.Facultad)
            .Where(d => d.IdF == tramite.Estudiante!.IdF)
            .FirstOrDefaultAsync();

        // Obtener configuraciones visuales activas para este tipo de carta
        var hoy = DateTime.Now;
        var tipoCarta2 = tramite.TipoCarta?.NombreCarta?.ToLower() ?? "";
        string mostrarEnFiltro;
        if (tipoCarta2.Contains("aceptacion") || tipoCarta2.Contains("aceptación"))
            mostrarEnFiltro = "Aceptacion";
        else
            mostrarEnFiltro = "Termino";
        
        var configsVisuales = await _db.ConfiguracionesVisuales
            .Include(c => c.Imagen)
            .Where(c => c.FechaInicio <= hoy && c.FechaFin >= hoy)
            .Where(c => c.MostrarEn == mostrarEnFiltro || c.MostrarEn == "Ambas" || c.MostrarEn == null)
            .ToListAsync();

        // Pre-cargar imágenes desde archivo local o URL
        var imagenesBytes = new Dictionary<int, byte[]>();
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // Obtener leyendas activas
        var leyendasActivas = await _db.Leyendas
            .Where(l => l.FechaInicio <= hoy && l.FechaFin >= hoy)
            .Where(l => l.MostrarEn == mostrarEnFiltro || l.MostrarEn == "Ambas" || l.MostrarEn == null)
            .ToListAsync();

        // Obtener pies de página activos
        var piesDePagina = await _db.PiesDePagina
            .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy)
            .Where(p => p.MostrarEn == mostrarEnFiltro || p.MostrarEn == "Ambas" || p.MostrarEn == null)
            .ToListAsync();
        using var httpClient = new HttpClient();
        foreach (var config in configsVisuales)
        {
            var url = config.Imagen?.UrlImagen;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    byte[] bytes;
                    if (url.StartsWith("http://") || url.StartsWith("https://"))
                    {
                        bytes = await httpClient.GetByteArrayAsync(url);
                    }
                    else
                    {
                        // Ruta local relativa a wwwroot
                        var filePath = Path.Combine(webRootPath, url.TrimStart('/'));
                        bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    }
                    imagenesBytes[config.Id] = bytes;
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
            }
        }

        var nombreEstudiante = tramite.Estudiante?.NombreCompleto ?? "N/A";
        var matricula = tramite.Estudiante?.Matricula ?? "N/A";
        var carrera = tramite.Estudiante?.Carrera ?? "N/A";
        var planEstudios = tramite.Estudiante?.PlanEstudios?.ToLower() ?? "";
        var periodoActual = tramite.Estudiante?.PeriodoActual;
        var periodoTexto = ObtenerOrdinal(periodoActual) + " " + ObtenerNombrePeriodo(planEstudios);
        var facultad = tramite.Estudiante?.Facultad?.NombreF ?? "N/A";
        var universidad = tramite.Estudiante?.Facultad?.Universidad?.NombreU ?? "N/A";
        var tipoEtapa = tramite.DetalleEtapa?.TipoEtapa ?? "N/A";
        var tipoCarta = tramite.TipoCarta?.NombreCarta ?? "Carta";
        var folio = tramite.Folio;
        var direccion = tramite.Estudiante?.Direccion?.NombreDir ?? etapa?.Direccion?.NombreDir ?? "Dirección de Administración";
        var subdireccion = tramite.Estudiante?.Subdireccion?.NombreSub ?? etapa?.Subdireccion?.NombreSub ?? "Subdirección de Recursos Humanos";
        var fechaInicio = tramite.FechaInicioPeriodo ?? etapa?.FechaInicio ?? tramite.FechaRegistro;
        var fechaFin = tramite.FechaFinPeriodo ?? etapa?.FechaFin ?? tramite.FechaRegistro;
        var autoridad = tramite.Autoridad;
        var directorNombre = director != null ? $"{director.Rango} {director.Nombre}" : "";
        var directorPuesto = director?.Puesto ?? "";
        var culturaEs = new CultureInfo("es-MX");

        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(12));

                var contentPaddingLeft = 99f;
                var contentPaddingRight = 28f;
                var contentPaddingTop = 57f;

                page.Header().PaddingLeft(contentPaddingLeft).PaddingRight(contentPaddingRight).PaddingTop(contentPaddingTop).Column(header =>
                {
                    foreach (var ley in leyendasActivas)
                    {
                        var lineas = ley.Texto.Split('\n');
                        foreach (var linea in lineas)
                        {
                            var lineItem = header.Item();
                            if (ley.Alineacion == "Centrado") lineItem = lineItem.AlignCenter();
                            else if (ley.Alineacion == "Derecha") lineItem = lineItem.AlignRight();
                            var textBlock = lineItem.Text(linea.Trim()).FontSize(ley.TamanoFuente);
                            if (ley.EstiloFuente == "Bold") textBlock.Bold();
                            else if (ley.EstiloFuente == "Italic") textBlock.Italic();
                        }
                        header.Item().PaddingBottom(5);
                    }

                    header.Item().AlignRight().Column(right =>
                    {
                        right.Item().AlignRight().Text(direccion);
                        right.Item().AlignRight().Text(subdireccion);
                        right.Item().AlignRight().Text($"No. {folio}").Bold();
                    });

                    header.Item().PaddingTop(15).Column(asunto =>
                    {
                        asunto.Item().AlignRight().Text($"Asunto: {tipoCarta} para prestación de");
                        asunto.Item().AlignRight().Text($"{tipoEtapa}.");
                    });

                    header.Item().PaddingTop(15).AlignRight()
                        .Text($"Zumpango, Edo. Méx., a {tramite.FechaRegistro.ToString("dd 'de' MMMM 'de' yyyy", culturaEs)}.");
                });

                // Imágenes de FONDO (detrás del texto)
                page.Background().Layers(layers =>
                {
                    layers.PrimaryLayer(); // capa principal requerida
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Fondo" || c.Posicion == null))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var pageW = 612f;
                        var pageH = 792f;
                        var xPt = (float)(config.CoordX / 100m) * pageW;
                        var yPt = (float)(config.CoordY / 100m) * pageH;
                        var wPt = (float)(config.Ancho / 100m) * pageW;
                        var hPt = (float)(config.Alto / 100m) * pageH;

                        // Aplicar opacidad: 0 = normal, 100 = totalmente transparente
                        var imgBytes = imagenesBytes[config.Id];
                        if (config.Opacidad > 0)
                        {
                            using var original = SKBitmap.Decode(imgBytes);
                            using var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
                            var skCanvas = surface.Canvas;
                            skCanvas.Clear(SKColors.Transparent);
                            var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(255 * (1f - config.Opacidad / 100f))) };
                            skCanvas.DrawBitmap(original, 0, 0, paint);
                            using var img = surface.Snapshot();
                            using var data = img.Encode(SKEncodedImageFormat.Png, 90);
                            imgBytes = data.ToArray();
                        }
                        
                        layers.Layer()
                            .AlignLeft()
                            .AlignTop()
                            .PaddingLeft(xPt)
                            .PaddingTop(yPt)
                            .Width(wPt)
                            .Height(hPt)
                            .Image(imgBytes)
                            .FitArea();
                    }
                });

                page.Content().PaddingLeft(contentPaddingLeft).PaddingRight(contentPaddingRight).PaddingTop(20).Column(col =>
                {

                    // Destinatario (Director Escolar)
                    if (director != null)
                    {
                        col.Item().Column(dest =>
                        {
                            dest.Item().Text(directorNombre).Bold();
                            dest.Item().Text(directorPuesto).Bold();
                            dest.Item().Text(universidad).Bold();
                            dest.Item().Text("Presente.");
                        });
                    }

                    // Cuerpo de la carta
                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Justify();
                        text.Span("        Por medio del presente, me permito comunicarle que la C. ");
                        text.Span(nombreEstudiante).Bold();
                        text.Span(" con número de matrícula ");
                        text.Span(matricula).Bold();
                        text.Span(", adscrita al ");
                        text.Span(periodoTexto).Bold();
                        text.Span(" de la carrera en ");
                        text.Span(carrera).Bold();
                        text.Span($" en esa institución, ha concluido satisfactoriamente su prestación en el programa de ");
                        text.Span(tipoEtapa).Bold();
                        text.Span($" en la {subdireccion} del Aeropuerto Internacional Felipe Ángeles, S.A. de C.V., cubriendo un total de {(tramite.TotalHoras ?? 200)} horas durante el periodo comprendido del ");
                        text.Span(fechaInicio.ToString(fechaInicio.Year == fechaFin.Year ? "dd 'de' MMMM" : "dd 'de' MMMM 'de' yyyy", culturaEs)).Bold();
                        text.Span(" al ");
                        text.Span(fechaFin.ToString("dd 'de' MMMM 'de' yyyy", culturaEs)).Bold();
                        text.Span(", de acuerdo con lo solicitado.");
                    });

                    // Despedida
                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Justify();
                        text.Span("        Sin otro en particular, aprovechando la ocasión para enviarle un cordial saludo.");
                    });

                    // Atentamente
                    col.Item().PaddingTop(30).Text("Atentamente.");

                    // Firma de la autoridad
                    col.Item().PaddingTop(40).Column(firma =>
                    {
                        firma.Item().PaddingTop(4).Text($"{autoridad?.Rango ?? ""} {autoridad?.Nombre ?? "N/A"}")
                            .Bold();
                        firma.Item().Text(autoridad?.Puesto ?? "");
                    });

                    // CCP
                    col.Item().PaddingTop(120).Text("c.c.p. la Coordinación de Empleo; debiendo integrar el expediente de la interesada. –Presente.")
                        .FontSize(11);

                });

                // Foreground: imágenes sólidas
                page.Foreground().Layers(layers =>
                {
                    layers.PrimaryLayer(); // capa principal requerida

                    // Imágenes sólidas en su posición fija
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Solida"))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var pageW = 612f;
                        var pageH = 792f;
                        var xPt = (float)(config.CoordX / 100m) * pageW;
                        var yPt = (float)(config.CoordY / 100m) * pageH;
                        var wPt = (float)(config.Ancho / 100m) * pageW;
                        var hPt = (float)(config.Alto / 100m) * pageH;
                        
                        layers.Layer()
                            .AlignLeft()
                            .AlignTop()
                            .PaddingLeft(xPt)
                            .PaddingTop(yPt)
                            .Width(wPt)
                            .Height(hPt)
                            .Image(imagenesBytes[config.Id])
                            .FitArea();
                    }
                });

                // Footer: pies de página activos (sobresale 1cm a cada lado)
                page.Footer().ExtendHorizontal().PaddingHorizontal(-28.35f).PaddingBottom(14f).Column(footer =>
                {
                    foreach (var pie in piesDePagina)
                    {
                        if (pie.TieneLinea)
                        {
                            // Recuadro con color de fondo, borde del mismo color, texto dentro
                            footer.Item().Background(pie.ColorFondo ?? "#0c2f57")
                                .Border(pie.GrosorLinea)
                                .BorderColor(pie.ColorFondo ?? "#0c2f57")
                                .Padding(4)
                                .Text(text =>
                                {
                                    if (pie.Alineacion == "Centrado") text.AlignCenter();
                                    else if (pie.Alineacion == "Derecha") text.AlignRight();
                                    text.Span(pie.Texto).FontSize(pie.TamanoFuente).FontColor(pie.ColorLetra ?? "#ffffff");
                                });
                        }
                        else
                        {
                            var footerItem = footer.Item().Padding(4);
                            footerItem.Text(text =>
                            {
                                if (pie.Alineacion == "Centrado") text.AlignCenter();
                                else if (pie.Alineacion == "Derecha") text.AlignRight();
                                text.Span(pie.Texto).FontSize(pie.TamanoFuente).FontColor(pie.ColorLetra ?? "#000000");
                            });
                        }
                    }
                });
            });
        });

        var stream = new MemoryStream();
        pdf.GeneratePdf(stream);
        stream.Position = 0;

        return File(stream, "application/pdf", $"Carta_{tramite.Folio?.Replace("/", "-")}.pdf");
    }

    public async Task<IActionResult> ImprimirTodos()
    {
        var ids = await _db.TramitesAcademicos.Select(t => t.Id).ToListAsync();
        return await GenerarPdfMultiple(ids);
    }

    public async Task<IActionResult> ImprimirVarios(string ids)
    {
        if (string.IsNullOrWhiteSpace(ids)) return NotFound();
        var listaIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s.Trim(), out var n) ? n : 0)
            .Where(n => n > 0)
            .ToList();
        if (listaIds.Count == 0) return NotFound();
        return await GenerarPdfMultiple(listaIds);
    }

    private async Task<IActionResult> GenerarPdfMultiple(List<int> ids)
    {
        if (ids.Count == 1)
            return RedirectToAction(nameof(ImprimirCarta), new { id = ids[0] });

        // Generar cada PDF individualmente y combinar páginas
        var pdfBytes = new List<byte[]>();
        foreach (var id in ids)
        {
            var bytes = await GenerarPdfBytes(id);
            if (bytes != null) pdfBytes.Add(bytes);
        }

        if (pdfBytes.Count == 0) return NotFound();
        if (pdfBytes.Count == 1)
            return File(pdfBytes[0], "application/pdf", $"Cartas_AIFA_{DateTime.Now:yyyyMMdd}.pdf");

        // Combinar PDFs usando PdfSharpCore
        var mergedDoc = new PdfSharpCore.Pdf.PdfDocument();
        foreach (var pdf in pdfBytes)
        {
            using var ms = new MemoryStream(pdf);
            var inputDoc = PdfSharpCore.Pdf.IO.PdfReader.Open(ms, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);
            for (int i = 0; i < inputDoc.PageCount; i++)
            {
                mergedDoc.AddPage(inputDoc.Pages[i]);
            }
        }

        using var outputStream = new MemoryStream();
        mergedDoc.Save(outputStream, false);
        return File(outputStream.ToArray(), "application/pdf", $"Cartas_AIFA_{DateTime.Now:yyyyMMdd}.pdf");
    }

    /// <summary>Genera los bytes del PDF de un trámite individual (misma lógica que ImprimirCarta)</summary>
    private async Task<byte[]?> GenerarPdfBytes(int id)
    {
        var tramite = await _db.TramitesAcademicos
            .Include(t => t.Estudiante).ThenInclude(e => e!.Facultad).ThenInclude(f => f!.Universidad)
            .Include(t => t.Estudiante).ThenInclude(e => e!.Subdireccion)
            .Include(t => t.Estudiante).ThenInclude(e => e!.Direccion)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.Autoridad)
            .Include(t => t.TipoCarta)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tramite == null) return null;

        var etapa = await _db.EtapasAifa
            .Include(e => e.Direccion)
            .Include(e => e.Subdireccion)
            .Include(e => e.DetalleEtapa)
            .Where(e => e.IdEstudiante == tramite.IdEstudiante && e.IdDetalleEtapa == tramite.IdDetalleEtapa)
            .FirstOrDefaultAsync();

        var director = await _db.DirectoresEscolares
            .Include(d => d.Facultad)
            .Where(d => d.IdF == tramite.Estudiante!.IdF)
            .FirstOrDefaultAsync();

        var hoy = DateTime.Now;
        var tipoCarta2 = tramite.TipoCarta?.NombreCarta?.ToLower() ?? "";
        string mostrarEnFiltro = (tipoCarta2.Contains("aceptacion") || tipoCarta2.Contains("aceptación")) ? "Aceptacion" : "Termino";

        var configsVisuales = await _db.ConfiguracionesVisuales
            .Include(c => c.Imagen)
            .Where(c => c.FechaInicio <= hoy && c.FechaFin >= hoy)
            .Where(c => c.MostrarEn == mostrarEnFiltro || c.MostrarEn == "Ambas" || c.MostrarEn == null)
            .ToListAsync();

        var imagenesBytes = new Dictionary<int, byte[]>();
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var leyendasActivas = await _db.Leyendas
            .Where(l => l.FechaInicio <= hoy && l.FechaFin >= hoy)
            .Where(l => l.MostrarEn == mostrarEnFiltro || l.MostrarEn == "Ambas" || l.MostrarEn == null)
            .ToListAsync();

        var piesDePagina = await _db.PiesDePagina
            .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy)
            .Where(p => p.MostrarEn == mostrarEnFiltro || p.MostrarEn == "Ambas" || p.MostrarEn == null)
            .ToListAsync();

        using var httpClient = new HttpClient();
        foreach (var config in configsVisuales)
        {
            var url = config.Imagen?.UrlImagen;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    byte[] bytes;
                    if (url.StartsWith("http://") || url.StartsWith("https://"))
                        bytes = await httpClient.GetByteArrayAsync(url);
                    else
                        bytes = await System.IO.File.ReadAllBytesAsync(Path.Combine(webRootPath, url.TrimStart('/')));
                    imagenesBytes[config.Id] = bytes;
                }
                catch { }
            }
        }

        var nombreEstudiante = tramite.Estudiante?.NombreCompleto ?? "N/A";
        var matricula = tramite.Estudiante?.Matricula ?? "N/A";
        var carrera = tramite.Estudiante?.Carrera ?? "N/A";
        var planEstudios = tramite.Estudiante?.PlanEstudios?.ToLower() ?? "";
        var periodoActual = tramite.Estudiante?.PeriodoActual;
        var periodoTexto = ObtenerOrdinal(periodoActual) + " " + ObtenerNombrePeriodo(planEstudios);
        var universidad = tramite.Estudiante?.Facultad?.Universidad?.NombreU ?? "N/A";
        var tipoEtapa = tramite.DetalleEtapa?.TipoEtapa ?? "N/A";
        var tipoCarta = tramite.TipoCarta?.NombreCarta ?? "Carta";
        var folio = tramite.Folio;
        var direccion = tramite.Estudiante?.Direccion?.NombreDir ?? etapa?.Direccion?.NombreDir ?? "Dirección de Administración";
        var subdireccion = tramite.Estudiante?.Subdireccion?.NombreSub ?? etapa?.Subdireccion?.NombreSub ?? "Subdirección de Recursos Humanos";
        var fechaInicio = tramite.FechaInicioPeriodo ?? etapa?.FechaInicio ?? tramite.FechaRegistro;
        var fechaFin = tramite.FechaFinPeriodo ?? etapa?.FechaFin ?? tramite.FechaRegistro;
        var autoridad = tramite.Autoridad;
        var directorNombre = director != null ? $"{director.Rango} {director.Nombre}" : "";
        var directorPuesto = director?.Puesto ?? "";
        var culturaEs = new CultureInfo("es-MX");

        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(12));

                var contentPaddingLeft = 99f;
                var contentPaddingRight = 28f;
                var contentPaddingTop = 57f;

                page.Header().PaddingLeft(contentPaddingLeft).PaddingRight(contentPaddingRight).PaddingTop(contentPaddingTop).Column(header =>
                {
                    foreach (var ley in leyendasActivas)
                    {
                        var lineas = ley.Texto.Split('\n');
                        foreach (var linea in lineas)
                        {
                            var lineItem = header.Item();
                            if (ley.Alineacion == "Centrado") lineItem = lineItem.AlignCenter();
                            else if (ley.Alineacion == "Derecha") lineItem = lineItem.AlignRight();
                            var textBlock = lineItem.Text(linea.Trim()).FontSize(ley.TamanoFuente);
                            if (ley.EstiloFuente == "Bold") textBlock.Bold();
                            else if (ley.EstiloFuente == "Italic") textBlock.Italic();
                        }
                        header.Item().PaddingBottom(5);
                    }

                    header.Item().AlignRight().Column(right =>
                    {
                        right.Item().AlignRight().Text(direccion);
                        right.Item().AlignRight().Text(subdireccion);
                        right.Item().AlignRight().Text($"No. {folio}").Bold();
                    });

                    header.Item().PaddingTop(15).Column(asunto =>
                    {
                        asunto.Item().AlignRight().Text($"Asunto: {tipoCarta} para prestación de");
                        asunto.Item().AlignRight().Text($"{tipoEtapa}.");
                    });

                    header.Item().PaddingTop(15).AlignRight()
                        .Text($"Zumpango, Edo. Méx., a {tramite.FechaRegistro.ToString("dd 'de' MMMM 'de' yyyy", culturaEs)}.");
                });

                page.Background().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Fondo" || c.Posicion == null))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var pageW = 612f; var pageH = 792f;
                        var xPt = (float)(config.CoordX / 100m) * pageW;
                        var yPt = (float)(config.CoordY / 100m) * pageH;
                        var wPt = (float)(config.Ancho / 100m) * pageW;
                        var hPt = (float)(config.Alto / 100m) * pageH;

                        var imgBytes = imagenesBytes[config.Id];
                        if (config.Opacidad > 0)
                        {
                            using var original = SKBitmap.Decode(imgBytes);
                            using var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
                            var skCanvas = surface.Canvas;
                            skCanvas.Clear(SKColors.Transparent);
                            var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)(255 * (1f - config.Opacidad / 100f))) };
                            skCanvas.DrawBitmap(original, 0, 0, paint);
                            using var img = surface.Snapshot();
                            using var data = img.Encode(SKEncodedImageFormat.Png, 90);
                            imgBytes = data.ToArray();
                        }

                        layers.Layer().AlignLeft().AlignTop().PaddingLeft(xPt).PaddingTop(yPt).Width(wPt).Height(hPt).Image(imgBytes).FitArea();
                    }
                });

                page.Content().PaddingLeft(contentPaddingLeft).PaddingRight(contentPaddingRight).PaddingTop(20).Column(col =>
                {
                    if (director != null)
                    {
                        col.Item().Column(dest =>
                        {
                            dest.Item().Text(directorNombre).Bold();
                            dest.Item().Text(directorPuesto).Bold();
                            dest.Item().Text(universidad).Bold();
                            dest.Item().Text("Presente.");
                        });
                    }

                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Justify();
                        text.Span("        Por medio del presente, me permito comunicarle que la C. ");
                        text.Span(nombreEstudiante).Bold();
                        text.Span(" con número de matrícula ");
                        text.Span(matricula).Bold();
                        text.Span(", adscrita al ");
                        text.Span(periodoTexto).Bold();
                        text.Span(" de la carrera en ");
                        text.Span(carrera).Bold();
                        text.Span($" en esa institución, ha concluido satisfactoriamente su prestación en el programa de ");
                        text.Span(tipoEtapa).Bold();
                        text.Span($" en la {subdireccion} del Aeropuerto Internacional Felipe Ángeles, S.A. de C.V., cubriendo un total de {(tramite.TotalHoras ?? 200)} horas durante el periodo comprendido del ");
                        text.Span(fechaInicio.ToString(fechaInicio.Year == fechaFin.Year ? "dd 'de' MMMM" : "dd 'de' MMMM 'de' yyyy", culturaEs)).Bold();
                        text.Span(" al ");
                        text.Span(fechaFin.ToString("dd 'de' MMMM 'de' yyyy", culturaEs)).Bold();
                        text.Span(", de acuerdo con lo solicitado.");
                    });

                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Justify();
                        text.Span("        Sin otro en particular, aprovechando la ocasión para enviarle un cordial saludo.");
                    });

                    col.Item().PaddingTop(30).Text("Atentamente.");

                    col.Item().PaddingTop(40).Column(firma =>
                    {
                        firma.Item().PaddingTop(4).Text($"{autoridad?.Rango ?? ""} {autoridad?.Nombre ?? "N/A"}").Bold();
                        firma.Item().Text(autoridad?.Puesto ?? "");
                    });

                    col.Item().PaddingTop(120).Text("c.c.p. la Coordinación de Empleo; debiendo integrar el expediente de la interesada. –Presente.").FontSize(11);
                });

                page.Foreground().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Solida"))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var pageW = 612f; var pageH = 792f;
                        var xPt = (float)(config.CoordX / 100m) * pageW;
                        var yPt = (float)(config.CoordY / 100m) * pageH;
                        var wPt = (float)(config.Ancho / 100m) * pageW;
                        var hPt = (float)(config.Alto / 100m) * pageH;
                        layers.Layer().AlignLeft().AlignTop().PaddingLeft(xPt).PaddingTop(yPt).Width(wPt).Height(hPt).Image(imagenesBytes[config.Id]).FitArea();
                    }
                });

                page.Footer().ExtendHorizontal().PaddingHorizontal(-28.35f).PaddingBottom(14f).Column(footer =>
                {
                    foreach (var pie in piesDePagina)
                    {
                        if (pie.TieneLinea)
                        {
                            footer.Item().Background(pie.ColorFondo ?? "#0c2f57").Border(pie.GrosorLinea).BorderColor(pie.ColorFondo ?? "#0c2f57").Padding(4).Text(text =>
                            {
                                if (pie.Alineacion == "Centrado") text.AlignCenter();
                                else if (pie.Alineacion == "Derecha") text.AlignRight();
                                text.Span(pie.Texto).FontSize(pie.TamanoFuente).FontColor(pie.ColorLetra ?? "#ffffff");
                            });
                        }
                        else
                        {
                            footer.Item().Padding(4).Text(text =>
                            {
                                if (pie.Alineacion == "Centrado") text.AlignCenter();
                                else if (pie.Alineacion == "Derecha") text.AlignRight();
                                text.Span(pie.Texto).FontSize(pie.TamanoFuente).FontColor(pie.ColorLetra ?? "#000000");
                            });
                        }
                    }
                });
            });
        });

        var stream = new MemoryStream();
        pdf.GeneratePdf(stream);
        return stream.ToArray();
    }

    private async Task CargarViewBags()
    {
        ViewBag.IdEstudiante = new SelectList(await _db.Estudiantes.ToListAsync(), "Id", "NombreCompleto");
        ViewBag.IdDetalleEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        ViewBag.IdAut = new SelectList(await _db.AutoridadesAifa.ToListAsync(), "Id", "Nombre");
        ViewBag.IdTipoCarta = new SelectList(await _db.DetallesTipoCartas.ToListAsync(), "Id", "NombreCarta");
    }

    private static string ObtenerOrdinal(int? numero)
    {
        if (numero == null || numero <= 0) return "";
        return numero switch
        {
            1 => "primer",
            2 => "segundo",
            3 => "tercer",
            4 => "cuarto",
            5 => "quinto",
            6 => "sexto",
            7 => "séptimo",
            8 => "octavo",
            9 => "noveno",
            10 => "décimo",
            11 => "undécimo",
            12 => "duodécimo",
            13 => "decimotercer",
            14 => "decimocuarto",
            15 => "decimoquinto",
            16 => "decimosexto",
            17 => "decimoséptimo",
            18 => "decimoctavo",
            19 => "decimonoveno",
            20 => "vigésimo",
            21 => "vigésimo primer",
            22 => "vigésimo segundo",
            _ => $"{numero}°"
        };
    }

    private static string ObtenerNombrePeriodo(string? plan)
    {
        if (string.IsNullOrEmpty(plan)) return "periodo";
        return plan.ToLower() switch
        {
            "bimestral" => "bimestre",
            "trimestral" => "trimestre",
            "cuatrimestral" => "cuatrimestre",
            "semestral" => "semestre",
            "anual" => "año",
            _ => "periodo"
        };
    }
}
