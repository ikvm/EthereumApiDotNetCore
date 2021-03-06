﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.EthereumCore.Core.Exceptions;
using Lykke.Service.EthereumCore.Models;
using Microsoft.AspNetCore.Mvc;
using Lykke.Service.EthereumCore.Services;
using Lykke.Service.EthereumCore.Services.Coins;
using Common.Log;
using Lykke.Service.EthereumCoreSelfHosted.Models;
using Lykke.Service.EthereumCore.Core.Repositories;
using Nethereum.Util;

namespace Lykke.Service.EthereumCore.Controllers
{
    [Route("api/transition")]
    [Produces("application/json")]
    public class TransitionContractsController : Controller
    {
        private readonly ILog _logger;
        private readonly ITransferContractService _transferContractService;
        private readonly AddressUtil _addressUtil;

        public TransitionContractsController(ITransferContractService transferContractService, ILog logger)
        {
            _addressUtil = new AddressUtil();
            _transferContractService = transferContractService;
            _logger = logger;
        }

        [Route("create")]
        [HttpPost]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> CreateTransferContract([FromBody]CreateTransitionContractModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string contractAddress = await _transferContractService.CreateTransferContract(_addressUtil.ConvertToChecksumAddress(model.UserAddress),
                model.CoinAdapterAddress);

            return Ok(new RegisterResponse
            {
                Contract = contractAddress
            });
        }

        [Route("contractAddress/{userAddress}/{coinAdapterAddress}")]
        [HttpGet]
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        [ProducesResponseType(typeof(ApiException), 400)]
        [ProducesResponseType(typeof(void), 404)]
        [ProducesResponseType(typeof(ApiException), 500)]
        public async Task<IActionResult> GetAddress(string userAddress, string coinAdapterAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ITransferContract contract = await _transferContractService.GetTransferContract(_addressUtil.ConvertToChecksumAddress(userAddress),
                coinAdapterAddress);

            if (contract == null)
            {
                return NotFound();
            }

            return Ok(new RegisterResponse
            {
                Contract = contract.ContractAddress
            });
        }
    }
}
