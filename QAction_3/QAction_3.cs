using System;

using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Scripting;

using Parameter = Skyline.DataMiner.Scripting.Parameter;

/// <summary>
/// DataMiner QAction Class: Calculate Speed.
/// </summary>
public static class QAction
{
	private const uint MaxIntegerValue = 4294967295;

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			object[] columnsData = (object[])protocol.NotifyProtocol(
				(int)NotifyType.NT_GET_TABLE_COLUMNS,
				Parameter.Interfacetable.tablePid,
				new uint[]
				{
					Parameter.Interfacetable.Idx.interfaceindex_51,
					Parameter.Interfacetable.Idx.interfacespeed_54,
					Parameter.Interfacetable.Idx.interfaceextendedspeed_56,
				});

			if (columnsData.Length != 3)
			{
				protocol.Log($"QA{protocol.QActionID}|Unexpected number of columns retrieved. Expected 3, got {columnsData.Length}.", LogType.Error, LogLevel.NoLogging);
				return;
			}

			object[] primaryKeys = (object[])columnsData[0];
			object[] columnSpeed = (object[])columnsData[1];
			object[] columnExtendedSpeed = (object[])columnsData[2];

			var numofRows = primaryKeys.Length;

			string[] primaryKeysString = new string[numofRows];
			object[] calculatedSpeed = new object[numofRows];

			for (int i = 0; i < numofRows; i++)
			{
				uint speedValue = Convert.ToUInt32(columnSpeed[i]);
				calculatedSpeed[i] = speedValue < MaxIntegerValue ? Convert.ToDouble(columnSpeed[i])/1_000_000 : columnExtendedSpeed[i];
				primaryKeysString[i] = Convert.ToString(primaryKeys[i]);
			}

			protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacecalculatedspeed_57, primaryKeysString, calculatedSpeed);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}