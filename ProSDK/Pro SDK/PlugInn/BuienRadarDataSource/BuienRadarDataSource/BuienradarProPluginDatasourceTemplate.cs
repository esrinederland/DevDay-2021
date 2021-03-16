using ArcGIS.Core.Data.PluginDatastore;
using System;
using System.Collections.Generic;
using System.Net;

namespace BuienRadarDataSource
{
	public class BuienradarProPluginDatasourceTemplate : PluginDatasourceTemplate
	{
		private BuienRadarActualProPluginTableTemplate ActualWeather { get; set; } = null;

		private WebClient Client { get; set; } 

		/// <summary>
		/// Open the dataconnection to the datasource
		/// </summary>
		/// <param name="connectionPath"></param>
		public override void Open(Uri connectionPath)
		{
			string weburl;
			if (connectionPath.IsFile)
			{
				weburl = connectionPath.ToString().Substring(connectionPath.ToString().IndexOf("https")).Replace(@"https:/", @"https://");
			}
			else
			{
				weburl = connectionPath.ToString();
			}

			Client = new WebClient();
			string webResponse = Client.DownloadString(weburl);

			ActualWeather = new BuienRadarActualProPluginTableTemplate(webResponse);
		}

		/// <summary>
		/// Close the connection, 
		/// </summary>
		public override void Close()
		{
			Client?.Dispose();
			Client = null;
			ActualWeather = null;
		}

		/// <summary>
		/// Switch for selection the table.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override PluginTableTemplate OpenTable(string name)
		{
			switch (name)
			{
				case "Actuele weersituatie":
					return ActualWeather;
				default:
					return null;
			}
		}

		/// <summary>
		/// Create a list of available tables, in this case, the actual weather information. 
		/// </summary>
		/// <returns></returns>
		public override IReadOnlyList<string> GetTableNames()
		{
			List<string> tableNames = new List<string>();

			tableNames.Add("Actuele weersituatie");

			return tableNames;
		}

		public override bool IsQueryLanguageSupported()
		{
			return false;
		}
	}
}