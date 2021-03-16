using System;
using System.Collections.Generic;
using ArcGIS.Core.Data;
using ArcGIS.Core.Internal.Data.DDL;
using ArcGIS.Desktop.Framework.Dialogs;

namespace DDL_Demo
{
	public static class DDLLogic
	{
		public static void Create(string geodatabasePath)
		{
			// Create a File Geodatabase with the given path
			Geodatabase weatherGDB = SchemaBuilder.CreateGeodatabase(new FileGeodatabaseConnectionPath(new Uri(geodatabasePath)));

			// This static helper routine creates a FieldDescription for a GlobalID field with default values
			FieldDescription globalIDFieldDescription = FieldDescription.CreateGlobalIDField();

			// This static helper routine creates a FieldDescription for an ObjectID field with default values
			FieldDescription objectIDFieldDescription = FieldDescription.CreateObjectIDField();

			// Create a FieldDescription for the minimal temperature field
			FieldDescription minTempFieldDescription = new FieldDescription("Min", FieldType.Double)
			{
				AliasName = "Minimum temperatuur"
			};

			// Create a FieldDescription for the maximum temperature field
			FieldDescription maxTempFieldDescription = new FieldDescription("Max", FieldType.Double)
			{
				AliasName = "Maximum temperatuur"
			};

			// This static helper routine creates a string field
			FieldDescription weatherTypeFieldDescription = FieldDescription.CreateStringField("WeatherType", 512);
			weatherTypeFieldDescription.AliasName = "Weertype";

			// This static helper routine creates a string field
			FieldDescription dayOfWeekFieldDescription = FieldDescription.CreateStringField("Day", 25);
			dayOfWeekFieldDescription.AliasName = "Dag van de week";

			// Assemble a list of all of our field descriptions
			List<FieldDescription> fieldDescriptions = new List<FieldDescription>()
			{ 
				globalIDFieldDescription, 
				objectIDFieldDescription,
				dayOfWeekFieldDescription,
				minTempFieldDescription,
				maxTempFieldDescription,
				weatherTypeFieldDescription
			};

			// Create a TableDescription object to describe the table to create
			TableDescription tableDescription = new TableDescription("Vooruitzichten", fieldDescriptions);

			// Create a SchemaBuilder object
			SchemaBuilder schemaBuilder = new SchemaBuilder(weatherGDB);

			// Add the creation of the weatherforecast featureclass to our list of DDL tasks
			schemaBuilder.Create(tableDescription);

			// Execute the DDL
			bool success = schemaBuilder.Build();

			// Inspect error messages, and show an error.
			if (!success)
			{
				IReadOnlyList<string> errorMessages = schemaBuilder.ErrorMessages;
				foreach (string errorMessage in errorMessages)
				{
					MessageBox.Show($"Fout bij het aanmaken {errorMessage}");
				}
			}
		}
	}
}