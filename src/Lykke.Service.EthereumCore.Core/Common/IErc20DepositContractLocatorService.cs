﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumCore.Core.Common
{
    public interface IErc20DepositContractLocatorService
    {
        Task<bool> ContainsAsync(string address);
    }
}
