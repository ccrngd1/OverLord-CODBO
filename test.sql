show databases;

use c951004_phpFusion;
show tables;

select * from Players;

select commandName from commands where statusPermission = (select status from Players where GUID = '9743419' and banned=0);
select status from Players where GUID = '9743419' and banned=0;