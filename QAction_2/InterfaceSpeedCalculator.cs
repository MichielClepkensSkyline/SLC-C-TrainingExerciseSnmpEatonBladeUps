namespace QAction_2
{
	using System;

	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Scripting;

	using Parameter = Skyline.DataMiner.Scripting.Parameter;

	internal class InterfaceSpeedCalculator
	{
		private string[] primaryKeysString;
		private object[] calculatedSpeed;
		private bool nullDescriptionExists;

		internal TableColumnsData FetchingTableColumnsData(SLProtocolExt protocol, int parameterId)
		{
			object[] columnsData = (object[])protocol.NotifyProtocol(
			(int)NotifyType.NT_GET_TABLE_COLUMNS,
			parameterId,
			new uint[]
			{
			Parameter.Interfacestable.Idx.interfacesindex_51,
			Parameter.Interfacestable.Idx.interfacesdescription_52,
			Parameter.Interfacestable.Idx.interfacesspeed_54,
			Parameter.Interfacestable.Idx.interfacesextendedspeed_56,
			});

			if (columnsData.Length != 4)
			{
				return new TableColumnsData
				{
					ErrorLogMessage = $"Unexpected number of columns retrieved. Expected 4, got {columnsData.Length}.",
					PrimaryKeys = new object[0],
					ColumnDescription = new object[0],
					ColumnSpeed = new object[0],
					ColumnExtendedSpeed = new object[0],
				};
			}

			return new TableColumnsData
			{
				ErrorLogMessage = string.Empty,
				PrimaryKeys = (object[])columnsData[0],
				ColumnDescription = (object[])columnsData[1],
				ColumnSpeed = (object[])columnsData[2],
				ColumnExtendedSpeed = (object[])columnsData[3],
			};
		}

		internal void CalculatingTableColumns(TableColumnsData tableColumnsData)
		{
			var numofRows = tableColumnsData.PrimaryKeys.Length;
			primaryKeysString = new string[numofRows];
			calculatedSpeed = new object[numofRows];
			nullDescriptionExists = false;

			for (int i = 0; i < numofRows; i++)
			{
				uint speedValue = Convert.ToUInt32(tableColumnsData.ColumnSpeed[i]);
				calculatedSpeed[i] = speedValue < UInt32.MaxValue ? Convert.ToDouble(tableColumnsData.ColumnSpeed[i]) / 1_000_000 : tableColumnsData.ColumnExtendedSpeed[i];
				primaryKeysString[i] = Convert.ToString(tableColumnsData.PrimaryKeys[i]);

				if (tableColumnsData.ColumnDescription[i] == null || string.IsNullOrEmpty(Convert.ToString(tableColumnsData.ColumnDescription[i])))
				{
					nullDescriptionExists = true;
					tableColumnsData.ColumnDescription[i] = QAction.NotAvailableString;
				}
			}
		}

		internal void SettingTableColumns(SLProtocolExt protocol, object[] columnDescription)
		{
			if (nullDescriptionExists)
			{
				protocol.interfacestable.SetColumn(Parameter.Interfacestable.Pid.interfacesdescription_52, primaryKeysString, columnDescription);
			}

			protocol.interfacestable.SetColumn(Parameter.Interfacestable.Pid.interfacescalculatedspeed_57, primaryKeysString, calculatedSpeed);
		}
	}
}
