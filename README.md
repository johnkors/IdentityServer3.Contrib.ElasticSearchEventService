master build: 
[![Build status](https://ci.appveyor.com/api/projects/status/5o9shqnmao5ln18w/branch/master?svg=true)](https://ci.appveyor.com/project/JohnKorsnes/identityserverv3-contrib/branch/master)

dev build: 
[![Build status](https://ci.appveyor.com/api/projects/status/5o9shqnmao5ln18w/branch/dev?svg=true)](https://ci.appveyor.com/project/JohnKorsnes/identityserverv3-contrib/branch/dev)

[![NuGet Stable](http://img.shields.io/nuget/v/IdentityServer3.ElasticSearchEventService.svg?style=flat)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)

[![NuGet Prerelease](https://img.shields.io/nuget/vpre/IdentityServer3.ElasticSearchEventService.svg)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)

[![Downloads](https://img.shields.io/nuget/dt/IdentityServer3.ElasticSearchEventService.svg)](https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService/)

# Contents

Implementation of IdentityServerV3s IEventService using Serilogs ElasticSearchSink to push IdentityServerv3 events on a format useful for Kibana.


## Usage

```
   var elasticUri = new Uri("http://your.elasticsearch.instance/");
   var options = new ElasticsearchSinkOptions(elasticUri);
   var eventService = new ElasticSearchEventService(options);
```

## Install

```
  PM> Install-Package IdentityServer3.ElasticSearchEventService
```

NuGet:
https://www.nuget.org/packages/IdentityServer3.ElasticSearchEventService


## Dependencies

 * Thinktecture.IdentityServer3 - http://www.nuget.org/packages/Thinktecture.IdentityServer3/
 * Serilog.Sinks.ElasticSearch - http://www.nuget.org/packages/Serilog.Sinks.ElasticSearch/
