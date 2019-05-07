# ComputerShareConsoleApp

This console app will take either a single postcode or a filepath that contains abatch of postcodes.  The App will attempt to work out the most expensive postcode area nearest to the original postcode supplied.  An Html file is generated for you to view the information.

## Command Line Args:
-p "BS22 9BY"                 // For a single postcode, Wrap the postcode in quotes if using a space.

-f C:\Docs\testPostcodes.txt  // when a file path needs to be supplied.  The app only allows 5 postcodes in this file (ratelimiting on API)

-help                         // to show this list of arguments.

If a postcode file is to be used, the file should contain a postcode per line without any extra commas or quotation marks required.
eg.

BS21 9BY

OX11 7XU

etc.

## Data Sources
The Application uses a number of third party APIs to source its data.  All are used on a trial/free basis are subjected to rate limiting etc.  I have added some integration tests to help aid quick debugging should the worst happen.  Zoopla are known for suddenly retracting an API key.  If this is the case then the program should output this exception to the console with a zoopla url and a 403 status.  I can provide a new api key if this happens.  

Geolocation is supplied by Postcode.Io

Mapping images are supplied by MapQuest

Houseprice data is supplied by Zoopla

## Third Party Libraries
I have used some third party libraries in here to help speed development along.
- CommandLineParser to give us some extra functionality around the CommandLineArgs parsing
- Microsoft Extentions (Dependency Injection, Configuriation) to give me some extra functionality that do not come with the console app out of the box.
