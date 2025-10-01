from fastapi import FastAPI
from fastapi.responses import JSONResponse
import logging

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(
    openapi_url="/openapi.json",
    servers=[{"url": "https://ca3d-76-68-29-237.ngrok-free.app"}]
)

@app.get("/")
def read_root():
    return {"DotNetLabs": "Parks information Server"}

@app.get('/getparkinformation')
async def get_park_information(park_name: str):
    """This function is used to get information for a specific park based on the park name."""
    logger.info("Received request to get information for park: %s", park_name)
    park_information = f"Information for {park_name}!"
    return JSONResponse(content={'park_information': park_information}, status_code=200)

@app.get('/getweatheratlocation')
async def get_weather_at_location(city_name: str):
    """This function is used to get the weather at a location."""
    weather = {
        "Seattle": "21c",
        "Toronto": "15c"
    }
    city_weather_info = ""
    if city_name in weather:
        city_weather_info  = weather[city_name]
    else:
        raise NotImplementedError()
    return JSONResponse(content={'city_weather_information': city_weather_info }, status_code=200)

@app.get('/getparksatlocation')
async def get_parks_at_location(city_name: str):
    """This function is used to get parks at a location."""
    parks = {
        "Seattle": "Wild Waves Theme Park in Seattle offers roller coaster rides1. Visitors can also enjoy other attractions such as water slides, carousel rides, arcade games, and miniature golf3. Another option is Silverwood, a theme park near Coeur D'Alene with both wood and steel roller coasters5",
        "Toronto": "There are several amusement parks near Toronto with roller coasters and other fun rides. Some options include Canada's Wonderland, Centreville Island Amusement Park, and the Canadian National Exhibition (CNE)"
    }
    city_parks_info = ""
    if city_name in parks:
        city_parks_info  = parks[city_name]
    else:
        raise NotImplementedError()
    return JSONResponse(content={'city_parks_information': city_parks_info }, status_code=200)

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)