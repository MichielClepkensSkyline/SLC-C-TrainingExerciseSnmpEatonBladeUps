using System;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
    public static string NotAvailableString => "-1";

    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocolExt protocol)
    {
        try
        {
            int triggerPID = protocol.GetTriggerParameter();

            if (triggerPID == Parameter.Interfacetable.tablePid)
            {
                HandleTableDataCalculations(protocol, triggerPID);
            }
            else
            {
                HandleParameterNullValues(protocol, triggerPID);
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    private static void HandleParameterNullValues(SLProtocolExt protocol, int triggerPID)
    {
        var paramValue = Convert.ToString(protocol.GetParameter(triggerPID));

        if (string.IsNullOrWhiteSpace(paramValue))
        {
            protocol.SetParameter(triggerPID, NotAvailableString);
        }
    }

    private static void HandleTableDataCalculations(SLProtocolExt protocol, int parameterId)
    {
        InterfaceSpeedCalculator interfaceSpeedCalculator = new InterfaceSpeedCalculator();
        var tableData = interfaceSpeedCalculator.FetchingTableColumnsData(protocol, parameterId);

        if (!string.IsNullOrEmpty(tableData.ErrorLogMessage))
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|HandleTableDataCalculations|{tableData.ErrorLogMessage}", LogType.Error, LogLevel.NoLogging);
            return;
        }

        interfaceSpeedCalculator.CalculatingTableColumns(tableData);
        interfaceSpeedCalculator.SettingTableColumns(protocol, tableData.ColumnDescription);
    }
}
