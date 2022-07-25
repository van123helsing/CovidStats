# CovidStats

A simple .NET web api that exposes Slovenian Covid statistics as two RESTful endpoints with a simple authentication. 

## Table of Contents

- [Prerequisites](#prerequisites)
- [User authentication](#user-authentication)
- [Region Cases](#region-cases)
  *  [/api/cases](#cases)
  *  [/api/lastweek](#last-week)
  
## Prerequisites

In order to build the solution you need to have installed .NET 6.0 Core.

After you build and run the solution, you first need to authenticate yourself before you can use Covid statistics [Region cases](#region-cases). How you can authenticate is described in chapter [User authentication](#user-authentication).

## User authentication
User authentication endpoint provides a simple authentication with JWT token. You can achieve this in 3 steps:
1. Prepare a POST request to url `http://localhost:7254/users/authenticate`
2. In the body field append a simple json (only this credentials are allowed):
```
{
    "username": "test",
    "password": "test"
}
```
3. Send the request, and you should recieve a token in the response.

With the recieved token you are now allowed to execute requests to the other endpoints. Just pass the token as the `Bearer token` in the Authorization section and your access won't be denied.

## Region Cases

Region cases implement 2 endpoints: `/api/cases` and `/api/lastweek`. Both require user to pass a JWT token.

### `/api/cases`

Allows filtering of daily cases based on 3 parameters:
1. `fromDate`: oldest date which should still be included in results (ignored if not passed)
1. `toDate`: the most recent date which should still be included in results (ignored if not passed)
1. `region`: retrieve results only for specific region (if not passed all regions are included in result)

Example request (don't dorget to include JWT token):
```
https://localhost:7254/api/region/cases?fromDate=2022-01-01&toDate=2022-01-01&region=lj
```

Example response:
```
[
    {
        "date": "2022-01-01T00:00:00",
        "region": "LJ",
        "activeCases": 4990,
        "vaccinated1st": 342189,
        "vaccinated2nd": 324379,
        "deceased": 1235
    }
]
```

### `/api/lastweek`

Provides average number of new daily cases in the last 7 days per each region. List is sorted in a decsending order. No parameters are supported.

Example request (don't dorget to include JWT token):
```
https://localhost:7254/api/region/lastweek
```

Example response:
```
[
    {
        "region": "LJ",
        "dailyAverage": 282
    },
    {
        "region": "MB",
        "dailyAverage": 163
    },
    {
        "region": "CE",
        "dailyAverage": 141
    },
    {
        "region": "KR",
        "dailyAverage": 102
    },
    {
        "region": "KP",
        "dailyAverage": 91
    },
    {
        "region": "NM",
        "dailyAverage": 90
    },
    {
        "region": "NG",
        "dailyAverage": 78
    },
    {
        "region": "MS",
        "dailyAverage": 62
    },
    {
        "region": "KK",
        "dailyAverage": 49
    },
    {
        "region": "ZA",
        "dailyAverage": 35
    },
    {
        "region": "SG",
        "dailyAverage": 25
    },
    {
        "region": "PO",
        "dailyAverage": 19
    }
]
```
