
-- update process
delete FreqDiff
insert into FreqDiff
select 
    ud1.itemid, ud2.itemid, count(*), (sum(ud1.rating - ud2.rating))/count(*)
from 
    vw_userdata ud1 
    join vw_userdata ud2 on 
            ud1.userid = ud2.userid
        and ud1.itemid > ud2.itemid
group by ud1.itemid, ud2.itemid

-- predict process
declare @pref table(itemid varchar(50), rating float)
insert into @pref values(1, 0.4)
insert into @pref values(3, 0.3)

select -- distinct top 10
    itemid1,
    sum(freq)                               as freq,
    sum(freq*(diff + rating))            as pref,
    sum(freq*(diff + rating)) /sum(freq) as rating
from 
    vw_freqdiff fd
    join @pref p on fd.itemid2 = p.itemid
where itemid1 not in( select itemid from @pref )
group by itemid1