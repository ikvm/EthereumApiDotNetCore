﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Core;
using Core.Repositories;
using Core.Settings;
using EdjCase.JsonRpc.Client;
using Lykke.JobTriggers.Triggers.Attributes;
using Lykke.JobTriggers.Triggers.Bindings;
using Services;
using Services.Coins;
using Services.New.Models;

namespace EthereumJobs.Job
{
    public class MonitoringOperationJob
    {
        private readonly ICoinEventService        _coinEventService;
        private readonly IEventTraceRepository    _eventTraceRepository;
        private readonly IExchangeContractService _exchangeContractService;
        private readonly ILog                     _log;
        private readonly IPendingOperationService _pendingOperationService;
        private readonly IBaseSettings            _settings;
        private readonly ITransferContractService _transferContractService;



        public MonitoringOperationJob(
            ILog log,
            IBaseSettings settings,
            IPendingOperationService pendingOperationService,
            IExchangeContractService exchangeContractService,
            ICoinEventService coinEventService,
            ITransferContractService transferContractService,
            IEventTraceRepository eventTraceRepository)
        {
            _eventTraceRepository    = eventTraceRepository;
            _exchangeContractService = exchangeContractService;
            _pendingOperationService = pendingOperationService;
            _settings                = settings;
            _log                     = log;
            _coinEventService        = coinEventService;
            _transferContractService = transferContractService;
        }

        [QueueTrigger(Constants.PendingOperationsQueue, 100, true)]
        public async Task Execute(OperationHashMatchMessage opMessage, QueueTriggeringContext context)
        {
            await ProcessOperation(opMessage, context, _exchangeContractService.Transfer);
        }

        public async Task ProcessOperation(
            OperationHashMatchMessage opMessage,
            QueueTriggeringContext context,
            Func<Guid, string, string, string, BigInteger, string, Task<string>> transferDelegate)
        {
            try
            {
                var operation = await _pendingOperationService.GetOperationAsync(opMessage.OperationId);
                var guid      = Guid.Parse(operation.OperationId);
                var amount    = BigInteger.Parse(operation.Amount);

                BigInteger resultAmount;
                string transactionHash = null;
                CoinEventType? eventType = null;
                var currentBalance =
                    await _transferContractService.GetBalanceOnAdapter(operation.CoinAdapterAddress,
                        operation.FromAddress);

                switch (operation.OperationType)
                {
                    case OperationTypes.Cashout:
                        eventType = CoinEventType.CashoutStarted;
                        resultAmount = amount;
                        if (!CheckBalance(currentBalance, resultAmount)) break;
                        transactionHash = await _exchangeContractService.CashOut(guid,
                            operation.CoinAdapterAddress,
                            operation.FromAddress,
                            operation.ToAddress, amount, operation.SignFrom);
                        break;
                    case OperationTypes.Transfer:
                        eventType = CoinEventType.TransferStarted;
                        resultAmount = amount;
                        if (!CheckBalance(currentBalance, resultAmount)) break;
                        transactionHash = await transferDelegate(guid, operation.CoinAdapterAddress,
                            operation.FromAddress,
                            operation.ToAddress, amount, operation.SignFrom);
                        break;
                    case OperationTypes.TransferWithChange:
                        eventType = CoinEventType.TransferStarted;
                        var change = BigInteger.Parse(operation.Change);
                        resultAmount = amount - change;
                        if (!CheckBalance(currentBalance, resultAmount)) break;
                        transactionHash = await _exchangeContractService.TransferWithChange(guid,
                            operation.CoinAdapterAddress,
                            operation.FromAddress,
                            operation.ToAddress, amount, operation.SignFrom, change, operation.SignTo);
                        break;
                    default:
                        await _log.WriteWarningAsync("MonitoringOperationJob", "Execute",
                            $"Can't find right operation type for {opMessage.OperationId}", "");
                        break;
                }

                if (transactionHash != null && eventType != null)
                {
                    await _pendingOperationService.MatchHashToOpId(transactionHash, operation.OperationId);
                    await _coinEventService.PublishEvent(new CoinEvent(operation.OperationId, transactionHash,
                        operation.FromAddress, operation.ToAddress,
                        resultAmount.ToString(), eventType.Value, operation.CoinAdapterAddress));
                    await _eventTraceRepository.InsertAsync(new EventTrace
                    {
                        Note =
                            $"Operation Processed. Put it in the {Constants.TransactionMonitoringQueue}. With hash {transactionHash}",
                        OperationId = operation.OperationId,
                        TraceDate = DateTime.UtcNow
                    });

                    return;
                }
            }
            catch (RpcClientException exc)
            {
                await _log.WriteErrorAsync("MonitoringOperationJob", "Execute", "RpcException", exc);
                opMessage.LastError = exc.Message;
                opMessage.DequeueCount++;
                if (opMessage.DequeueCount < 6)
                {
                    context.MoveMessageToEnd(opMessage.ToJson());
                    context.SetCountQueueBasedDelay(_settings.MaxQueueDelay, 200);
                }
                else
                {
                    context.MoveMessageToPoison(opMessage.ToJson());
                }

                return;
            }
            catch (Exception ex)
            {
                if (ex.Message != opMessage.LastError)
                    await _log.WriteWarningAsync("MonitoringOperationJob", "Execute",
                        $"OperationId: [{opMessage.OperationId}]", "");

                opMessage.LastError = ex.Message;
                opMessage.DequeueCount++;
                context.MoveMessageToPoison(opMessage.ToJson());

                await _log.WriteErrorAsync("MonitoringOperationJob", "Execute", "", ex);

                return;
            }

            opMessage.DequeueCount++;
            context.MoveMessageToEnd(opMessage.ToJson());
            context.SetCountQueueBasedDelay(_settings.MaxQueueDelay, 200);
        }

        private bool CheckBalance(BigInteger currentBalance, BigInteger amount)
        {
            return currentBalance >= amount;
        }
    }
}