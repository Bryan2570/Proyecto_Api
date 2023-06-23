using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using System.Data;
using System.Data.SqlClient;
using Proyecto_Api.Models;

namespace Proyecto_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        //guardamos nuestra informacion de la configuracion 
        private readonly string _rutaDocumento;
        private readonly string _cadenaSql;

        //obtenemos la informacion
        public DocumentoController(IConfiguration config) {
            _rutaDocumento = config.GetSection("Configuracion").GetSection("RutaDocumentos").Value;
            _cadenaSql = config.GetConnectionString("CadenaSQL");
        }


        [HttpPost]
        [Route("Subir")] //Nombre de la API
        [DisableRequestSizeLimit,RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)] // valor maximo del archivo a subir sin restriccion

        public IActionResult Subir([FromForm] Documento request) 
        {
            //concateno la ruta DONDE VA A SER GUARDADO junto con el nombre con el cual va a ser guardado
            string rutaDocu = Path.Combine(_rutaDocumento, request.Archivo.FileName);

            try
            {
                //con esto decimos que nuestro diocumento va a ser guardado en nuestra ruta del servidor
                using (FileStream newFile = System.IO.File.Create(rutaDocu)) {
                    request.Archivo.CopyTo(newFile);
                    newFile.Flush();                
                }

                //guardamos la informacion en nuestra DB
                using (var cn = new SqlConnection(_cadenaSql)) { 
                    cn.Open();
                    var cmd = new SqlCommand("sp_guardar_documento", cn);
                    cmd.Parameters.AddWithValue("descripcion", request.Descripcion);
                    cmd.Parameters.AddWithValue("ruta", rutaDocu);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "documento guardado" });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status200OK, new { mensaje = error.Message });
            }
        }

    }
}
