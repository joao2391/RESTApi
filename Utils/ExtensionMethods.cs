using Newtonsoft.Json;

namespace ApiREST.Utils
{
    public static class ExtensionMethods
    {
        public static bool IsObjectNull(this object obj)
        {
            return obj == null ? true : false;
        }

        public static string SerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
