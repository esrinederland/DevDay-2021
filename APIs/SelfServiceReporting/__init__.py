import logging
import urllib.parse
import os
import azure.functions as func
from . import SelfServiceReporting2021


__version__ = "v20210315.03"

def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info(f'{__version__} Python HTTP trigger function processed a request. SelfServiceReporting2021')
    try:
        
        req_body = req.get_json()
        logging.info("req_body: {}".format(req_body))

        logging.info("agolusername: {}".format(os.environ["AGOL_USERNAME"]))
        SelfServiceReporting2021.Parsebody(req_body)
        return func.HttpResponse(f"This HTTP triggered function executed successfully. {__version__}")
    except Exception as ex:
        logging.exception("error parsing body")
        return func.HttpResponse(
             f"This HTTP triggered function executed unsuccessfully. {__version__} Error: {ex}",
             status_code=500
        )
   
       