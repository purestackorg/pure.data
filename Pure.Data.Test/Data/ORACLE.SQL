DROP TABLE `TB_USER`;

-- Create table
create table TB_USER
(
  id          NUMBER not null,
  Age       NUMBER,
  Name      VARCHAR2(255),
  Sex       NUMBER(2),
  Email     VARCHAR2(255),
  DTCreate  DATE,
  HasDelete NUMBER(2)
)
tablespace REVIEWDB_TS
  pctfree 10
  initrans 1
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );
-- Create/Recreate primary, unique and foreign key constraints 
alter table TB_USER
  add primary key (ID)
  using index 
  tablespace REVIEWDB_TS
  pctfree 10
  initrans 2
  maxtrans 255
  storage
  (
    initial 64K
    next 1M
    minextents 1
    maxextents unlimited
  );

-- Create sequence 
create sequence S_TB_USER
minvalue 1
maxvalue 99999999
start with 1
increment by 1
cache 20
order;

--触发器
 CREATE OR REPLACE TRIGGER trg_TB_USER_id 
BEFORE INSERT ON TB_USER 
FOR EACH ROW 
BEGIN
SELECT  S_TB_USER.nextval INTO :new.id FROM dual; 
END; 


INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (80,'Drake',1,'non.arcu.Vivamus@tellusloremeu.edu',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (55,'Neil',0,'Quisque.varius.Nam@non.com',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (82,'Duncan',1,'Curabitur.consequat.lectus@nisisemsemper.com',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (21,'Graham',0,'risus.Morbi@sed.org',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (83,'Clayton',0,'ultrices.posuere@tinciduntDonecvitae.org',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (24,'Harrison',1,'amet@egetdictum.edu',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (14,'Thane',1,'odio.Etiam@sagittis.net',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (49,'Donovan',0,'egestas@purusgravidasagittis.co.uk',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (23,'Mark',1,'lectus@vitaesemper.ca',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (1,'Jelani',1,'sit.amet.orci@arcueuodio.org',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (20,'Uriel',0,'at.egestas.a@consequat.com',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (82,'Levi',0,'Nullam@molestie.edu',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (77,'Elliott',1,'Aenean.egestas.hendrerit@nuncsit.ca',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (77,'Michael',0,'semper@Curabituregestasnunc.org',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (49,'Abdul',1,'netus@eutellus.com',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (79,'Kaseem',1,'eget.lacus.Mauris@nonvestibulum.co.uk',sysdate,1);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (10,'Kevin',1,'Donec@neque.ca',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (28,'Carson',1,'tincidunt.congue@et.ca',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (49,'Brian',0,'et.risus.Quisque@magnaLoremipsum.ca',sysdate,0);
INSERT INTO TB_USER (Age,Name,Sex,Email,DTCreate,HasDelete) VALUES (33,'Drew',1,'dis.parturient.montes@mollis.ca',sysdate,1);




CREATE TABLE "REVIEWDB"."TB_ACCOUNT" 
   (	"ID" VARCHAR2(255) NOT NULL ENABLE, 
	"AGE" NUMBER, 
	"NAME" VARCHAR2(255), 
	"SEX" NUMBER(2,0), 
	"EMAIL" VARCHAR2(255), 
	"DTCREATE" DATE, 
	"HASDELETE" NUMBER(2,0), 
	"FLAGNAME" NUMBER(2,0), 
	"PASSWORD" VARCHAR2(255), 
	 PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "REVIEWDB_TS"  ENABLE
   ) SEGMENT CREATION IMMEDIATE 
  PCTFREE 10 PCTUSED 40 INITRANS 1 MAXTRANS 255 
 NOCOMPRESS LOGGING
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "REVIEWDB_TS" ;
  CREATE UNIQUE INDEX "REVIEWDB"."SYS_C0049375" ON "REVIEWDB"."TB_ACCOUNT" ("ID") 
  PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "REVIEWDB_TS" ;
  ALTER TABLE "REVIEWDB"."TB_ACCOUNT" ADD PRIMARY KEY ("ID")
  USING INDEX PCTFREE 10 INITRANS 2 MAXTRANS 255 COMPUTE STATISTICS 
  STORAGE(INITIAL 65536 NEXT 1048576 MINEXTENTS 1 MAXEXTENTS 2147483645
  PCTINCREASE 0 FREELISTS 1 FREELIST GROUPS 1
  BUFFER_POOL DEFAULT FLASH_CACHE DEFAULT CELL_FLASH_CACHE DEFAULT)
  TABLESPACE "REVIEWDB_TS"  ENABLE;
  ALTER TABLE "REVIEWDB"."TB_ACCOUNT" MODIFY ("ID" NOT NULL ENABLE);