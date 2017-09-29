﻿using BusinessModels;
using BusinessModels.PrivateWallet;
using Core;
using Core.Exceptions;
using Core.Settings;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.Util;
using Nethereum.Web3;
using Services.Model;
using Services.Signature;
using Services.Transactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Services.PrivateWallet
{
    public interface IPrivateWalletService
    {
        Task<OperationEstimationResult> EstimateTransactionExecutionCost(string from, string signedTrHex);
        Task<string> GetTransactionForSigning(EthTransaction ethTransaction);
        Task<string> SubmitSignedTransaction(string from, string signedTrHex);
        //Task<bool> CheckTransactionSign(string from, string signedTrHex);
        Task ValidateInputAsync(EthTransaction transaction);
    }

    public class PrivateWalletService : IPrivateWalletService
    {
        private readonly IWeb3 _web3;
        private readonly INonceCalculator _nonceCalculator;
        private readonly IRawTransactionSubmitter _rawTransactionSubmitter;
        private readonly IEthereumTransactionService _ethereumTransactionService;
        private readonly IPaymentService _paymentService;
        private readonly ISignatureChecker _signatureChecker;
        private readonly ITransactionValidationService _transactionValidationService;
        private readonly IErc20Service _erc20Service;

        public PrivateWalletService(IWeb3 web3,
            INonceCalculator nonceCalculator,
            IRawTransactionSubmitter rawTransactionSubmitter,
            IEthereumTransactionService ethereumTransactionService,
            IPaymentService paymentService,
            ISignatureChecker signatureChecker,
            ITransactionValidationService transactionValidationService,
            IErc20Service erc20Service)
        {
            _signatureChecker             = signatureChecker;
            _rawTransactionSubmitter      = rawTransactionSubmitter;
            _nonceCalculator              = nonceCalculator;
            _web3                         = web3;
            _ethereumTransactionService   = ethereumTransactionService;
            _paymentService               = paymentService;
            _transactionValidationService = transactionValidationService;
            _erc20Service                 = erc20Service;
        }

        public async Task<string> GetTransactionForSigning(EthTransaction ethTransaction)
        {
            string from = ethTransaction.FromAddress;

            var gas      = new Nethereum.Hex.HexTypes.HexBigInteger(ethTransaction.GasAmount);
            var gasPrice = new Nethereum.Hex.HexTypes.HexBigInteger(ethTransaction.GasPrice);
            var nonce    = await _nonceCalculator.GetNonceAsync(from);
            var to       = ethTransaction.ToAddress;
            var value    = new Nethereum.Hex.HexTypes.HexBigInteger(ethTransaction.Value);
            var tr       = new Nethereum.Signer.Transaction(to, value, nonce, gasPrice, gas);
            var hex      = tr.GetRLPEncoded().ToHex();

            return hex;
        }

        public async Task<OperationEstimationResult> EstimateTransactionExecutionCost(string from, string signedTrHex)
        {
            Nethereum.Signer.Transaction transaction = new Nethereum.Signer.Transaction(signedTrHex.HexToByteArray());
            var gasLimit                             = new HexBigInteger(transaction.GasLimit.ToHexCompact());
            var gasPrice                             = new HexBigInteger(transaction.GasPrice.ToHexCompact());
            var value                                = new HexBigInteger(transaction.Value.ToHexCompact());
            var to                                   = transaction.ReceiveAddress.ToHex().EnsureHexPrefix();
            var data                                 = transaction.Data.ToHex().EnsureHexPrefix();
            var callInput                            = new CallInput(data, to, from, gasLimit, gasLimit);
            callInput.GasPrice                       = gasLimit;

            var callResult = await _web3.Eth.Transactions.Call.SendRequestAsync(callInput);
            var response = await _web3.Eth.Transactions.EstimateGas.SendRequestAsync(callInput);

            return new OperationEstimationResult()
            {
                GasAmount = response.Value,
                IsAllowed = response.Value < gasLimit.Value || response.Value == Constants.DefaultTransactionGas
            };
        }

        public async Task<string> SubmitSignedTransaction(string from, string signedTrHex)
        {
            await _transactionValidationService.ValidateInputForSignedAsync(from, signedTrHex);
            string transactionHex = await _rawTransactionSubmitter.SubmitSignedTransaction(from, signedTrHex);

            return transactionHex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        /// <exception cref="ClientSideException">Throws client side exception</exception>
        public async Task ValidateInputAsync(EthTransaction transaction)
        {
            await _transactionValidationService.ValidateAddressBalanceAsync(transaction.FromAddress, transaction.Value, transaction.GasAmount, transaction.GasPrice);
        }
    }
}
