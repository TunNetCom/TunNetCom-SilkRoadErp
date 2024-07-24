﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TunNetCom.SilkRoadErp.Sales.Contracts.Products
{
    public class ProductResponse
    {
        [JsonPropertyName("refe")]
        public string Refe { get; set; } = null!;

        [JsonPropertyName("nom")]
        public string Nom { get; set; } = null!;

        [JsonPropertyName("qte")]
        public int Qte { get; set; }

        [JsonPropertyName("qteLimite")]
        public int QteLimite { get; set; }

        [JsonPropertyName("Remise")]
        public double Remise { get; set; }

        [JsonPropertyName("RemiseAchat")]
        public double RemiseAchat { get; set; }

        [JsonPropertyName("tva")]
        public double Tva { get; set; }

        [JsonPropertyName("prix")]
        public decimal Prix { get; set; }

        [JsonPropertyName("prixAchat")]
        public decimal PrixAchat { get; set; }

        [JsonPropertyName("visibilite")]
        public bool Visibilite { get; set; }
    }
}