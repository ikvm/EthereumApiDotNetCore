﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumCore.Models
{
    [DataContract]
    public class EstimatedGasModel
    {
        [DataMember]
        public string EstimatedGas { get; set; }

        [DataMember]
        public bool IsAllowed { get; set; }
    }
}
