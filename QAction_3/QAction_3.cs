using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;

/// <summary>
/// DataMiner QAction Class: QActionName.
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

			var column=protocol.GetColumn(Parameter.Interfacestable.tablePid, Parameter.Interfacestable.Idx.interfacesiftablespeed_103);

			foreach (var item in column)
			{
                protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Data from parameter:{Environment.NewLine}{item}", LogType.Information, LogLevel.NoLogging);
            }

			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|QAction for calculation started", LogType.Information, LogLevel.NoLogging);			
        }
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}