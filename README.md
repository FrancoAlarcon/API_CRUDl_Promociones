 API_CRUDl_Promociones
 
Por defecto trae todas la promociones que esten activas

Para traer una promocion filtrada por id (get) 
https://localhost:5001/api/promociones/f06ca160-5c1f-483b-89bf-2da2237a4e7a
                                        (Guid)

Trae todas las promociones vigentes(get) 
https://localhost:5001/api/promociones/GetPromocionesVigentes

Para traer promociones filtradas por fechas usar el siguiente(solo activas)(get) 
https://localhost:5001/api/promociones/GetPromocionesVigentesFiltradasPorFecha/30-10-2020/24-08-2020

Trae todas las promociones vigentes(get) 
https://localhost:5001/api/promociones/GetPromocionesVigentesParaUnaVenta
    {
          "MediosDePago": "EFECTIVO",
          "Bancos": "Santander Rio",
          "CategoriasDeProductos": ["Colchones"]
    }

Para crear (post)
https://localhost:5001/api/promociones
        {
          "Id": "f06ca160-5c1f-483b-89bf-2da2237a4e7a",
          "Bancos": [],
          "CategoriasProductos": ["Colchones"],
          "MediosDePago": ["EFECTIVO"],
          "MaximaCantidadDeCuotas": null,
          "ValorInteresCuotas": null,
          "PorcentajeDeDescuento": 10,
          "FechaDeInicio": "01/06/2018",
          "FechaDeFin": "01/06/2019"
        }

Para editar (put)
https://localhost:5001/api/promociones/f06ca160-5c1f-483b-89bf-2da2237a4e7a
                                        (Guid)                                        

     (entidad editada)
     {
          "Id": "f06ca160-5c1f-483b-89bf-2da2237a4e7a",
          "Bancos": [],
          "CategoriasProductos": ["Colchones"],
          "MediosDePago": ["EFECTIVO"],
          "MaximaCantidadDeCuotas": null,
          "ValorInteresCuotas": null,
          "PorcentajeDeDescuento": 10,
          "FechaDeInicio": "01/06/2018",
          "FechaDeFin": "01/06/2019"
        }  

Para editar fecha de vigencia (put)
https://localhost:5001/api/promociones/UpdateVigencia/f06ca160-5c1f-483b-89bf-2da2237a4e7a/30-10-2020/29-11-2020 
                                                       (Guid de promocion a editar)  (fecha de comienzo y fin de vigencia)       

Para eliminar (delete) (El eliminado es logico, se pone activo en false)
https://localhost:5001/api/promociones/f06ca160-5c1f-483b-89bf-2da2237a4e7a
                                        (Guid)         

