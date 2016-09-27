﻿using System;
using AzureRepositories.Azure.Queue;
using AzureRepositories.Azure.Tables;
using AzureRepositories.Log;
using AzureRepositories.Repositories;
using Core;
using Core.Log;
using Core.Repositories;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace AzureRepositories
{
	public static class RegisterReposExt
	{
		public static void RegisterAzureLogs(this IServiceCollection services, IBaseSettings settings, string logPrefix)
		{
			var logToTable = new LogToTable(
				new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, logPrefix + "Error", null),
				new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, logPrefix + "Warning", null),
				new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, logPrefix + "Info", null));

			services.AddSingleton(logToTable);
			services.AddTransient<LogToConsole>();

			services.AddTransient<ILog, LogToTableAndConsole>();
		}

		public static void RegisterAzureStorages(this IServiceCollection services, IBaseSettings settings)
		{
			services.AddSingleton<IMonitoringRepository>(provider => new MonitoringRepository(
				new AzureTableStorage<MonitoringEntity>(settings.Db.ExchangeQueueConnString, "Monitoring",
					provider.GetService<ILog>())));

			services.AddSingleton<IUserContractRepository>(provider => new UserContractRepository(
				new AzureTableStorage<UserContractEntity>(settings.Db.DataConnString, "UserContracts",
					provider.GetService<ILog>())));

			services.AddSingleton<IAppSettingsRepository>(provider => new AppSettingsRepository(
				new AzureTableStorage<AppSettingEntity>(settings.Db.DataConnString, "AppSettings",
					provider.GetService<ILog>())));
		}

		public static void RegisterAzureQueues(this IServiceCollection services, IBaseSettings settings)
		{
			services.AddTransient<Func<string, IQueueExt>>(provider =>
			{
				return (x =>
				{
					switch (x)
					{
						case Constants.EthereumContractQueue:
							return new AzureQueueExt(settings.Db.DataConnString, x);
						case Constants.EthereumOutQueue:
							return new AzureQueueExt(settings.Db.EthereumNotificationsConnString, x);
						case Constants.EmailNotifierQueue:
							return new AzureQueueExt(settings.Db.ExchangeQueueConnString, x);
						case Constants.ContractTransferQueue:
							return new AzureQueueExt(settings.Db.DataConnString, x);
						default:
							throw new Exception("Queue is not registered");
					}
				});
			});

		}
	}
}
