using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_promos.DTO
{
    public class PromocionFilterDTO
    { 
        public string MediosDePago { get; set; }

        public string Bancos { get; set; }

        public IEnumerable<string> CategoriasDeProductos { get; set; }  
    }
}
