using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class Constants
    {
        public static string apikey = "uzuVWsyRY2fM9UJhh38qPfDeHcPZ8pConh2PrVYM";
        //Geomagnetic Storm (GST)
        public static string AddressGST = "https://api.nasa.gov/DONKI/GST";
        //Solar Flare (FLR)
        public static string AddressFLR = "https://api.nasa.gov/DONKI/FLR";
        //Solar Energetic Particle (SEP)
        public static string AddressSEP = "https://api.nasa.gov/DONKI/SEP";
        public static string Connect = "Host=localhost;Username=postgres; Password=0309; Database=Rate";
    }
}
