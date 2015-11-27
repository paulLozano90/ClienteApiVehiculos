using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BaseServicios
{
    public class ServicioRestImpl<TModelo>:IServiciosRest<TModelo>
    {
        private String url;
        private bool auth;
        private String user;
        private String pass;

        //Siempre recibira la url del servicio, si queremos autenticacion, usuario y pass
        public ServicioRestImpl(String url, bool auth=false, String user=null, String pass=null)
        {
            this.url = url;
            this.auth = auth;
            this.user = user;
            this.pass = pass;
        } 

        public async Task<TModelo> Add(TModelo model)
        {
            var datos = Serializacion<TModelo>.Serializar(model);
            using (var handler = new HttpClientHandler())
            {
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                using (var client = new HttpClient(handler))
                {
                    var contenido = new StringContent(datos);
                    contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var r = await client.PostAsync(new Uri(url), contenido);
                    if (!r.IsSuccessStatusCode)
                        throw new Exception("Fallo gordo");
                    //Hasta aqui igual que el update
                    var objSerializado = await r.Content.ReadAsStringAsync();
                    //var objSerializado = r.Content.ReadAsStringAsync().ContinueWith(expresion landa);
                    return Serializacion<TModelo>.Deserializar(objSerializado);
                }
            }
        }
        //Hay que ponerlo como asincrono
        public async Task Update(TModelo model)
        {
            //Datos = objeto que le mando serializado
            var datos = Serializacion<TModelo>.Serializar(model);
            //HttpClient: Crea un cliente y se maneja atraves del Handler
            //Se encarga de autenticar(credenciales), crear las cabeceras...
            using (var handler = new HttpClientHandler())
            {
                //En el handler se podria meter mas informacion
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                using (var client = new HttpClient(handler))
                {
                    var contenido = new StringContent(datos);
                    contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    //Si no le metemos el await nos aseguramos que no cierra el client hasta que nosotros se lo digamos. Ya que si lo cierra nos quedamos sin informacion asi que es mejor ponerlo.
                    //El putAsync devuelve un task
                    //Si no le haces un await se le podria meter un callback --> .ContinueWith
                    //client.PutAsync(new Uri(url), contenido).ContinueWith(...);
                    var r = await client.PutAsync(new Uri(url), contenido);
                    if (!r.IsSuccessStatusCode)
                        throw new Exception("Fallo gordo");
                    
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var handler = new HttpClientHandler())
            {
                if (auth)
                {
                    handler.Credentials = new NetworkCredential(user, pass);
                }
                using (var client = new HttpClient(handler))
                {
                    var r = await client.DeleteAsync(new Uri(url+"/"+id));
                    if (!r.IsSuccessStatusCode)
                        throw new Exception("Fallo gordo");
                }
            }
        }
        //Primero se hace los GET.
        public List<TModelo> Get(String paramUrl=null)
        {
            List<TModelo> lista;
            var request = WebRequest.Create(url);
            if (auth)
            {
                //NetworkCreantial: Genera unas credenciales (usuario y pass)
                request.Credentials = new NetworkCredential(user,pass);
            }
            //Steam sirve como canal de conexion para enviar datos entre dos puntos
            //Los stream viene en datos binarios por eso hay que transformarlo
            request.Method = "GET";
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                //Creamos un reader para leer datos de un stream como si fueran texto
                using (var reader = new StreamReader(stream))
                {
                    //ReadToEnd:Lee desde donde estas hasta el final, esto ya lo
                    //devuelve como un string. El ReadLine, solo la linea
                    var serializado = reader.ReadToEnd();
                    lista = Serializacion<List<TModelo>>.Deserializar(serializado);
                }
            }
            return lista;
        }

        public List<TModelo> Get(Dictionary<string, string> param)
        {
            var paramsurl = "";
            var primero = true;
            foreach (var key in param.Keys)
            {
                if (primero)
                {
                    paramsurl += "?";
                    primero = false;
                }
                else
                    paramsurl += "&";
                paramsurl += key + "=" + param[key];
            }
            return Get(paramsurl);
        }

        public TModelo Get(int id)
        {
            TModelo objeto;
            var request = WebRequest.Create(url+"/"+id);
            if (auth)
            {
                request.Credentials = new NetworkCredential(user, pass);
            }
            request.Method = "GET";
            var response = request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var serializado = reader.ReadToEnd();
                    objeto = Serializacion<TModelo>.Deserializar(serializado);
                }
            }
            return objeto;
        }
    }
}