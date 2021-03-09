using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_promos.Data.Configuration
{
    public class PromocionesStoreDatabaseSettings : IPromocionesStoreDatabaseSettings
    {
        public string PromocionesColletionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IPromocionesStoreDatabaseSettings
    {
        public string PromocionesColletionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
