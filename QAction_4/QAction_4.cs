using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Skyline.DataMiner.Scripting;
using SLNetMessages = Skyline.DataMiner.Net.Messages;

/// <summary>
/// DataMiner QAction Class: qname.
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
			var tableNameColumnsToGetIDXs = new uint[] { Parameter.Iftable.Idx.iftablespeed_204, Parameter.Iftable.Idx.ifcalculatedspeed_206};
			object[] columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Iftable.tablePid, tableNameColumnsToGetIDXs);

			object[] speed = (object[])columns[0];
			object[] calcSpeed = (object[])columns[1];

			for (int i = 0; i < speed.Length; i++)
			{
				if (Convert.ToInt64(speed[i])>=Int32.MaxValue)
					speed[i] = calcSpeed[i];
			}

            protocol.iftable.SetColumn(Parameter.Iftable.Pid.iftablespeed_204, protocol.iftable.Keys, speed);

         }
        catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
