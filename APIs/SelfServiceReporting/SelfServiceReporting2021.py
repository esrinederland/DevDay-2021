import logging
import datetime
import requests
import json
import base64
import random
_logFilePath = r"D:\temp\logging\SSR2021_{}.log".format(datetime.datetime.now().strftime('%Y%m%d_%H%M%S'))
#_logFilePath = None
_emailUrl = "<< e-mail url >>"
_regionLayerUrl = "<< Service URL >>"
_routingAPIKey = "<< API Key >>"
officeGeometry = {"x":6.09203,"y":52.502783}
_imageUrls = {"Dog":"https://dog.ceo/api/breeds/image/random","Cat":"https://cataas.com/cat"}

def Parsebody(inputbody):
    logging.info("SSR2021::Parsebody::Start")
    logging.info(f"body: {inputbody}")

    email = inputbody["feature"]["attributes"]["email"]

    reportItems = {}
    reportItems["comment"] = inputbody["feature"]["attributes"]["comment"]
    reportItems["name"] = inputbody["feature"]["attributes"]["name"]
    #print route image
    fromPoint = inputbody["feature"]["geometry"]
    routeGeometry = GenerateRouteGeometry(fromPoint,officeGeometry)

    mapExtent = BufferGeometry(routeGeometry,1000)

    routeImageUrl = PrintImage(routeGeometry,fromPoint,officeGeometry,mapExtent)

    routeUrl = "https://via.placeholder.com/500x400.png?text=hier+komt+de+route"
    routeBase64String = base64.b64encode(requests.get(routeImageUrl).content).decode('utf-8')
    reportItems["base64route"] = routeBase64String


    #get image
    imageurl = "https://cataas.com/cat"
    reportItems["animal"] = "Kat"
    if inputbody["feature"]["attributes"]["choice"] =="Dog":
        reportItems["animal"] = "Hond"
        dogResponse = requests.get("https://dog.ceo/api/breeds/image/random")
        imageurl = dogResponse.json()["message"]
        
    imageBase64String = base64.b64encode(requests.get(imageurl).content).decode('utf-8')
    reportItems["base64image"] = imageBase64String

    
    #get travel regions
    queryParams = {}
    queryParams["f"]="json"
    queryParams["where"]="1=1"
    queryParams["geometry"]=json.dumps(routeGeometry)
    queryParams["geometryType"]="esriGeometryPolyline"
    queryParams["inSR"]=4326
    queryParams["spatialRel"]="esriSpatialRelIntersects"
    queryParams["outFields"]="*"

    queryresults = requests.post(f"{_regionLayerUrl}/query",queryParams)
    regions_obj = queryresults.json()
    logging.info("Found {} features".format(len(regions_obj["features"])))
    logging.debug("features {}".format(regions_obj["features"]))

    tableRows = ""
    for feature in regions_obj["features"]:
        rowTemplate = tableRowstring
        songnr = random.randint(1,3)
        atts = feature["attributes"]
        song = json.loads(atts[f"Song{songnr}"])
        rowTemplate = rowTemplate.replace("@place@",atts["Gemeentenaam"])
        rowTemplate = rowTemplate.replace("@imageurl@",song["albumCover"])
        rowTemplate = rowTemplate.replace("@songurl@",song["url"],2)
        rowTemplate = rowTemplate.replace("@artist@",song["artists"])
        rowTemplate = rowTemplate.replace("@title@",song["name"])

        tableRows +=rowTemplate

    reportItems["tablerows"] = tableRows

    #create html for report
    
    mailBody = htmlString
    for key in reportItems:
        logging.debug(f"Parsing key: {key}")
        mailBody = mailBody.replace(f"@{key}@",reportItems[key])


    #send email
    r = requests.post(_emailUrl,json = {"to":email,"subject":"Hier is je rapport","body":mailBody});
    logging.info("Script complete")


def GenerateRouteGeometry(fromGeom,toGeom):
    #generate route
    logging.info("GenerateRouteGeometry")
    from_x = fromGeom["x"]
    from_y = fromGeom["y"]
    to_x = toGeom["x"]
    to_y = toGeom["y"]
    routeApiUrl = "https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World/solve"
    solveParams = {}
    solveParams["f"] = "json"
    solveParams["token"] = _routingAPIKey
    solveParams["stops"] = f"{from_x},{from_y};{to_x},{to_y}"
    solveParams["startTime"] = "now"
    
    logging.info("Calculating route")
    results = requests.post(routeApiUrl,solveParams)
    route_object = results.json()
    #logging.info("Route results: {}".format(route_object))

    return route_object["routes"]["features"][0]["geometry"]

def BufferGeometry(routeGeometry,nrofMeters):
    logging.info("BufferGeometry")
    geomService = "https://utility.arcgisonline.com/arcgis/rest/services/Geometry/GeometryServer/buffer"

    bufferParams = {"geometries":json.dumps({"geometryType":"esriGeometryPolyline","geometries":[routeGeometry]}),
                    "inSR":4326,   #WGS84 Lat long
                    "outSR":102100,   #Web merkator
                    "distances":nrofMeters,  #20000 units
                    "unit":9001,      #unitid: meters
                    "f":"json"}

    r = requests.post(geomService,bufferParams)

    #print(r.text)

    #get the outer ring (there should only be one because we only buffer one point)
    bufferResult = r.json()
    outerRing = bufferResult["geometries"][0]["rings"][0]
    #print(outerRing)

    #get the min max xy values 
    minx = min([vertice[0] for vertice in outerRing])
    maxx = max([vertice[0] for vertice in outerRing])
    miny = min([vertice[1] for vertice in outerRing])
    maxy = max([vertice[1] for vertice in outerRing])

    extent = {
         "xmin":minx,
         "ymin":miny,
         "xmax":maxx,
         "ymax":maxy,
         "spatialReference":{
            "wkid":102100
         }
      }
    logging.info(f"Extent determined to be: {extent}")
    return extent

def PrintImage(routeGeometry,fromPoint,toPoint,extent):
    logging.info("Creating print")
    printurl = "https://utility.arcgisonline.com/arcgis/rest/services/Utilities/PrintingTools/GPServer/Export%20Web%20Map%20Task/execute"

    routeGeometry["spatialReference"] = {"wkid":4326}

    print_info = {
    "mapOptions":{
        "extent":extent
    },
    "operationalLayers":[
        {
            "url":"https://services.arcgisonline.com/arcgis/rest/services/Canvas/World_Dark_Gray_Base/MapServer",
            "id":"topo-base-layer",
            "title":"World Topo Map",
            "opacity":1,
            "minScale":0,
            "maxScale":0
        },
        {
            "featureCollection":{
                "layers":[
                {
                    "layerDefinition":{
                        "name":"pointLayer",
                        "geometryType":"esriGeometryPoint"
                    },
                    "featureSet":{
                        "geometryType":"esriGeometryPoint",
                        "features":[
                            {
                            "geometry":{
                                "spatialReference":{
                                    "wkid":4326
                                },
                                "x":fromPoint["x"],
                                "y":fromPoint["y"]
                            },
                            "symbol":{
  "type": "esriSMS",
  "color": [255, 255, 0, 255],
  "angle": 0,
  "xoffset": 0,
  "yoffset": 0,
  "size": 18,
  "style": "esriSMSCircle",
  "outline": {
    "type": "esriSLS",
    "color": [177, 177, 177, 255],
    "width": 2,
    "style": "esriSLSSolid"
  }
}
                            },
                            {
                            "geometry":{
                                "spatialReference":{
                                    "wkid":4326
                                },
                                "x":toPoint["x"],
                                "y":toPoint["y"]
                            },
                            "symbol":{
  "type": "esriSMS",
  "color": [255, 255, 0, 255],
  "angle": 0,
  "xoffset": 0,
  "yoffset": 0,
  "size": 18,
  "style": "esriSMSCircle",
  "outline": {
    "type": "esriSLS",
    "color": [177, 177, 177, 255],
    "width": 2,
    "style": "esriSLSSolid"
  }
}
                            }
                        ]
                    }
                },
                {
                    "layerDefinition":{
                        "name":"lineLayer",
                        "geometryType":"esriGeometryPolyline"
                    },
                    "featureSet":{
                        "geometryType":"esriGeometryPolyline",
                        "features":[
                            {
                             
                            "geometry":routeGeometry,
                            "symbol":{
  "type": "esriSLS",
  "color": [255, 255, 0, 255],
  "width": 3,
  "style": "esriSLSSolid"
}
                            }
                        ]
                    }
                }
                ]
            }
        }
    ],
    "baseMap":{
        "title":"Topographic Basemap",
        "baseMapLayers":[
            {
                "url":"https://services.arcgisonline.com/arcgis/rest/services/Canvas/World_Dark_Gray_Base/MapServer"
            }
        ]
    },
    "exportOptions":{
        "outputSize":[
            600,
            500
        ]
    }
    }

    #print the map
    printParams = {"Web_Map_as_JSON":json.dumps(print_info),"f":"json","format":"png32"}
    #logging.info(printParams)
    logging.info("requesting print")
    r = requests.post(printurl,printParams)

    logging.info("print results: {}".format(r.text))

    result = r.json()

    if "error" in result:
        imgurl = "https://via.placeholder.com/600x500.png?text=Error+creating+route"
    else:
        #get the img url to download it
        imgurl = result["results"][0]["value"]["url"]
    logging.info(f"Result pint: {imgurl}")
    return imgurl

tableRowstring = """<tr class="row">
    <td>@place@</td>
    <td><a href="@songurl@"><img src="@imageurl@" height="60" width="60"></a></td>
    <td><b>@artist@</b><br/> 
        <a href="@songurl@">@title@</a>
    </td>
</tr>"""

htmlString = """

<style>
    HTML,BODY
    {
font-family: sans-serif;
    }
    *{
        font-family: sans-serif;
    }
    .panel
    {
        background-color: #f8f8f8;
    border: 1px solid #efefef;
    padding: 0.99rem;
    margin-bottom:5px;
    font-family: sans-serif;
    }
    td
    {
        vertical-align: top;;
    }
    td.A
    {
        color:#000000;
    }
</style>

<div style="background-color: #004575;color:#FFFFFF;padding:10px 0px 0px 15px;height:60px;margin-bottom: 5px;">
<h2 style="font-size: 1.9994rem;line-height: 1.35;font-weight: 400;margin: 0 0 0.75rem 0">Hallo @name@</h2>
</div>
<div style="margin-bottom:5px" class="panel"> Dit is wat je van onze sessie vond:<br/>
<i>@comment@</i>
</div>
<div class="panel">
Dit is de route van het punt naar ons kantoor in Zwolle:
<br>
<img alt="Route" id="routeimage" src="data:image/png;base64, @base64route@">
</div>
<div class="panel">
    Dit zijn wat liedjes voor tijdens de route:
    <table id="songtable">
        <tr class="header">
            <td>Gemeente</td>
            <td colspan="2">Liedje</td>
        </tr>
        @tablerows@
    </table>

</div>
<div class="panel">
    Een plaatje van een @animal@:<br/>
<img width="600" id="animalimage" alt="CatorDog" src="data:image/png;base64, @base64image@">
</div>


"""
if __name__=="__main__":
    main()
