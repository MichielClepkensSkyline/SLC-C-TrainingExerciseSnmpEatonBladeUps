using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Skyline.DataMiner.Net;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;

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
			//string rowPK = protocol.RowKey();
			//var interfaceSpeed = protocol.GetCell(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablespeed_2003);
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}| this is the interface row key: and speed:", LogType.Error, LogLevel.NoLogging);

			var unsignedIntegerMaxValue = UInt32.MaxValue;

			/*if (Convert.ToUInt32(interfaceSpeed) < unsignedIntegerMaxValue)
			{

			}
			else
			{

			}*/
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}