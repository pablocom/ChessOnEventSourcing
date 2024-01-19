CREATE OR REPLACE PROCEDURE save_aggregate(aggregate_id UUID, aggregate_type VARCHAR(255), expected_version INT, events JSON)
LANGUAGE plpgsql
AS $$
DECLARE
    current_version INT;
   	event_record JSON;
BEGIN
    SELECT a."Version" INTO current_version FROM "Aggregates" a WHERE "AggregateId" = aggregate_id;
   
    IF current_version IS NULL THEN
        INSERT INTO "Aggregates" ("AggregateId", "AggregateType", "Version")
        VALUES (aggregate_id, aggregate_type, 0);
        current_version := 0;
    END IF;
   
    IF expected_version != current_version THEN
        RAISE EXCEPTION 'Concurrency conflict. Expected version: %, but current version is: %', expected_version, current_version;
    END IF;

    FOR event_record IN SELECT * FROM json_array_elements(events) LOOP
        current_version := current_version + 1;
        INSERT INTO "Events" ("EventId", "AggregateId", "AggregateType", "EventType", "EventData", "Version", "OccurredOn")
        VALUES (
       		uuid(event_record->>'EventId'), 
       		uuid(event_record->>'AggregateId'), 
       		event_record->>'AggregateType', 
       		event_record->>'EventType', 
       		(event_record->>'EventData')::jsonb, 
       		current_version, 
       		(event_record->>'OccurredOn')::timestamptz
       	);
    END LOOP;

    UPDATE "Aggregates"
    SET "Version" = current_version
    WHERE "AggregateId" = aggregate_id AND "Version" = expected_version;
END;
$$;
