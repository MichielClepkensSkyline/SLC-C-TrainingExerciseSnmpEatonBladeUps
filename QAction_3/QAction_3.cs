using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;

/// <summary>
/// DataMiner QAction Class.
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
			// Enkel updaten van rij die QA heeft doen triggeren (op deze manier is de methode efficienter)
			string rowKey = protocol.RowKey();
			object speedObject = protocol.GetCell(protocol.interfacetable.TableId, rowKey, 2);
			uint speed = Convert.ToUInt32(speedObject);
			protocol.Log($"QA{protocol.QActionID}|RUN|{speed}", LogType.Error, LogLevel.NoLogging);
			if (speed == 4294967295)
			{
				speedObject = protocol.GetCell(protocol.extendedinterfacetable.TableId, rowKey, 1);
				speed = Convert.ToUInt32(speedObject);
				protocol.SetCell(protocol.interfacetable.TableId, rowKey, 4, speed);
				//protocol.Log($"QA{protocol.QActionID}|Run|ExtendedTable needed", LogType.Error, LogLevel.NoLogging);
			}
			else
			{
				protocol.SetCell(protocol.interfacetable.TableId, rowKey, 4, speed/1000000);
			}

			/*
			List<uint> interfaceSpeeds = new List<uint>();
            object[] speeds = protocol.GetColumn(protocol.interfacetable.TableId, 2);
            foreach (object sp in speeds)
			{
				uint speed = Convert.ToUInt32(sp);
				
			}

            protocol.Log($"QA{protocol.QActionID}|Run|{protocol.RowKey()}", LogType.Error, LogLevel.NoLogging);
            protocol.RowKey();
			//protocol.Log($"QA{protocol.QActionID}|Run|{interfaceSpeeds[0]}", LogType.Error, LogLevel.NoLogging);*/
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
