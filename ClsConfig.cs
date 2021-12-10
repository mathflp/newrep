using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERTEDUC.EnvioLote
{
    public class ClsConfig
    {
        public string nome { get; set; }
        public string cor_primaria { get; set; }
        public string cor_secundaria { get; set; }
        public string logo { get; set; }
        public string link { get; set; }
        public List<Organizacao> organizacoes { get; set; }
    }

}
