create or replace procedure create_table
authid current_user
as
INTA NUMBER;
RESULT_SET SYS_REFCURSOR;
poi_max int;
poi_min int;
link_max int;
link_min int;
node_max int;
node_min int;
begin
    GetMaxMinFromPoi(poi_max,poi_min);
    GetMaxMinFromLink(link_max,link_min);
    GetMaxMinFromNode(node_max,node_min);
    select count(1) into INTA from TAB where TNAME= 'GLOBAL_MAXID_COUNTER';
    if INTA = 0
    then
        execute immediate 'create table GLOBAL_MAXID_COUNTER(OBJECTID int NOT NULL PRIMARY KEY,FeatureCode VARCHAR(64),MaxID int)';
        commit;
        execute immediate 'insert into GLOBAL_MAXID_COUNTER values(:1,:2,:3)' using 1,'poi',poi_max;
        execute immediate 'insert into GLOBAL_MAXID_COUNTER values(:1,:2,:3)' using 2,'link',link_max;
        execute immediate 'insert into GLOBAL_MAXID_COUNTER values(:1,:2,:3)' using 3,'node',node_max;
    else
        execute immediate 'update GLOBAL_MAXID_COUNTER set MaxID=:1 where objectid=1' using poi_max;
        execute immediate 'update GLOBAL_MAXID_COUNTER set MaxID=:1 where objectid=2' using link_max;
        execute immediate 'update GLOBAL_MAXID_COUNTER set MaxID=:1 where objectid=3' using node_max;
    end if;
    select count(1) into INTA from TAB where TNAME= 'GLOBAL_MINID_COUNTER';
    if INTA = 0
    then
        execute immediate 'create table GLOBAL_MINID_COUNTER(OBJECTID int NOT NULL PRIMARY KEY,FeatureCode VARCHAR(64),MinID int)';
        commit;
        execute immediate 'insert into GLOBAL_MINID_COUNTER values(:1,:2,:3)' using 1,'poi',poi_min;
        execute immediate 'insert into GLOBAL_MINID_COUNTER values(:1,:2,:3)' using 2,'link',link_min;
        execute immediate 'insert into GLOBAL_MINID_COUNTER values(:1,:2,:3)' using 3,'node',node_min;
    else
        execute immediate 'update GLOBAL_MINID_COUNTER set MinID=:1 where objectid=1' using poi_min;
        execute immediate 'update GLOBAL_MINID_COUNTER set MinID=:1 where objectid=2' using link_min;
        execute immediate 'update GLOBAL_MINID_COUNTER set MinID=:1 where objectid=3' using node_min;
    end if;
end create_table;

create or replace procedure GetMaxMinFromPoi(max_out out int,min_out out int)
as
begin
select max(GLOBAL_ID) into max_out from poi;
select min(GLOBAL_ID) into min_out from poi;
end;

create or replace procedure GetMaxMinFromLink(max_out out int,min_out out int)
as
begin
select max(Link_ID) into max_out from link;
select min(Link_ID) into min_out from link;
end;

create or replace procedure GetMaxMinFromNode(max_out out int,min_out out int)
as
begin
select max(Node_ID) into max_out from node;
select min(Node_ID) into min_out from node;
end;
