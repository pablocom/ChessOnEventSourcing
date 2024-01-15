CREATE OR REPLACE PROCEDURE save_aggregate(aggregate_id UUID, expected_version INT, events JSON)
LANGUAGE plpgsql
AS $$
DECLARE
    current_version INT;
BEGIN
    -- Start transaction
    BEGIN;

    -- Retrieve the last version of the aggregate
    SELECT Version INTO current_version FROM Aggregates WHERE AggregateId = aggregate_id;

    -- If the aggregate does not exist, insert it
    IF current_version IS NULL THEN
        INSERT INTO Aggregates (AggregateId, AggregateType, Version)
        VALUES (aggregate_id, 'YourAggregateType', 0);
        current_version := 0;
    END IF;

    -- Check for version conflicts (optimistic concurrency control)
    IF expected_version != current_version THEN
        RAISE EXCEPTION 'Concurrency conflict. Expected version: %, but current version is: %', expected_version, current_version;
    END IF;

    -- Loop through each event and insert with incremented version number
    FOR event IN SELECT * FROM json_array_elements(events) LOOP
        current_version := current_version + 1;
        INSERT INTO Events (EventId, AggregateId, AggregateType, EventType, EventData, Version, OccurredOn)
        VALUES (uuid_generate_v4(), aggregate_id, 'YourAggregateType', event->>'EventType', event, current_version, NOW());
    END LOOP;

    -- Update the aggregate with the last version number
    UPDATE Aggregates
    SET Version = current_version
    WHERE AggregateId = aggregate_id AND Version = expected_version;

    -- End transaction
    COMMIT;
EXCEPTION
    WHEN others THEN
        -- In case of any error, rollback the transaction
        ROLLBACK;
        RAISE;
END;
$$;
