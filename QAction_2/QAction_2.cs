using System;

using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Net.SLDataGateway.Types;
using Skyline.DataMiner.Scripting;

using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
	private const string NotAvailableString = "-1";
	private const uint MaxIntegerValue = 4294967295;

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
    {
        try
        {
			int triggerPID = protocol.GetTriggerParameter();

			if (triggerPID == Parameter.Interfacetable.tablePid)
			{
				HandleTableDataCalculations(protocol, triggerPID);
			}
			else
			{
				HandleParameterNullValues(protocol, triggerPID);
			}
		}
		catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
	}

	private static void HandleParameterNullValues(SLProtocolExt protocol, int triggerPID)
	{
		var paramValue = Convert.ToString(protocol.GetParameter(triggerPID));

		if (string.IsNullOrWhiteSpace(paramValue))
		{
			protocol.SetParameter(triggerPID, NotAvailableString);
		}
	}

	private static void HandleTableDataCalculations(SLProtocolExt protocol, int parameterId)
	{
		var tableData = FetchingTableColumnsData(protocol, parameterId);

		if (!string.IsNullOrEmpty(tableData.ErrorLogMessage))
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|HandleTableDataCalculations|{tableData.ErrorLogMessage}", LogType.Error, LogLevel.NoLogging);
			return;
		}

		CalculatingTableColumns(tableData, out string[] primaryKeysString, out object[] calculatedSpeed, out bool nullDescriptionExists);

		SettingTableColumns(protocol, nullDescriptionExists, primaryKeysString, tableData.ColumnDescription, calculatedSpeed);
	}

	private static TableColumnsData FetchingTableColumnsData(SLProtocolExt protocol, int parameterId)
	{
		object[] columnsData = (object[])protocol.NotifyProtocol(
		(int)NotifyType.NT_GET_TABLE_COLUMNS,
		parameterId,
		new uint[]
		{
			Parameter.Interfacetable.Idx.interfaceindex_51,
			Parameter.Interfacetable.Idx.interfacedescription_52,
			Parameter.Interfacetable.Idx.interfacespeed_54,
			Parameter.Interfacetable.Idx.interfaceextendedspeed_56,
		});

		if (columnsData.Length != 4)
		{
			return new TableColumnsData
			{
				ErrorLogMessage = $"Unexpected number of columns retrieved. Expected 4, got {columnsData.Length}.",
				PrimaryKeys = new object[0],
				ColumnDescription = new object[0],
				ColumnSpeed = new object[0],
				ColumnExtendedSpeed = new object[0],
			};
		}

		return new TableColumnsData
		{
			ErrorLogMessage = string.Empty,
			PrimaryKeys = (object[])columnsData[0],
			ColumnDescription = (object[])columnsData[1],
			ColumnSpeed = (object[])columnsData[2],
			ColumnExtendedSpeed = (object[])columnsData[3],
		};
	}

	private static void CalculatingTableColumns(TableColumnsData tableColumnsData, out string[] primaryKeysString, out object[] calculatedSpeed, out bool nullDescriptionExists)
	{
		var numofRows = tableColumnsData.PrimaryKeys.Length;
		primaryKeysString = new string[numofRows];
		calculatedSpeed = new object[numofRows];
		nullDescriptionExists = false;

		for (int i = 0; i < numofRows; i++)
		{
			uint speedValue = Convert.ToUInt32(tableColumnsData.ColumnSpeed[i]);
			calculatedSpeed[i] = speedValue < MaxIntegerValue ? Convert.ToDouble(tableColumnsData.ColumnSpeed[i]) / 1_000_000 : tableColumnsData.ColumnExtendedSpeed[i];
			primaryKeysString[i] = Convert.ToString(tableColumnsData.PrimaryKeys[i]);

			if (tableColumnsData.ColumnDescription[i] == null || string.IsNullOrEmpty(Convert.ToString(tableColumnsData.ColumnDescription[i])))
			{
				nullDescriptionExists = true;
				tableColumnsData.ColumnDescription[i] = NotAvailableString;
			}
		}
	}

	private static void SettingTableColumns(SLProtocolExt protocol, bool nullDescriptionExists, string[] primaryKeysString, object[] columnDescription, object[] calculatedSpeed)
	{
		if (nullDescriptionExists)
		{
			protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacedescription_52, primaryKeysString, columnDescription);
		}

		protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacecalculatedspeed_57, primaryKeysString, calculatedSpeed);
	}

	public class TableColumnsData
	{
		public object[] PrimaryKeys { get; set; }

		public object[] ColumnDescription { get; set; }

		public object[] ColumnSpeed { get; set; }

		public object[] ColumnExtendedSpeed { get; set; }

		public string ErrorLogMessage { get; set; }
	}
}
