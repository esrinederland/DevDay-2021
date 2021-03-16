using ArcGIS.Core.CIM;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GraphicsLayerDemo
{
	internal class MouseInteraction : MapTool
	{
		private GraphicsLayer FieldOfJoy { get; set; } = null;

		public MouseInteraction()
		{
			IsSketchTool = false;
		}

		protected override void OnToolMouseDown(MapViewMouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case MouseButton.Right:
				case MouseButton.Left:
					e.Handled = true;
					break;
			}
		}

		protected override Task OnToolActivateAsync(bool hasMapViewChanged)
		{
			// Check if the current map is available and a 2D map.
			Map map = MapView.Active.Map;
			if (map.MapType != MapType.Map)
			{
				// Map isn't a 2d map.
				return null;
			}

			QueuedTask.Run(() =>
			{
				FieldOfJoy = map.Layers.FirstOrDefault(item => item.GetType() == typeof(GraphicsLayer) && item.Name == "Boter Kaas en Eieren") as GraphicsLayer;
				if (FieldOfJoy == null)
				{
					// Create a grapics layer
					GraphicsLayerCreationParams gl_param = new GraphicsLayerCreationParams { Name = "Boter Kaas en Eieren" };
					// By default will be added to the top of the TOC
					FieldOfJoy = LayerFactory.Instance.CreateLayer<GraphicsLayer>(gl_param, map);
				}

				// Get the centre
				var x =  MapView.Active.GetViewSize().Width / 2;
				var y = MapView.Active.GetViewSize().Height / 2;
				CreatePlayField(new System.Windows.Point(x, y), FieldOfJoy);
			});

			return base.OnToolActivateAsync(hasMapViewChanged);
		}

		protected override Task OnToolDeactivateAsync(bool hasMapViewChanged)
		{
			// Check if the current map is available and a 2D map.
			Map map = MapView.Active.Map;
			if (map.MapType != MapType.Map)
			{
				// Map isn't a 2d map.
				return null;
			}

			// Remove the layer.
			QueuedTask.Run(() =>
			{
				map.RemoveLayer(FieldOfJoy);
			});

			return base.OnToolDeactivateAsync(hasMapViewChanged);
		}

		protected override Task HandleMouseDownAsync(MapViewMouseButtonEventArgs e)
		{
			return QueuedTask.Run(() =>
			{
				// Get the mouse click point
				MapPoint location = MapView.Active.ClientToMap(e.ClientPoint);

				// Create a symbol based on the mouse button.
				CIMPointSymbol pointSymbol = null;
				if (e.ChangedButton == MouseButton.Left)
				{
					// Specify a symbol
					pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.CreateRGBColor(150, 0, 0, 60), 80, SimpleMarkerStyle.Circle);
				}
				else if (e.ChangedButton == MouseButton.Right)
				{
					// Specify a symbol
					pointSymbol = SymbolFactory.Instance.ConstructPointSymbol(ColorFactory.Instance.CreateRGBColor(0, 0, 150, 60), 80, SimpleMarkerStyle.Cross);
				}

				// Create a CIMGraphic to show the symbol on the map in the grapicslayer. 
				var graphic = new CIMPointGraphic()
				{
					Symbol = pointSymbol?.MakeSymbolReference(),
					Location = location
				};

				// Add the graphic to the grapicslayer. 
				FieldOfJoy.AddElement(graphic);

				// By default all items are selected, deselect all items
				FieldOfJoy.UnSelectElements();
			});
		}

		/// <summary>
		/// Create a tiktaktow play field, with 2 vertical en 2 horizontal lines.
		/// </summary>
		/// <param name="centre"></param>
		/// <param name="fieldOfJoy"></param>
		public void CreatePlayField(System.Windows.Point centre, GraphicsLayer fieldOfJoy)
		{
			MapPoint topLeft = MapView.Active.ClientToMap(new System.Windows.Point(centre.X - 200, centre.Y + 600));
			MapPoint bottomLeft = MapView.Active.ClientToMap(new System.Windows.Point(centre.X - 200, centre.Y - 600));

			MapPoint topRight = MapView.Active.ClientToMap(new System.Windows.Point(centre.X + 200, centre.Y + 600));
			MapPoint bottomRight = MapView.Active.ClientToMap(new System.Windows.Point(centre.X + 200, centre.Y - 600));

			MapPoint rightTop = MapView.Active.ClientToMap(new System.Windows.Point(centre.X + 600, centre.Y + 200));
			MapPoint leftTop = MapView.Active.ClientToMap(new System.Windows.Point(centre.X - 600, centre.Y + 200));

			MapPoint rightBottom = MapView.Active.ClientToMap(new System.Windows.Point(centre.X + 600, centre.Y - 200));
			MapPoint leftButtom = MapView.Active.ClientToMap(new System.Windows.Point(centre.X - 600, centre.Y - 200));

			CIMLineSymbol lineSymbol = SymbolFactory.Instance.ConstructLineSymbol(ColorFactory.Instance.CreateRGBColor(0, 0, 0), 5, SimpleLineStyle.Solid);

			CreateLine(fieldOfJoy, topLeft, bottomLeft, lineSymbol);
			CreateLine(fieldOfJoy, topRight, bottomRight, lineSymbol);
			CreateLine(fieldOfJoy, rightTop, leftTop, lineSymbol);
			CreateLine(fieldOfJoy, rightBottom, leftButtom, lineSymbol);
		}

		/// <summary>
		/// Create a line graphic. 
		/// </summary>
		/// <param name="fieldOfJoy"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="lineSymbol"></param>
		private static void CreateLine(GraphicsLayer fieldOfJoy, MapPoint a, MapPoint b, CIMLineSymbol lineSymbol)
		{
			fieldOfJoy.AddElement(new CIMLineGraphic()
			{
				Symbol = lineSymbol.MakeSymbolReference(),
				Line = PolylineBuilder.CreatePolyline(new List<MapPoint>() { a, b }, fieldOfJoy.GetSpatialReference())
			});
		}
	}
}