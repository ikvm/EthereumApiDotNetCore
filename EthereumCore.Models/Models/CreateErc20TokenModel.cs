﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EthereumApi.Models
{
    public class CreateErc20TokenModel
    {
        public string Address { get; set; }
        public string Name { get; set; }
    }
}