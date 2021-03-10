using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto_promos.Data;
using Proyecto_promos.DTO;
using Proyecto_promos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace Proyecto_promos.Controllers
{
    [Route("api/promociones")]
    [ApiController]
    public class PromocionesController : ControllerBase
    {
        private readonly PromocionesService _promocionesService;

        public PromocionesController(PromocionesService promocionesService)
        {
            _promocionesService = promocionesService;
        }

        #region Gets 
        [HttpGet]
        public IActionResult Get()
        { 
            return Ok(_promocionesService.Get().Where(p => p.Activo));
        }

        [HttpGet("GetPromocionesVigentes", Name = "GetPromocionesVigentes")]        
        public IActionResult GetPromocionesVigentes()
        {
            return Ok(_promocionesService.GetPromocionesVigentes().Where(p => p.Activo));
        }  

        [HttpGet("GetPromocionesVigentesFiltradasPorFecha/{fechaDeComienzo:length(10)}/{fechaDeFinal:length(10)}", Name = "GetPromocionesVigentesFiltradasPorFecha")]
        public IActionResult GetPromocionesVigentesFiltradasPorFecha(string fechaDeComienzo, string fechaDeFinal)
        {
            DateTime fechadeComienzoParseada = DateTime.Parse(fechaDeComienzo);
            DateTime fechaDeFinalParseada = DateTime.Parse(fechaDeFinal);
            return Ok(_promocionesService.GetByVigency(fechadeComienzoParseada, fechaDeFinalParseada).Where(p => p.Activo));
        } 

        [HttpGet("GetPromocionesVigentesParaUnaVenta", Name = "GetPromocionesVigentesParaUnaVenta")]
        public IActionResult GetPromocionesVigentesParaUnaVenta(PromocionFilterDTO promocionFilterDTO)
        {
            return Ok(_promocionesService.GetPromocionesVigentesParaUnaVenta(promocionFilterDTO.MediosDePago, promocionFilterDTO.Bancos, promocionFilterDTO.CategoriasDeProductos).Where(p => p.Activo));
        }

        [HttpGet("{id:length(36)}", Name = "GetById")]
        public IActionResult GetById(string id)
        {
            var promocion = _promocionesService.GetById(id);

            if (promocion == null)
            {
                return NotFound();
            }

            return Ok(promocion);
        }
        #endregion

        #region CRUDl 
        [HttpPost]
        public IActionResult Create(PromocionDTO promocionDTO)
        {
            Promocion promocion = new Promocion()
            {
                Id = promocionDTO.Id,
                MediosDePago = promocionDTO.MediosDePago,
                Bancos = promocionDTO.Bancos,
                CategoriasDeProductos = promocionDTO.CategoriasDeProductos,
                MaximaCantidadDeCuotas = promocionDTO.MaximaCantidadDeCuotas,
                ValorInteresCuotas = promocionDTO.ValorInteresCuotas,
                PorcentajeDeDescuento = promocionDTO.PorcentajeDeDescuento,
                FechaDeInicio = DateTime.Parse(promocionDTO.FechaDeInicio),
                FechaDeFin = DateTime.Parse(promocionDTO.FechaDeFin)

            };
            var result = _promocionesService.Create(promocion);

            return Ok(result);
        }

        [HttpPut("{id:length(36)}")]
        public IActionResult Update(string id, PromocionDTO promocionDTO)
        {
            var promocion = _promocionesService.GetById(id);

            if (promocion == null)
            {
                return NotFound();
            }

            Promocion promocionParse = new Promocion()
            {
                Id = promocionDTO.Id,
                MediosDePago = promocionDTO.MediosDePago,
                Bancos = promocionDTO.Bancos,
                CategoriasDeProductos = promocionDTO.CategoriasDeProductos,
                MaximaCantidadDeCuotas = promocionDTO.MaximaCantidadDeCuotas,
                ValorInteresCuotas = promocionDTO.ValorInteresCuotas,
                PorcentajeDeDescuento = promocionDTO.PorcentajeDeDescuento,
                FechaDeInicio = DateTime.Parse(promocionDTO.FechaDeInicio),
                FechaDeFin = DateTime.Parse(promocionDTO.FechaDeFin)

            };

            var result = _promocionesService.Update(id, promocionParse);

            return Ok(result);
        }

        [HttpPut("UpdateVigencia/{id:length(36)}/{fechaDeComienzo:length(10)}/{fechaDeFinal:length(10)}", Name = "UpdateVigencia")]
        public IActionResult UpdateVigencia(string id, string fechaComienzo, string fechaFinal)
        {
            if(DateTime.Parse(fechaComienzo) > DateTime.Parse(fechaFinal))
                return Ok("La fecha de comienzo no puede ser mayor a la fecha de final");

            var result = _promocionesService.UpdateVigenciaDePromocion(id, DateTime.Parse(fechaComienzo), DateTime.Parse(fechaFinal));

            return Ok(result);
        }

        [HttpDelete("{id:length(36)}")]
        public IActionResult DeleteById(string id)
        {
            var promocion = _promocionesService.GetById(id);

            if (promocion == null)
            {
                return NotFound();
            }

            _promocionesService.DeleteById(id);

            return NoContent();
        }
        #endregion

        //Ejemplo
        //        {
        //  "Id": "f06ca160-5c1f-483b-89bf-2da2237a4e7a",
        //  "Bancos": [],
        //  "CategoriasProductos": ["Colchones"],
        //  "MediosDePago": ["EFECTIVO"],
        //  "MaximaCantidadDeCuotas": null,
        //  "ValorInteresCuotas": null,
        //  "PorcentajeDeDescuento": 10,
        //  "FechaDeInicio": "01/06/2018",
        //  "FechaDeFin": "01/06/2019"
        //}
}
}