/** @jsx jsx */
import { React, AllWidgetProps, jsx, appActions } from "jimu-core";
import { IMConfig } from "../config";
import {
  JimuMapView,
  JimuMapViewComponent,
  JimuFeatureLayerView,
} from "jimu-arcgis";
import { Dropdown, DropdownButton, DropdownMenu, DropdownItem } from "jimu-ui";
import {
  SettingRow,
  SettingSection,
} from "jimu-ui/advanced/setting-components";

const { useState, useEffect } = React;

export default function Widget(props: AllWidgetProps<IMConfig>) {
  const [selectedCity, setSelectedCity] = useState(null);
  const [cityData, setCityData] = useState([]);
  const [highlightedCity, setHighlightedCity] = useState(null);
  const [jimuMV, setjimuMV] = useState(null);

  useEffect(() => {
    zoomToCity(selectedCity), sendMessage(selectedCity);
  }, [cityData, selectedCity]);

  // START Demo 1: Dropdown met alle Steden
  const FilterCity = () => {
    return (
      <SettingSection title={"Stad:"}>
        <SettingRow>
          <div className="d-flex justify-content-between w-100 align-items-center">
            <Dropdown
              style={{
                width: "100%",
              }}
            >
              <DropdownButton>
                {selectedCity === null
                  ? "Selecteer stad"
                  : selectedCity.attributes[props.config.cityField]}
              </DropdownButton>

              <DropdownMenu maxHeight={500}>
                {cityData.map((item) => {
                  return (
                    <DropdownItem onClick={() => onClickFilterCity(item)}>{`${
                      item.attributes[props.config.cityField]
                    }`}</DropdownItem>
                  );
                })}
                {/* EIND Demo 3: vul de dropdown */}
              </DropdownMenu>
            </Dropdown>
          </div>
        </SettingRow>
      </SettingSection>
    );
  };

  //
  //
  //
  //
  //

  //Zodra de gebruiker een stad geselecteerd heeft in de dropdown
  const onClickFilterCity = (item) => {
    setSelectedCity(item);
  };

  //Zoom de kaart naar het gekozen stad
  //Obv useEffect, gaat de ZoomToCity af zodra de cityData gezet wordt
  const zoomToCity = (cityFeature) => {
    if (cityFeature == null) return;

    let selectedCity = cityData.filter((city) => {
      return (
        city.attributes[props.config.cityField] ==
        cityFeature.attributes[props.config.cityField]
      );
    });
    //popup vullen met geselcteerde stad
    jimuMV.view.popup.features = selectedCity;
    //jimuMV.view.popup.open(); //werkt niet lekker, moet template in

    //highlight stad op de kaart
    const layerView: __esri.FeatureLayerView = jimuMV.view.layerViews
      .toArray()
      .find(
        (v) => v.layer.id === selectedCity[0].layer.id
      ) as __esri.FeatureLayerView;

    //vorige highlight verwijderen
    if (highlightedCity) highlightedCity.remove();
    setHighlightedCity(layerView?.highlight(selectedCity[0]));

    //Zoom naar featureextent
    jimuMV.view.goTo(selectedCity[0].geometry);
  };

  //
  //
  //
  //
  //

  //Obv useEffect, gaat de sendMessage af zodra de cityData gezet wordt
  //Stuur een bericht naar geabonneerde widgets
  const sendMessage = (cityFeature) => {
    console.log("sendMessage");
    if (cityFeature == null) return;

    var messageToSend = {};
    messageToSend["city"] = JSON.stringify(cityFeature);

    props.dispatch(
      appActions.widgetStatePropChange("DDselectWidget", "Data", messageToSend)
    );
  };

  //
  //
  //
  //
  //

  // Reageer op veranderingen in de MapView (koppeling met de kaart)
  const activeViewChangeHandler = (jmv: JimuMapView) => {
    if (jmv) {
      setjimuMV(jmv);
      console.log("Props.Config: ", props.config.TabelCity);
      //Ophalen alle Cityen en rijbanen
      //deze aanroep geeft een setState (setCityData) als callbackfunctie mee
      //als de features zijn opgehaald, wordt de setState aangeroepen met de features
      getData(
        jmv,
        props.config.layerName,
        "1=1",
        true,
        [
          "Plaats",
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
        // ["*"],
        setCityData
      );

      //watch op de popup. Als een gebruiker een feature selecteert of next/prev in de popup doet, gaat deze af
      jmv.view.popup.watch("selectedFeature", function (feature) {
        if (feature?.layer.title.includes(props.config.layerName)) {
          setSelectedCity(feature);
        }
      });
    }
  };

  //
  //
  //
  //
  //

  //Ophalen data uit featurelayer met aanroep naar callback-functie
  const getData = (
    jmv: JimuMapView,
    layername: string,
    where: string,
    returnGeometry: boolean,
    outfields: string[],
    callbackfunc
  ) => {
    var jimumapview = jmv != null ? jmv : jimuMV;
    //loop over alle featurelayers
    const flayer: __esri.FeatureLayer = jimumapview.view.map.layers
      .toArray()
      .find((v) => v.title === layername);
    var qry = flayer.createQuery();
    qry.where = where;
    qry.outFields = outfields;
    qry.returnGeometry = returnGeometry;
    // if (outfields[0] != "*") qry.orderByFields = [outfields[0]];

    flayer.queryFeatures(qry).then((results) => {
      if (callbackfunc) {
        callbackfunc(results.features); //roep de setstate-functie aan
      }
    });
  };

  return (
    <div>
      <FilterCity />
      {/*onderstaand vanaf Demo 3*/}
      <JimuMapViewComponent
        useMapWidgetIds={props.useMapWidgetIds}
        onActiveViewChange={activeViewChangeHandler}
      />
    </div>
  );
}
