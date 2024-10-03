using System;

using QAction_2;

using Skyline.DataMiner.Scripting;

using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
	internal const string NotAvailableString = "-1";

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
    {
        try
        {
			int triggerPID = protocol.GetTriggerParameter();

			if (triggerPID == Parameter.Interfacestable.tablePid)
			{
				HandleTableDataCalculations(protocol, triggerPID);
			}
			else
			{
				HandleParameterNullValues(protocol);
			}
		}
		catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
	}

	private static void HandleParameterNullValues(SLProtocolExt protocol)
	{
		var paramIds = new UInt32[]
			{
				Parameter.systemdescription_10,
				Parameter.upsdevicemanufacturer_13,
				Parameter.upsdeviceidentitymodel_14,
			};
		var paramIdsInt = new int[paramIds.Length];
		var paramValues = (object[])protocol.GetParameters(paramIds);

		for (int i = 0; i < paramValues.Length; i++)
		{
			if (string.IsNullOrWhiteSpace(Convert.ToString(paramValues[i])))
			{
				paramValues[i] = NotAvailableString;
			}

			paramIdsInt[i] = (int)paramIds[i];
		}

		protocol.SetParameters(paramIdsInt, paramValues);
	}

	private static void HandleTableDataCalculations(SLProtocolExt protocol, int parameterId)
	{
		var interfaceSpeedCalculator = new InterfaceSpeedCalculator();
		var tableData = interfaceSpeedCalculator.FetchingTableColumnsData(protocol, parameterId);

		if (!string.IsNullOrEmpty(tableData.ErrorLogMessage))
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|HandleTableDataCalculations|{tableData.ErrorLogMessage}", LogType.Error, LogLevel.NoLogging);
			return;
		}

		interfaceSpeedCalculator.CalculatingTableColumns(tableData);

		interfaceSpeedCalculator.SettingTableColumns(protocol, tableData.ColumnDescription);
	}
}
