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

			var interfaceSpeed = protocol.GetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablespeed_2003 + oneBasedOffSet);
			uint convertedSpeed = Convert.ToUInt32(interfaceSpeed);

			var extendeSpeed = protocol.GetParameterIndexByKey(Parameter.Extendedinterfacetable.tablePid, rowPK, Parameter.Extendedinterfacetable.Idx.extendedinterfacetablehighspeed_4002 + 1);
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}| this is the interface row key: {rowPK} and speed: {interfaceSpeed}  and extended: {extendeSpeed}", LogType.Error, LogLevel.NoLogging);

			var unsignedIntegerMaxValue = UInt32.MaxValue;

			if (convertedSpeed < unsignedIntegerMaxValue)
			{
				bool hasSucceeded = protocol.SetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablecalculatedinterfacespeed_2005 + oneBasedOffSet, convertedSpeed);

				if (!hasSucceeded)
				{
					protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown: Parameter set for Calculated Interface Speed was not successful", LogType.Error, LogLevel.NoLogging);
				}
			}
			else if (convertedSpeed == unsignedIntegerMaxValue)
			{
				var extendedSpeed = protocol.GetParameterIndexByKey(Parameter.Extendedinterfacetable.tablePid, rowPK, Parameter.Extendedinterfacetable.Idx.extendedinterfacetablehighspeed_4002 + oneBasedOffSet);

				bool hasSucceeded = protocol.SetParameterIndexByKey(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablecalculatedinterfacespeed_2005 + oneBasedOffSet, extendedSpeed);

				if (!hasSucceeded)
				{
					protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown: Parameter set for Extended Speed was not successful", LogType.Error, LogLevel.NoLogging);
				}
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}