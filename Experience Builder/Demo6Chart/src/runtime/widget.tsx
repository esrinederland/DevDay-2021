/** @jsx jsx */
import { React, AllWidgetProps, jsx, getAppStore } from "jimu-core";
import * as Highcharts from "highcharts";
import HighchartsReact from "highcharts-react-official";

const { useState, useEffect } = React;

export default function Widget(props: AllWidgetProps<{}>) {
  let chartOptionsData = {};

  const [selectedCityFeature, setSelectedCityFeature] = useState({});
  const [chartOptions, setChartOptions] = useState({
    chart: {
      type: "column",
    },
    title: {
      text: "Afstanden 11 Steden",
    },
    xAxis: {
      categories: [
        "Leeuwarden",
        "Sneek",
        "Ijlst",
        "Sloten",
        "Stavoren",
        "Hindeloopen",
        "Workum",
        "Bolsward",
        "Harlingen",
        "Franeker",
        "Dokkum",
      ],
    },
    yAxis: {
      title: {
        text: "Afstand in kilometers",
      },
    },
    series: [
      {
        name: "11 Steden",
        data: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
      },
    ],
  });

  //
  //
  //
  //
  //

  // initialize widget communication in the receiving end
  // this fires only once because it has no properties to act upon
  useEffect(() => {
    getAppStore().subscribe(() => {
      if (
        getAppStore().getState().widgetsState["DDselectWidget"] != undefined
      ) {
        var messageAsJSON = getAppStore().getState().widgetsState[
          "DDselectWidget"
        ].Data;
        let cityFeature = JSON.parse(messageAsJSON["city"]);
        setSelectedCityFeature(cityFeature);
      }
    });
  }, []);

  // this is needed to update the chart every time a new city name is received through communication
  useEffect(() => {
    UpdateSeries();
  }, [selectedCityFeature]);

  // update the contents of the chart
  const UpdateSeries = () => {
    if (
      selectedCityFeature &&
      Object.keys(selectedCityFeature).length > 0 &&
      selectedCityFeature.constructor === Object
    ) {
      let newChartSeries = [];
      for (const [key, value] of Object.entries(
        selectedCityFeature.attributes
      )) {
        if (key != "Plaats") {
          newChartSeries.push(value);
        }
      }
      chartOptionsData["title"] = {
        text: `Afstanden vanaf ${selectedCityFeature.attributes["Plaats"]}`,
      };
      chartOptionsData["series"] = [];
      chartOptionsData["series"] = [
        { name: "11 Steden", data: newChartSeries },
      ];

      // set state with adjusted chartOptionsData
      setChartOptions(chartOptionsData);
    }
  };

  //
  //
  //
  //
  //

  return (
    <div className="widget-demo jimu-widget m-2">
      <HighchartsReact highcharts={Highcharts} options={chartOptions} />
    </div>
  );
}
