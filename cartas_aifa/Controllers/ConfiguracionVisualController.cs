using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace cartas_aifa.Controllers;

public class ConfiguracionVisualController : Controller
{
    private readonly AppDbContext _db;
    public ConfiguracionVisualController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar, string? mostrarEn)
    {
        var query = _db.ConfiguracionesVisuales.Include(c => c.Imagen).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(c => c.Imagen != null && c.Imagen.NombreArchivo.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(mostrarEn))
        {
            query = query.Where(c => c.MostrarEn == mostrarEn);
        }

        ViewBag.BuscarActual = buscar;
        ViewBag.MostrarEnActual = mostrarEn;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.ConfiguracionesVisuales.Include(c => c.Imagen).FirstOrDefaultAsync(c => c.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await CargarViewBags();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ConfiguracionVisual item)
    {
        if (ModelState.IsValid)
        {
            _db.ConfiguracionesVisuales.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await CargarViewBags();
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.ConfiguracionesVisuales.FindAsync(id);
        if (item == null) return NotFound();
        await CargarViewBags(id.Value);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ConfiguracionVisual item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _db.Update(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        await CargarViewBags(id);
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.ConfiguracionesVisuales.Include(c => c.Imagen).FirstOrDefaultAsync(c => c.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.ConfiguracionesVisuales.FindAsync(id);
        if (item != null) { _db.ConfiguracionesVisuales.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> PreviewImage(int? excludeId)
    {
        var hoy = DateTime.Now;
        var configsVisuales = await _db.ConfiguracionesVisuales.Include(c => c.Imagen)
            .Where(c => c.FechaInicio <= hoy && c.FechaFin >= hoy)
            .Where(c => excludeId == null || c.Id != excludeId)
            .ToListAsync();
        var leyendasActivas = await _db.Leyendas.Where(l => l.FechaInicio <= hoy && l.FechaFin >= hoy).ToListAsync();
        var piesDePagina = await _db.PiesDePagina.Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy).ToListAsync();

        // Usar datos del primer trámite real
        var tramite = await _db.TramitesAcademicos
            .Include(t => t.Estudiante).ThenInclude(e => e!.Facultad).ThenInclude(f => f!.Universidad)
            .Include(t => t.DetalleEtapa).Include(t => t.Autoridad).Include(t => t.TipoCarta)
            .FirstOrDefaultAsync();

        var etapa = tramite != null ? await _db.EtapasAifa.Include(e => e.Direccion).Include(e => e.Subdireccion)
            .Where(e => e.IdEstudiante == tramite.IdEstudiante && e.IdDetalleEtapa == tramite.IdDetalleEtapa).FirstOrDefaultAsync() : null;
        var director = tramite != null ? await _db.DirectoresEscolares
            .Where(d => d.IdF == tramite.Estudiante!.IdF).FirstOrDefaultAsync() : null;

        var nombreEstudiante = tramite?.Estudiante?.NombreCompleto ?? "Nombre Estudiante Ejemplo";
        var matricula = tramite?.Estudiante?.Matricula ?? "1234567890";
        var carrera = tramite?.Estudiante?.Carrera ?? "Ingeniería en Software";
        var universidad = tramite?.Estudiante?.Facultad?.Universidad?.NombreU ?? "Universidad Ejemplo";
        var tipoEtapa = tramite?.DetalleEtapa?.TipoEtapa ?? "Estancia II";
        var tipoCarta = tramite?.TipoCarta?.NombreCarta ?? "Carta de finalización";
        var folio = tramite?.Folio ?? "AIFA/DA/SRH/EJEMPLO-001";
        var direccion = etapa?.Direccion?.NombreDir ?? "Dirección de administración";
        var subdireccion = etapa?.Subdireccion?.NombreSub ?? "Subdirección de Recursos Humanos";
        var fechaInicio = etapa?.FechaInicio ?? DateTime.Now;
        var fechaFin = etapa?.FechaFin ?? DateTime.Now;
        var directorNombre = director != null ? $"{director.Rango} {director.Nombre}" : "Dr. Director Ejemplo";
        var directorPuesto = director?.Puesto ?? "Director de División";
        var autoridadNombre = tramite?.Autoridad != null ? $"{tramite.Autoridad.Rango} {tramite.Autoridad.Nombre}" : "Lic. Autoridad Ejemplo";
        var autoridadPuesto = tramite?.Autoridad?.Puesto ?? "Sub de Recursos Humanos del AIFA, S.A de C.V";

        var imagenesBytes = new Dictionary<int, byte[]>();
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        using var httpClient = new HttpClient();
        foreach (var config in configsVisuales)
        {
            var url = config.Imagen?.UrlImagen;
            if (string.IsNullOrEmpty(url)) continue;
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

        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(12));

                var cPL = 99f; var cPR = 28f; var cPT = 57f;

                page.Background().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Fondo" || c.Posicion == null))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var imgBytes = imagenesBytes[config.Id];
                        if (config.Opacidad < 100)
                        {
                            using var original = SKBitmap.Decode(imgBytes);
                            using var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
                            var skCanvas = surface.Canvas;
                            skCanvas.Clear(SKColors.Transparent);
                            var paint = new SKPaint { Color = new SKColor(255, 255, 255, (byte)(255 * config.Opacidad / 100f)) };
                            skCanvas.DrawBitmap(original, 0, 0, paint);
                            using var img = surface.Snapshot();
                            using var data = img.Encode(SKEncodedImageFormat.Png, 90);
                            imgBytes = data.ToArray();
                        }
                        var xPt = (float)config.CoordX / 100f * 612f;
                        var yPt = (float)config.CoordY / 100f * 792f;
                        var wPt = (float)config.Ancho / 100f * 612f;
                        var hPt = (float)config.Alto / 100f * 792f;
                        layers.Layer().AlignLeft().AlignTop()
                            .PaddingLeft(xPt)
                            .PaddingTop(yPt)
                            .Width(wPt)
                            .Height(hPt)
                            .Image(imgBytes).FitArea();
                    }
                });

                page.Header().PaddingLeft(cPL).PaddingRight(cPR).PaddingTop(cPT).Column(header =>
                {
                    foreach (var ley in leyendasActivas)
                    {
                        var lineas = ley.Texto.Split('\n');
                        foreach (var linea in lineas)
                        {
                            var lineItem = header.Item();
                            if (ley.Alineacion == "Centrado") lineItem = lineItem.AlignCenter();
                            else if (ley.Alineacion == "Derecha") lineItem = lineItem.AlignRight();
                            var tb = lineItem.Text(linea.Trim()).FontSize(ley.TamanoFuente);
                            if (ley.EstiloFuente == "Bold") tb.Bold();
                            else if (ley.EstiloFuente == "Italic") tb.Italic();
                        }
                        header.Item().PaddingBottom(5);
                    }
                    header.Item().AlignRight().Column(right =>
                    {
                        right.Item().AlignRight().Text(direccion).Italic();
                        right.Item().AlignRight().Text(subdireccion).Italic();
                        right.Item().AlignRight().Text($"No. {folio}").Bold();
                    });
                    header.Item().PaddingTop(15).Column(asunto =>
                    {
                        asunto.Item().AlignRight().Text($"Asunto: {tipoCarta} para prestación de");
                        asunto.Item().AlignRight().Text($"{tipoEtapa}.");
                    });
                    header.Item().PaddingTop(15).AlignRight().Text($"Zumpango, Edo. Méx., a {DateTime.Now:dd 'de' MMMM 'de' yyyy}.");
                });

                page.Content().PaddingLeft(cPL).PaddingRight(cPR).PaddingTop(20).Column(col =>
                {
                    col.Item().Column(dest =>
                    {
                        dest.Item().Text(directorNombre).Bold();
                        dest.Item().Text(directorPuesto).Bold();
                        dest.Item().Text(universidad).Bold();
                        dest.Item().Text("Presente.");
                    });
                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Justify();
                        text.Span("        Por medio del presente, me permito comunicarle que la C. ");
                        text.Span(nombreEstudiante).Bold();
                        text.Span(" con número de matrícula ");
                        text.Span(matricula).Bold();
                        text.Span(", adscrita de la carrera en ");
                        text.Span(carrera).Bold();
                        text.Span($" en esa institución, ha concluido satisfactoriamente su prestación en el programa de ");
                        text.Span(tipoEtapa).Bold();
                        text.Span($" en la {subdireccion} del Aeropuerto Internacional Felipe Ángeles, S.A. de C.V., cubriendo un total de 200 horas durante el periodo comprendido del ");
                        text.Span($"{fechaInicio:dd 'de' MMMM}").Bold();
                        text.Span(" al ");
                        text.Span($"{fechaFin:dd 'de' MMMM 'de' yyyy}").Bold();
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
                        firma.Item().PaddingTop(4).Text(autoridadNombre).Bold();
                        firma.Item().Text(autoridadPuesto);
                    });
                    col.Item().PaddingTop(120).Text("c.c.p. la Coordinación de Empleo; debiendo integrar el expediente de la interesada. –Presente.").FontSize(11);
                });

                page.Foreground().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Solida"))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var xPt = (float)config.CoordX / 100f * 612f;
                        var yPt = (float)config.CoordY / 100f * 792f;
                        var wPt = (float)config.Ancho / 100f * 612f;
                        var hPt = (float)config.Alto / 100f * 792f;
                        layers.Layer().AlignLeft().AlignTop()
                            .PaddingLeft(xPt)
                            .PaddingTop(yPt)
                            .Width(wPt)
                            .Height(hPt)
                            .Image(imagenesBytes[config.Id]).FitArea();
                    }
                });

                page.Footer().ExtendHorizontal().PaddingHorizontal(-28.35f).Column(footer =>
                {
                    foreach (var pie in piesDePagina)
                    {
                        if (pie.TieneLinea)
                            footer.Item().Background(pie.ColorFondo ?? "#0c2f57").Border(pie.GrosorLinea).BorderColor(pie.ColorFondo ?? "#0c2f57").Padding(4)
                                .Text(text => { text.AlignCenter(); text.Span(pie.Texto).FontSize(pie.TamanoFuente).FontColor(pie.ColorLetra ?? "#ffffff"); });
                        else
                            footer.Item().Padding(4).Text(text => { text.AlignCenter(); text.Span(pie.Texto).FontSize(pie.TamanoFuente); });
                    }
                });
            });
        });

        var images = pdf.GenerateImages(new QuestPDF.Infrastructure.ImageGenerationSettings { ImageFormat = QuestPDF.Infrastructure.ImageFormat.Png, RasterDpi = 150 }).ToArray();
        if (images.Length > 0)
            return File(images[0], "image/png");
        return NotFound();
    }

    private async Task CargarViewBags(int? excludeId = null)
    {
        var imagenes = await _db.ImagenesCatalogo.ToListAsync();
        ViewBag.ImagenId = new SelectList(imagenes, "Id", "NombreArchivo");
        ViewBag.ImagenesJson = System.Text.Json.JsonSerializer.Serialize(
            imagenes.Select(i => new { id = i.Id, nombre = i.NombreArchivo ?? "", url = i.UrlImagen ?? "" }));

        // Leyendas activas para preview
        var hoy = DateTime.Now;
        var leyendas = await _db.Leyendas
            .Where(l => l.FechaInicio <= hoy && l.FechaFin >= hoy)
            .ToListAsync();
        ViewBag.LeyendasJson = System.Text.Json.JsonSerializer.Serialize(
            leyendas.Select(l => new { l.Texto, l.Alineacion, coordY = l.CoordY, l.TamanoFuente, l.EstiloFuente }));

        // Otras configuraciones visuales activas para preview (excluyendo la actual en edición)
        var otrasConfigs = await _db.ConfiguracionesVisuales
            .Include(c => c.Imagen)
            .Where(c => c.FechaInicio <= hoy && c.FechaFin >= hoy)
            .Where(c => excludeId == null || c.Id != excludeId)
            .ToListAsync();
        ViewBag.OtrasConfigsJson = System.Text.Json.JsonSerializer.Serialize(
            otrasConfigs.Select(c => new { c.CoordX, c.CoordY, c.Ancho, c.Alto, url = c.Imagen?.UrlImagen ?? "", c.Posicion }));

        // Pies de página activos para preview
        var pies = await _db.PiesDePagina
            .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy)
            .ToListAsync();
        ViewBag.PiesPaginaJson = System.Text.Json.JsonSerializer.Serialize(
            pies.Select(p => new { p.Texto, p.ColorFondo, p.ColorLetra, p.TieneLinea, p.GrosorLinea, p.Alineacion, p.TamanoFuente }));
    }

    [HttpGet]
    public async Task<IActionResult> VistaPreviaPdf()
    {
        var hoy = DateTime.Now;

        // Obtener todas las configuraciones visuales activas
        var configsVisuales = await _db.ConfiguracionesVisuales
            .Include(c => c.Imagen)
            .Where(c => c.FechaInicio <= hoy && c.FechaFin >= hoy)
            .ToListAsync();

        // Obtener leyendas activas
        var leyendasActivas = await _db.Leyendas
            .Where(l => l.FechaInicio <= hoy && l.FechaFin >= hoy)
            .ToListAsync();

        // Pre-cargar imágenes
        var imagenesBytes = new Dictionary<int, byte[]>();
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        using var httpClient = new HttpClient();
        foreach (var config in configsVisuales)
        {
            var url = config.Imagen?.UrlImagen;
            if (string.IsNullOrEmpty(url)) continue;
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

        QuestPDF.Settings.License = LicenseType.Community;

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginTop(1.5f, Unit.Centimetre);
                page.MarginBottom(1.5f, Unit.Centimetre);
                page.MarginHorizontal(2.5f, Unit.Centimetre);

                // Background: imágenes de fondo
                page.Background().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Fondo" || c.Posicion == null))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var imgBytes = imagenesBytes[config.Id];
                        if (config.Opacidad < 100)
                        {
                            using var original = SKBitmap.Decode(imgBytes);
                            using var surface = SKSurface.Create(new SKImageInfo(original.Width, original.Height));
                            var skCanvas = surface.Canvas;
                            skCanvas.Clear(SKColors.Transparent);
                            var paint = new SKPaint { Color = new SKColor(255, 255, 255, (byte)(255 * config.Opacidad / 100f)) };
                            skCanvas.DrawBitmap(original, 0, 0, paint);
                            using var img = surface.Snapshot();
                            using var data = img.Encode(SKEncodedImageFormat.Png, 90);
                            imgBytes = data.ToArray();
                        }
                        var xPt = (float)config.CoordX / 100f * 612f;
                        var yPt = (float)config.CoordY / 100f * 792f;
                        var wPt = (float)config.Ancho / 100f * 612f;
                        var hPt = (float)config.Alto / 100f * 792f;
                        layers.Layer().AlignLeft().AlignTop().PaddingLeft(xPt).PaddingTop(yPt).Width(wPt).Height(hPt).Image(imgBytes).FitArea();
                    }
                });

                // Header de ejemplo
                page.Header().Column(header =>
                {
                    header.Item().AlignRight().Column(right =>
                    {
                        right.Item().Text("Dirección de Administración").FontSize(9).Italic();
                        right.Item().Text("Subdirección de Recursos Humanos").FontSize(9).Italic();
                        right.Item().Text("No. AIFA/DA/SRH/EJEMPLO-001").FontSize(9).Bold();
                    });
                    header.Item().PaddingTop(15).AlignRight().Column(asunto =>
                    {
                        asunto.Item().Text("Asunto: Carta de ejemplo para vista previa").FontSize(9);
                    });
                    header.Item().PaddingTop(10).AlignRight()
                        .Text($"Zumpango, Edo. Méx., a {DateTime.Now:dd 'de' MMMM 'de' yyyy}.")
                        .FontSize(9);
                });

                // Contenido de ejemplo
                page.Content().PaddingTop(20).Column(col =>
                {
                    // Calcular padding por imágenes sólidas
                    float paddingExtra = 0;
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Solida"))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var imgBottom = (float)((config.CoordY + config.Alto) / 100m * 792m);
                        if (imgBottom > paddingExtra) paddingExtra = imgBottom;
                    }
                    if (paddingExtra > 62) col.Item().PaddingTop(paddingExtra - 62);

                    col.Item().Column(dest =>
                    {
                        dest.Item().Text("Dr. Nombre del Director Ejemplo").FontSize(10).Bold();
                        dest.Item().Text("Director de la División de Ingeniería").FontSize(9).Bold();
                        dest.Item().Text("Universidad Ejemplo").FontSize(9).Bold().Underline();
                        dest.Item().Text("Presente.").FontSize(9);
                    });

                    col.Item().PaddingTop(20).Text(text =>
                    {
                        text.Span("        Por medio del presente, me permito comunicarle que la C. ").FontSize(10);
                        text.Span("Nombre del Estudiante Ejemplo").FontSize(10).Bold();
                        text.Span(" con número de matrícula ").FontSize(10);
                        text.Span("1234567890").FontSize(10).Bold();
                        text.Span(", adscrita de la carrera en ").FontSize(10);
                        text.Span("Ingeniería en Software").FontSize(10).Bold();
                        text.Span(" en esa institución, ha concluido satisfactoriamente su prestación en el programa de ").FontSize(10);
                        text.Span("Estancia II").FontSize(10).Bold();
                        text.Span(" en la Subdirección de Recursos Humanos del Aeropuerto Internacional Felipe Ángeles, S.A. de C.V., cubriendo un total de 200 horas.").FontSize(10);
                    });

                    col.Item().PaddingTop(20).Text("        Sin otro en particular, aprovechando la ocasión para enviarle un cordial saludo.")
                        .FontSize(10);

                    col.Item().PaddingTop(30).Text("Atentamente.").FontSize(10);

                    col.Item().PaddingTop(40).Column(firma =>
                    {
                        firma.Item().Text("_________________________________").FontSize(9);
                        firma.Item().PaddingTop(4).Text("Lic. Nombre de Autoridad Ejemplo").FontSize(10).Bold();
                        firma.Item().Text("Sub de Recursos Humanos del AIFA, S.A de C.V").FontSize(9);
                    });

                    col.Item().PaddingTop(40).Text("c.c.p. la Coordinación de Empleo; debiendo integrar el expediente de la interesada. –Presente.")
                        .FontSize(8).FontColor(Colors.Grey.Darken2);
                });

                // Foreground: imágenes sólidas + leyendas
                page.Foreground().Layers(layers =>
                {
                    layers.PrimaryLayer();
                    foreach (var config in configsVisuales.Where(c => c.Posicion == "Solida"))
                    {
                        if (!imagenesBytes.ContainsKey(config.Id)) continue;
                        var xPt = (float)config.CoordX / 100f * 612f;
                        var yPt = (float)config.CoordY / 100f * 792f;
                        var wPt = (float)config.Ancho / 100f * 612f;
                        var hPt = (float)config.Alto / 100f * 792f;
                        layers.Layer().AlignLeft().AlignTop().PaddingLeft(xPt).PaddingTop(yPt).Width(wPt).Height(hPt).Image(imagenesBytes[config.Id]).FitArea();
                    }
                    foreach (var ley in leyendasActivas)
                    {
                        var yPt2 = (float)ley.CoordY / 100f * 792f;
                        layers.Layer().AlignTop().PaddingTop(yPt2).PaddingHorizontal(40).Column(col2 =>
                        {
                            var lineas = ley.Texto.Split('\n');
                            foreach (var linea in lineas)
                            {
                                var lineItem = col2.Item();
                                if (ley.Alineacion == "Centrado") lineItem = lineItem.AlignCenter();
                                else if (ley.Alineacion == "Derecha") lineItem = lineItem.AlignRight();
                                var textBlock = lineItem.Text(linea.Trim()).FontSize(ley.TamanoFuente);
                                if (ley.EstiloFuente == "Bold") textBlock.Bold();
                                else if (ley.EstiloFuente == "Italic") textBlock.Italic();
                            }
                        });
                    }
                });
            });
        });

        var stream = new MemoryStream();
        pdf.GeneratePdf(stream);
        stream.Position = 0;
        return File(stream, "application/pdf", "Vista_Previa_Configuracion.pdf");
    }
}
