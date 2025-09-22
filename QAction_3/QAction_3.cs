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
	/// This method only updates one row at a time.
	/// Performance wise this is best in case many rows get updated not to often.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocolExt protocol)
	{
		try
		{
			// Get the changed column and calculate the speed
            string rowKey = protocol.RowKey();
            uint speed = CalculateSpeed(protocol, rowKey);

            bool hasSucceeded = protocol.SetCell(protocol.interfacetable.TableId, rowKey, Parameter.Interfacetable.Idx.interfacetablecalculatedspeed, speed);
            if (!hasSucceeded)
			{
				protocol.Log($"QA{protocol.QActionID}|Run|The calculated speed was not set correctly", LogType.Error, LogLevel.NoLogging);
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static UInt32 CalculateSpeed(SLProtocolExt protocol, string rowKey)
	{
        object speedObject = protocol.GetCell(protocol.interfacetable.TableId, rowKey, Parameter.Interfacetable.Idx.interfacetablespeed);
        uint speed = Convert.ToUInt32(speedObject);

        if (speed == UInt32.MaxValue)
        {
            speedObject = protocol.GetCell(protocol.interfacetable.TableId, rowKey, Parameter.Interfacetable.Idx.interfacetableifhighspeed);
            speed = Convert.ToUInt32(speedObject);
        }
		else
		{
			speed = ConvertbpsToMbps(speed);
		}

        return speed;
    }

	private static UInt32 ConvertbpsToMbps(UInt32 speed)
	{
		uint convertBpsToMbps = 1000000;
		return speed / convertBpsToMbps;

    }
}
