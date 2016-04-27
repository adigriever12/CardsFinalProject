
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


/*
CREATE PROCEDURE [dbo].[Procedure]
	@userid varchar(max)
AS

WITH freqdiff(itemid1, itemid2, freq, diff) AS
(
	select 
		ud1.itemid, ud2.itemid, count(*), (sum(ud1.rating - ud2.rating))/count(*)
	from 
		vw_userdata ud1 
		join vw_userdata ud2 on 
				ud1.userid = ud2.userid
			and ud1.itemid > ud2.itemid
	group by ud1.itemid, ud2.itemid
), 
viewdiff AS 
(
select itemid1 as itemid1, itemid2 as itemid2, freq,     diff from freqdiff fd
union all
select itemid2 as itemid1, itemid1 as itemid2, freq, -1* diff from freqdiff fd
),
curruser AS
(
select itemid, rating
from vw_userdata
where userid = @userid
),
outcome AS
(
	select
		itemid1,
		sum(freq)                            as freq,
		sum(freq*(diff + rating))            as pref,
		sum(freq*(diff + rating)) /sum(freq) as rating
	from 
		viewdiff fd
		join curruser p on fd.itemid2 = p.itemid
	where itemid1 not in( select itemid from curruser )
	group by itemid1
)
select top 10 * from outcome order by rating desc

RETURN*/

/*

/*
 * Wrap for userdata, 
 * switch from one model to another easily.
 */
CREATE view [dbo].[vw_userdata] as 
select UserId as userid, RestuarantId as itemid , rating as rating from UserRankings*/