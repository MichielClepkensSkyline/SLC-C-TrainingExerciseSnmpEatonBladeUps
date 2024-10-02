using System;
using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class: After Startup.
/// </summary>
public static class QAction
{
	private const string NotAvailableString = "-1";

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
    {
        try
        {
			int triggerPID = protocol.GetTriggerParameter();
			var paramValue = Convert.ToString(protocol.GetParameter(triggerPID));

			if (paramValue is null || String.IsNullOrWhiteSpace(paramValue))
            {
                protocol.SetParameter(triggerPID, NotAvailableString);
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
