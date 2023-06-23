namespace Proyecto_Api.Models
{
    public class Documento
    {
        public int IdDocumento { get; set; }
        public string? Descripcion { get; set; }
        public string? Ruta { get; set; }

        public IFormFile? Archivo { get; set; }  // nos permite almacenar el archivo atraves de la API
    }
}
