using MongoDB.Driver;
using Proyecto_promos.Data.Configuration;
using Proyecto_promos.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_promos.Data
{
    public class PromocionesService
    {
        private readonly IMongoCollection<Promocion> _promocionesCollection;

        public PromocionesService(IPromocionesStoreDatabaseSettings settings)
        {
            var mdbPromociones = new MongoClient(settings.ConnectionString);
            var database = mdbPromociones.GetDatabase(settings.DatabaseName);

            _promocionesCollection = database.GetCollection<Promocion>(settings.PromocionesColletionName);
        }
        #region Gets
        public List<Promocion> Get()
        {
            return _promocionesCollection.Find(promo => true).ToList();
        }

        public List<Promocion> GetPromocionesVigentes()
        {
            return _promocionesCollection.Find(promo => true).ToList().Where(p => p.FechaDeInicio <= DateTime.Now && p.FechaDeFin >= DateTime.Now).ToList();
        }

        public List<Promocion> GetPromocionesVigentesParaUnaVenta(string medioDePago, string banco, IEnumerable<string> categoriaDeProducto)
        {
            return _promocionesCollection.Find(promo => true).ToList().Where(p => p.Bancos.Any(b => b == banco) && p.MediosDePago.Any(b => b == medioDePago)).ToList();
        }

        public Promocion GetById(string id)
        {
            return _promocionesCollection.Find<Promocion>(promocion => promocion.Id == new Guid(id)).FirstOrDefault();
        }

        public List<Promocion> GetByVigency(DateTime? startDateTime, DateTime? endDateTime)
        {
            List<Promocion> result = _promocionesCollection.Find<Promocion>(promocion => promocion.FechaDeInicio >= startDateTime && promocion.Activo).ToList();
            if (endDateTime != null)
                result = result.Where(p => p.FechaDeFin <= endDateTime).ToList();
            return result;
        }

        #endregion

        #region CRUDl
        public string Create(Promocion promo)
        {
            if ((promo.PorcentajeDeDescuento == null || promo.PorcentajeDeDescuento == 0) && (promo.MaximaCantidadDeCuotas == null || promo.MaximaCantidadDeCuotas == 0))
                return "La promocion debe tener un porcentaje de descuentos o un maximo de cantidad de cuotas.";

            if (promo.PorcentajeDeDescuento != null && promo.PorcentajeDeDescuento != 0 && promo.MaximaCantidadDeCuotas != null && promo.MaximaCantidadDeCuotas != 0)
                return "La promocion no puede tener descuento o cuotas sin interes no ambas.";

            if ((promo.ValorInteresCuotas != null) && (promo.MaximaCantidadDeCuotas == null))
                return "El porcentaje de interes solo es valido si se inserta una cantidad de cuotas.";

            if (promo.PorcentajeDeDescuento < 5 || promo.PorcentajeDeDescuento > 80)
                return "El procentaje de descuento debe estar entre 5% y 80%.";

            if (promo.FechaDeInicio > promo.FechaDeFin)
                return "La fecha de comienzo no puede ser mayor a la fecha de final.";

            if (promo.Bancos != null && promo.Bancos.Count() > 0)
            {
                string validateBancos = this.ValidarBancos(promo.Bancos);
                if (validateBancos != "Bancos validos")
                    return validateBancos;
            }

            if (promo.MediosDePago != null && promo.MediosDePago.Count() > 0)
            {
                string validateMediosDePago = this.ValidarMediosDePago(promo.MediosDePago);
                if (validateMediosDePago != "Medios de pago validos")
                    return validateMediosDePago;
            }

            if (promo.CategoriasDeProductos != null && promo.CategoriasDeProductos.Count() > 0)
            {
                string validateCategoriasDeProducto = this.ValidarCategoriasDeProductos(promo.CategoriasDeProductos);
                if (validateCategoriasDeProducto != "Categorias validas")
                    return validateCategoriasDeProducto;
            }

            promo.FechaDeModificacion = DateTime.Now;
            promo.FechaDeCreacion = DateTime.Now;
            promo.Activo = true;
            if (ValidarSolapamiento(promo))
                return "Ya hay una promocion con las mismas propiedades";
            if (_promocionesCollection.Find(promo => true).ToList().Where(p => p.Id == promo.Id).Count() > 0)
                return "Ya hay una promocion con este id";
            _promocionesCollection.InsertOne(promo);
            return promo.Id.ToString();
        }

        public string Update(string id, Promocion promo)
        {
            if ((promo.PorcentajeDeDescuento == null || promo.PorcentajeDeDescuento == 0) && (promo.MaximaCantidadDeCuotas == null || promo.MaximaCantidadDeCuotas == 0))
                return "La promocion debe tener un porcentaje de descuentos o un maximo de cantidad de cuotas.";

            if (promo.PorcentajeDeDescuento != null && promo.PorcentajeDeDescuento != 0 && promo.MaximaCantidadDeCuotas != null && promo.MaximaCantidadDeCuotas != 0)
                return "La promocion no puede tener descuento o cuotas sin interes no ambas.";

            if (promo.ValorInteresCuotas != null && promo.MaximaCantidadDeCuotas == null)
                return "El porcentaje de interes solo es valido si se inserta una cantidad de cuotas.";

            if (promo.PorcentajeDeDescuento < 5 || promo.PorcentajeDeDescuento > 80)
                return "El procentaje de descuento debe estar entre 5% y 80%.";

            if (promo.FechaDeInicio > promo.FechaDeFin)
                return "La fecha de comienzo no puede ser mayor a la fecha de final.";

            if (_promocionesCollection.Find(promo => true).ToList().Where(p => p.Id == promo.Id).Count() > 0)
                return "Ya hay una promocion con este id";

            if (promo.Bancos != null && promo.Bancos.Count() > 0)
            {
                string validateBancos = this.ValidarBancos(promo.Bancos);
                if (validateBancos != "Bancos validos")
                    return validateBancos;
            }

            if (promo.MediosDePago != null && promo.MediosDePago.Count() > 0)
            {
                string validateMediosDePago = this.ValidarMediosDePago(promo.MediosDePago);
                if (validateMediosDePago != "Medios de pago validos")
                    return validateMediosDePago;
            }

            if (promo.CategoriasDeProductos != null && promo.CategoriasDeProductos.Count() > 0)
            {
                string validateCategoriasDeProducto = this.ValidarCategoriasDeProductos(promo.CategoriasDeProductos);
                if (validateCategoriasDeProducto != "Categorias validas")
                    return validateCategoriasDeProducto;
            }

            if (ValidarSolapamiento(promo))
                return "Ya hay una promocion con las mismas propiedades";

            promo.FechaDeModificacion = DateTime.Now;
            _promocionesCollection.ReplaceOne(promocion => promocion.Id == new Guid(id), promo);

            return id;
        }

        public string UpdateVigenciaDePromocion(string id, DateTime? fechaDeInicio, DateTime? fechaDeFin)
        {
            Promocion promocionDB = _promocionesCollection.Find<Promocion>(promocion => promocion.Id == new Guid(id)).FirstOrDefault();
            promocionDB.FechaDeModificacion = DateTime.Now;
            promocionDB.FechaDeInicio = fechaDeInicio;
            promocionDB.FechaDeInicio = fechaDeFin;
            if (ValidarSolapamiento(promocionDB))
                return "Ya hay una promocion con las mismas propiedades";
            _promocionesCollection.ReplaceOne(promocion => promocion.Id == new Guid(id), promocionDB);
            return "Editado correctamente";
        }

        public void DeleteById(string id)
        {
            Promocion promocionDB = _promocionesCollection.Find<Promocion>(promocion => promocion.Id == new Guid(id)).FirstOrDefault();
            promocionDB.Activo = false;
            promocionDB.FechaDeModificacion = DateTime.Now;
            _promocionesCollection.ReplaceOne(promocion => promocion.Id == new Guid(id), promocionDB);
        }
        #endregion

        #region Validations 
        public string ValidarMediosDePago(IEnumerable<string> mediosDePago)
        {
            int errores = 0;
            List<string> nombresErroneos = new List<string>();
            string result = "Medios de pago validos";

            foreach (string medioDePago in mediosDePago)
            {
                if (medioDePago != "EFECTIVO" && medioDePago != "GIFT_CARD" && medioDePago != "TARJETA_CREDITO"
                    && medioDePago != "TARJETA_DEBITO")
                {
                    errores++;
                    nombresErroneos.Add(medioDePago);
                }
            }
            if (errores > 0)
            {
                result = "Los siguientes medios de pago no son validos ";
                for (int i = 0; i < nombresErroneos.Count; i++)
                {
                    if (i == nombresErroneos.Count-1)
                        result = result + nombresErroneos[i] + ".";
                    else
                        result = result + nombresErroneos[i] + ", ";
                }
            }

            return result;
        }

        public string ValidarBancos(IEnumerable<string> bancos)
        {
            int errores = 0;
            List<string> nombresErroneos = new List<string>();
            string result = "Bancos validos";

            foreach (string banco in bancos)
            {
                if (banco != "Galicia" && banco != "Santander Rio" && banco != "Ciudad"
                    && banco != "BBVA" && banco != "Nacion" && banco != "ICBC" && banco != "Macro")
                {
                    errores++;
                    nombresErroneos.Add(banco);
                }
            }
            if (errores > 0)
            {
                result = "Los siguientes bancos no son validos ";
                for (int i = 0; i < nombresErroneos.Count; i++)
                {
                    if (i == nombresErroneos.Count-1)
                        result = result + nombresErroneos[i] + ".";
                    else
                        result = result + nombresErroneos[i] + ", ";
                }
            }

            return result;
        }

        public string ValidarCategoriasDeProductos(IEnumerable<string> categorias)
        {
            int errores = 0;
            List<string> nombresErroneos = new List<string>();
            string result = "Categorias validas";

            foreach (string categoria in categorias)
            {
                if (categoria != "Hogar" && categoria != "Jardin" && categoria != "ElectroCocina" && categoria != "Audio"
                    && categoria != "GrandesElectro" && categoria != "Colchones" && categoria != "Celulares" && categoria != "Tecnologia")
                {
                    errores++;
                    nombresErroneos.Add(categoria);
                }
            }
            if (errores > 0)
            {
                result = "Las siguientes categorias no son validas ";
                for (int i = 0; i < nombresErroneos.Count; i++)
                {
                    if (i == nombresErroneos.Count-1)
                        result = result + nombresErroneos[i] + ".";
                    else
                        result = result + nombresErroneos[i] + ", ";
                }
            }

            return result;
        }

        public bool ValidarSolapamiento(Promocion promocion)
        {
            var todasLasPromociones = _promocionesCollection.Find(promo => true).ToList().Where(p => p.Id != promocion.Id);

            var copia = todasLasPromociones.Where(p => p.MaximaCantidadDeCuotas == promocion.MaximaCantidadDeCuotas &&
            p.ValorInteresCuotas == promocion.ValorInteresCuotas &&
            p.PorcentajeDeDescuento == promocion.PorcentajeDeDescuento &&
            p.FechaDeInicio == promocion.FechaDeInicio &&
            p.FechaDeFin == promocion.FechaDeFin &&
            p.Activo == promocion.Activo &&
            p.MediosDePago == promocion.MediosDePago &&
            p.Bancos == promocion.Bancos &&
            p.CategoriasDeProductos == promocion.CategoriasDeProductos);

            return copia.Count() >= 1;
        }

        #endregion 
    }
}