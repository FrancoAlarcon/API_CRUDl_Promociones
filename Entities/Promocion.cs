using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_promos.Entities
{
    public class Promocion
    {
        public Guid Id { get; set; }
        public IEnumerable<string> MediosDePago { get; set; }
        public IEnumerable<string> Bancos { get; set; }
        public IEnumerable<string> CategoriasDeProductos { get; set; }
        public int? MaximaCantidadDeCuotas { get; set; }
        public decimal? ValorInteresCuotas { get; set; }
        public decimal? PorcentajeDeDescuento { get; set; }
        public DateTime? FechaDeInicio { get; set; }
        public DateTime? FechaDeFin { get; set; }
        public DateTime FechaDeCreacion { get; set; }
        public DateTime? FechaDeModificacion { get; set; }
        public bool Activo { get; set; }
    }
}
