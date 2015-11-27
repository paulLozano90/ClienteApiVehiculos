using System;
using System.Web.Script.Serialization;

namespace BaseServicios
{
    public class Serializacion<T>
    {
        public static T Deserializar(String obj)
        {
            var ser = new JavaScriptSerializer();
            var data = ser.Deserialize<T>(obj);
            return data;
        }
        //Debemos crear un constructor vacio
        //ya que si no esta va generar problemas

        public static String Serializar(T obj)
        {
            var ser = new JavaScriptSerializer();
            var data = ser.Serialize(obj);
            return data;
        } 
    }
}