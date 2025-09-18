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

			uint interfaceSpeed = GetInterfaceSpeed(protocol, rowPK, oneBasedOffSet);
			uint calculatedSpeed = GetCalculatedInterfaceSpeed(protocol, rowPK, interfaceSpeed, oneBasedOffSet);

			bool hasSucceeded = protocol.SetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablecalculatedinterfacespeed_1205 + oneBasedOffSet, calculatedSpeed);

			if (!hasSucceeded)
			{
				protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Parameter Set was not successful", LogType.Error, LogLevel.NoLogging);
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static uint GetInterfaceSpeed(SLProtocol protocol, string rowPK, int offset)
	{
		object interfaceSpeed = protocol.GetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablespeed_1203 + offset);

		return Convert.ToUInt32(interfaceSpeed);
	}

	private static uint GetCalculatedInterfaceSpeed(SLProtocol protocol, string rowPK, uint interfaceSpeed, int offset)
	{
		uint unsignedIntegerMaxValue = UInt32.MaxValue;
		uint dividerValueFrombpsToMbps = 1000000;

		if (interfaceSpeed < unsignedIntegerMaxValue)
		{
			interfaceSpeed = interfaceSpeed / dividerValueFrombpsToMbps;
		}
		else if(interfaceSpeed == unsignedIntegerMaxValue)
		{
			object extendedSpeed = protocol.GetParameterIndexByKey(Parameter.Extendedinterfacetable.tablePid, rowPK, Parameter.Extendedinterfacetable.Idx.extendedinterfacetablehighspeed_1402 + offset);

			interfaceSpeed = Convert.ToUInt32(extendedSpeed);
		}

		return interfaceSpeed;
	}
}