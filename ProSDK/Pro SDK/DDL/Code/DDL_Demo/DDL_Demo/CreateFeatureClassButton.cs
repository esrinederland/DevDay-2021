using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using System.IO;

namespace DDL_Demo
{
	internal class CreateFeatureClassButton : Button
	{
		protected override async void OnClick()
		{
			string filename = @"D:\Projecten\GisTech\Git\Pro SDK\DDL\Data\WeerDemo.gdb";
			if (Directory.Exists(filename))
			{
				MessageBox.Show("WeerDemo.gdb bestaat al");
			}
			else
			{
				await QueuedTask.Run(() =>
				{
					try
					{
						DDLLogic.Create(filename);
					}
					catch
					{
						MessageBox.Show("Fout bij het aanmaken.");
					}
				});
			}
		}
	}
}