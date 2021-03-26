# Pokemon API

David Milne 25/03/2021

## Project Structure

 - API project contains controllers and general startup configuration
 - Common project contains domain, some enums and constants.
 - Infrastructure project contains monitoring classes, repositories and adaptors
 - Service project contains business logic

## How to use
Download project and run locally. No other configuration should be required as long as visual studio is installed on the machine.

**Simple Retrieve Pokemon Request**
https://localhost:44301/api/Pokemon/{name}
GET

    {
    		"name": "Pikachu",
    		"description": "When several of these POKéMON gather, their 	electricity could build and cause lightning storms.",
    		"habitat": "forest",
    		"isLegendary": false
    }

**Translated Pokemon Request
https://localhost:44301/api/Pokemon/Translated/{name}
GET

    {
    		"name": "Pikachu",
    		"description": "When several of these POKéMON gather, their 	electricity could buildeth and causeth lightning storms.",
    		"habitat": "forest",
    		"isLegendary": false
    }

## Logging
I have added some simple metrics and error logging using Prometheus. Obviously this is not wired up to go anywhere but can be viewed using the standard https://localhost:44301/metrics. 

## Things to think about for a production release
**Line break removals**
Line breaks break the translator and make the response look ugly. Given the size of the available data it's not clear to me whether the fairly basic solution I implemented to remove these is adequate. This feels like an area of concern and there is almost certainly a nicer way to do it.

**Translation Route Handling**
I'm not sure about the way the controller determines whether to translate the route or not is particularly good. This is something I haven't looked at before so I'm not sure if there is a better way to do it. However it felt better to do this then have two separate methods which did nearly identical things. 

**Logging**
This could be built upon. I would like to add a timer for the entire request for the external requests so you could build up a much better picture of where time is spent.
In addition it would be fairly simple to add flavour to the logging for more detailed error information.
It would be nice to to build a Grafana dashboard based on these metrics to view, i.e. request count, time, amount of failures etc.

**Translation Information**
A simple statement to determine whether to translate with Yoda or Shakespeare feels like a candidate to break the open/closed rule for further iterations. I couldn't think of a nice way to deal with this in a short timeframe but it would be nice to explore the possibility of making this extensible without amending the PokemonService

**Tight coupling between 3rd party APIs and response classes**
Given the Pokemon and Translation APIs are open APIs and not something entered into with a commercial contract it makes me feel uneasy building concrete classes to deserialize into. It wouldn't take much of a change to break this.  I don't have any good ideas to guard against this but I'd like to explore making this more fault tolerant. As it stands we'd need to pick up the exceptions and react. 

**Add Value**
Would be good to add more of the flavour text entries, I'm not sure what the relevance of the different colours are (my kids didn't know either and they play it). We could look at collating more data and adding to the description or adding more fields. Any more text based description should be translated if possible. Would be nice to contact the funtranslation api owners and find a way (paid presumably) to increase the request limit.
> Written with [StackEdit](https://stackedit.io/).
