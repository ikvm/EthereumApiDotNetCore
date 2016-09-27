﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureRepositories.Azure.Queue;
using Core;
using Newtonsoft.Json;

namespace Services
{
	public class ContractTransferTransaction
	{
		public string TransactionHash { get; set; }

		public string Contract { get; set; }

		public decimal Amount { get; set; }

		public DateTime CreateDt { get; set; }
	}


	public interface IContractTransferTransactionService
	{
		Task PutContractTransferTransaction(ContractTransferTransaction tr);

		/// <summary>
		/// Returns true if transaction completed successfuly
		/// </summary>		
		Task<bool> CompleteTransaction();		
	}

	public class ContractTransferTransactionService : IContractTransferTransactionService
	{
		private readonly IPaymentService _paymentService;
		private readonly IEthereumQueueOutService _queueOutService;
		private IQueueExt _queue;

		public ContractTransferTransactionService(IPaymentService paymentService, Func<string, IQueueExt> queueFactory, IEthereumQueueOutService queueOutService)
		{
			_paymentService = paymentService;
			_queueOutService = queueOutService;
			_queue = queueFactory(Constants.ContractTransferQueue);
		}

		public async Task PutContractTransferTransaction(ContractTransferTransaction tr)
		{
			await _queue.PutRawMessageAsync(JsonConvert.SerializeObject(tr));
		}

		public async Task<bool> CompleteTransaction()
		{
			var item = await _queue.PeekRawMessageAsync();

			var contractTransferTr = JsonConvert.DeserializeObject<ContractTransferTransaction>(item.AsString);

			if (_paymentService.GetTransactionReceipt(contractTransferTr.TransactionHash) != null)
			{
				await _queueOutService.FirePaymentEvent(contractTransferTr.Contract, contractTransferTr.Amount,
					contractTransferTr.TransactionHash);
				await _queue.FinishRawMessageAsync(item);
				return true;
			}
			return false;			
		}
	}
}