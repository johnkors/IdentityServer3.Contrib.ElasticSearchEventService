# Contents

Implementation of IdentityServerV3s IEventService using Serilogs ElasticSearchSink to push IdentityServerv3 events on a format useful for Kibana.


## Usage

```
   var elasticUri = new Uri("http://your.elasticsearch.instance/");
   var options = new ElasticsearchSinkOptions(elasticUri);
   var eventService = new EventService(options);
```

## Dependencies

 * Thinktecture.IdentityServer3 - http://www.nuget.org/packages/Thinktecture.IdentityServer3/
 * Serilog.Sinks.ElasticSearch - http://www.nuget.org/packages/Serilog.Sinks.ElasticSearch/
