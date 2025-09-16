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
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			string rowPK = protocol.RowKey();
			var interfaceSpeed = protocol.GetCell(Parameter.Interfacetable.tablePid, rowPK, Parameter.Interfacetable.Idx.interfacetablespeed_2003);
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}| this is the interface row key: {rowPK} and speed: {Convert.ToUInt32(interfaceSpeed)}", LogType.Error, LogLevel.NoLogging);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}