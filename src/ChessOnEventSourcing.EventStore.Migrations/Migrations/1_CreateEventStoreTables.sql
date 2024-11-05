create table "Aggregates" (
	"AggregateId" uuid primary key,
	"AggregateType" varchar(255) not null,
	"Version" int not null
);

create table "Events" (
	"EventId" uuid primary key,
	"AggregateId" uuid not null references "Aggregates"("AggregateId"),
	"AggregateType" varchar(255) not null,
	"EventType" varchar(255) not null,
	"EventData" jsonb not null,
	"Version" int not null,
	"OccurredOn" timestamp with time zone not null
);

create table "Snapshots" (
 	AggregateId uuid not null references "Aggregates"("AggregateId"),
	SerializedData jsonb not null,
	Version int not null
);

create index idx_events_AggregateId_Version on "Events" ("AggregateId", "Version");
create index idx_aggregates_AggregateId_Version on "Aggregates" ("AggregateId", "Version");
create index idx_events_OccurredOn on "Events" ("OccurredOn");
