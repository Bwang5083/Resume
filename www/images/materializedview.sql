//������־
create materialized view log on POI with rowid;
create materialized view log on POI_EMAIL with rowid;
create materialized view log on POI_URL with rowid;

//�����̻���ͼ
create materialized view SolidSearch tablespace wangbin
refresh fast on commit
as select p.rowid p_rowid,e.rowid e_rowid,u.rowid u_rowid,p.*,e.EMAIL,u.URL
from POI p,POI_EMAIL e,poi_url u
where p.global_id=e.global_id and p.global_id=u.global_id