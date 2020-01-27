using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.Models
{
    public class ProjecaoFiltroViewModel
    {
        public string DtIni { get; set; }

        public DateTime DtIniTrat
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DtIni))
                    return DateTime.Now.Date;
                if (DtIni.Contains(":"))
                    return DateTime.ParseExact(DtIni, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                return DateTime.ParseExact(DtIni, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public string DtFim { get; set; }

        public DateTime DtFimTrat
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DtFim))
                    return DateTime.Now.Date;
                if (DtFim.Contains(":"))
                    return DateTime.ParseExact(DtFim, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                return DateTime.ParseExact(DtFim, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public int[] Filiais { get; set; }
        public int[] Categorias { get; set; }
    }
}
