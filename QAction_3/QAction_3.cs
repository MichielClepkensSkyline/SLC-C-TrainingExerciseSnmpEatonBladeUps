using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Skyline.DataMiner.Scripting;

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
			protocol.Log($"QA{protocol.QActionID}|Starting to retrieve columns data.", LogType.Information, LogLevel.NoLogging);

			object[] columnsData = (object[])protocol.NotifyProtocol(
				321,
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

			// protocol.Log($"QA{protocol.QActionID}|Successfully retrieved columns data, starting to calculate interface speed.", LogType.Error, LogLevel.NoLogging);

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

			// protocol.Log($"QA{protocol.QActionID}|Successfully calculated and set Calculated Interface Speed", LogType.Error, LogLevel.NoLogging);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}