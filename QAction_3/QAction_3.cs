using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Skyline.DataMiner.Net.SLDataGateway.Types;
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
            CalculateInterfaceSpeed(protocol);
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    public static void CalculateInterfaceSpeed(SLProtocol protocol)
    {
        uint dividerValueForBpsToMbs = 1000000;

        var columnInterfacesSpeed = protocol.GetColumn(Parameter.Interfacestable.tablePid, Parameter.Interfacestable.Idx.interfacesiftablespeed_103);
        var columnInterfaceHighSpeed = protocol.GetColumn(Parameter.Extendedinterfacetable.tablePid, Parameter.Extendedinterfacetable.Idx.extendedinterfaceifxifhighspeed_202);

        List<object> rows = new List<object>();
        Dictionary<int, List<object>> columnValues = new Dictionary<int, List<object>>();


        for (int i = 0; i<columnInterfacesSpeed.Length; i++)
        {
            var convertToUint32 = Convert.ToUInt32(columnInterfacesSpeed[i]);

            if (convertToUint32<UInt32.MaxValue)
            {
                var calculation = convertToUint32/dividerValueForBpsToMbs;
                rows.Add(calculation);
            }
            else
            {
                rows.Add(columnInterfaceHighSpeed[i]);
            }
        }

        columnValues[Parameter.Interfacestable.tablePid] = protocol.GetColumn(Parameter.Interfacestable.tablePid, Parameter.Interfacestable.Idx.interfacesiftableindex_101).ToList();
        columnValues[Parameter.Interfacestable.indexColumnPid+Parameter.Interfacestable.Idx.interfacescalculatedspeed_105] = rows;
        protocol.SetColumns(columnValues);
    }
}