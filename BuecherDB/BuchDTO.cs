using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuecherDB
{
    public class BuchDTO
    {
        public bool isAktuell { get; set; }
        public string? Titel { get; set; }
        public string? Autor { get; set; }
    }
}
