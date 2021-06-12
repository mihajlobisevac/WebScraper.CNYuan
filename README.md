## About

This is a console app that extracts data from [Bank of China webpage](https://srh.bankofchina.com/search/whpj/searchen.jsp) and outputs it to a text file.

It uses mainly [Html Agility Pack](https://html-agility-pack.net/) nuget package for data scraping.


## How it works

* Initial request gets information about available currencies
* For every currency, a new POST request is created
* Each POST request carries information about currency name, page number, and start & end date (last 2 days)
* Data is extracted from a html document using Html Agility Pack
* Data is saved in a text file, in a form of csv
* File path is retrieved from a config file (appsettings.json) using Dependency Injection
