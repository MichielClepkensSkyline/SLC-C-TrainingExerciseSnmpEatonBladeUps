namespace Skyline.DataMiner.Scripting
{
    using System;
    using Skyline.DataMiner.Net.Messages;

    public class InterfaceSpeedCalculator
    {
        private string[] primaryKeysString;
        private object[] calculatedSpeed;
        private bool nullDescriptionExists;

        public void CalculatingTableColumns(TableColumnsData tableColumnsData)
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

        public TableColumnsData FetchingTableColumnsData(SLProtocolExt protocol, int parameterId)
        {
            object[] columnsData = (object[])protocol.NotifyProtocol(
            (int)NotifyType.NT_GET_TABLE_COLUMNS,
            parameterId,
            new uint[]
            {
                Parameter.Interfacetable.Idx.interfaceindex_51,
                Parameter.Interfacetable.Idx.interfacedescription_52,
                Parameter.Interfacetable.Idx.interfacespeed_54,
                Parameter.Interfacetable.Idx.interfaceextendedspeed_56,
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

        public void SettingTableColumns(SLProtocolExt protocol, object[] columnDescription)
        {
            if (nullDescriptionExists)
            {
                protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacedescription_52, primaryKeysString, columnDescription);
            }

            protocol.interfacetable.SetColumn(Parameter.Interfacetable.Pid.interfacecalculatedspeed_57, primaryKeysString, calculatedSpeed);
        }
    }
}
