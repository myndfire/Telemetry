Run from pwershell:

// JAEGER
docker run -d --name jaeger `
-e COLLECTOR_ZIPKIN_HOST_PORT=:9411 `
-p 5775:5775/udp `
-p 6831:6831/udp `
-p 6832:6832/udp `
-p 5778:5778 `
-p 16686:16686 `
-p 14250:14250 `
-p 14268:14268 `
-p 14269:14269 `
-p 9411:9411 `
jaegertracing/all-in-one:1.31

//ZIPKIN
docker run --name zipkin -d -p 9411:9411 openzipkin/zipkin

//MONGO
docker run --name mongodb -d mongo:4.4

//POSTGRESQL
docker run --name some-postgres -e POSTGRES_PASSWORD=mysecretpassword -d postgres

Activity == Span
Events == Used to point out things that happen at any point in time (markers)
Tags == Attributes (do not travel across process boundaries)
Baggages == Attributes (do travel across process boundaries)
