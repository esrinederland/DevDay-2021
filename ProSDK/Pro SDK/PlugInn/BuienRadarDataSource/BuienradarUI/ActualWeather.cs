using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Data.PluginDatastore;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using BuienRadarSettings;

namespace BuienradarUI
{
	internal class ActualWeather : Button
	{
		protected async override void OnClick()
		{
			await QueuedTask.Run(() =>
			{
				ActualWeatherCore.ShowActualWeather();
			});
		}
	}
}