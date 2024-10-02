using System;

using Skyline.DataMiner.Net.Messages;
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
			protocol.Log($"QA{protocol.QActionID}|Unexpected number of columns retrieved. Expected 4, got {columnsData.Length}.", LogType.Error, LogLevel.NoLogging);
			return;
		}

		object[] primaryKeys = (object[])columnsData[0];
		object[] description = (object[])columnsData[1];
		object[] columnSpeed = (object[])columnsData[2];
		object[] columnExtendedSpeed = (object[])columnsData[3];

		var numofRows = primaryKeys.Length;
		string[] primaryKeysString = new string[numofRows];
		object[] calculatedSpeed = new object[numofRows];
		object[] descriptionWithoutNulls = description;
		var nullDescriptionExists = false;

		for (int i = 0; i < numofRows; i++)
		{
			uint speedValue = Convert.ToUInt32(columnSpeed[i]);
			calculatedSpeed[i] = speedValue < MaxIntegerValue ? Convert.ToDouble(columnSpeed[i]) / 1_000_000 : columnExtendedSpeed[i];
			primaryKeysString[i] = Convert.ToString(primaryKeys[i]);

			if (description[i] == null || string.IsNullOrEmpty(Convert.ToString(description[i])))
			{
				nullDescriptionExists = true;
				descriptionWithoutNulls[i] = NotAvailableString;
			}
		}

		if (nullDescriptionExists)
		{
			protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacedescription_52, primaryKeysString, descriptionWithoutNulls);
		}

		protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacecalculatedspeed_57, primaryKeysString, calculatedSpeed);
	}

}
