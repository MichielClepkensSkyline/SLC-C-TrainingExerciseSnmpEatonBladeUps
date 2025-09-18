using System;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: Calculate Interface Speed.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			string rowPK = protocol.RowKey();
			int oneBasedOffSet = 1;

			var interfaceSpeed = GetInterfaceSpeed(protocol, rowPK, oneBasedOffSet);

			var extendeSpeed = protocol.GetParameterIndexByKey(Parameter.Extendedinterfacetable.tablePid, rowPK, Parameter.Extendedinterfacetable.Idx.extendedinterfacetablehighspeed_1402 + 1);
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}| this is the interface row key: {rowPK} and speed: {interfaceSpeed}  and extended: {extendeSpeed}", LogType.Error, LogLevel.NoLogging);

			uint calculatedSpeed = GetCalculatedInterfaceSpeed(protocol, rowPK, interfaceSpeed, oneBasedOffSet);

			bool hasSucceeded = protocol.SetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablecalculatedinterfacespeed_1205 + oneBasedOffSet, calculatedSpeed);

			if (!hasSucceeded)
			{
				protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Parameter set was not successful", LogType.Error, LogLevel.NoLogging);
			}

		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static uint GetInterfaceSpeed(SLProtocol protocol, string rowPK, int offset)
	{
		var interfaceSpeed = protocol.GetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablespeed_1203 + offset);

		return Convert.ToUInt32(interfaceSpeed);
	}

	private static uint GetCalculatedInterfaceSpeed(SLProtocol protocol, string rowPK, uint interfaceSpeed, int offset)
	{
		uint unsignedIntegerMaxValue = UInt32.MaxValue;
		if (interfaceSpeed < unsignedIntegerMaxValue)
		{
			interfaceSpeed = interfaceSpeed / 1000000;
		}
		else if(interfaceSpeed == unsignedIntegerMaxValue)
		{
			var extendedSpeed = protocol.GetParameterIndexByKey(Parameter.Extendedinterfacetable.tablePid, rowPK, Parameter.Extendedinterfacetable.Idx.extendedinterfacetablehighspeed_1402 + offset);

			interfaceSpeed = Convert.ToUInt32(extendedSpeed);
		}

		return interfaceSpeed;
	}
}