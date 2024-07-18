using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products
{
    public class UpdateProductRequest
    {
        public string Refe { get; set; } = null!;

        public string Nom { get; set; } = null!;

        public int Qte { get; set; }

        public int QteLimite { get; set; }

        public double Remise { get; set; }

        public double RemiseAchat { get; set; }

        public double Tva { get; set; }

        public decimal Prix { get; set; }

        public decimal PrixAchat { get; set; }

        public bool Visibilite { get; set; }
    }
}
