

CREATE TABLE "public".TB_USER (
id int4 NOT NULL,
age int4,
name varchar(255) DEFAULT NULL::character varying,
sex int2,
email varchar(255) DEFAULT NULL::character varying,
dtcreate date,
hasdelete int4
)
WITH (OIDS=FALSE)

;

INSERT INTO "public".TB_USER VALUES ('1', '55', 'Maxwell', '0', 'Morbi.quis@id.org', '2017-12-10', '0');
INSERT INTO "public".TB_USER VALUES ('2', '45', 'Keefe', '1', 'ipsum@vitae.com', '2017-05-06', '1');
INSERT INTO "public".TB_USER VALUES ('3', '25', 'Timon', '1', 'leo.Cras.vehicula@Crasloremlorem.org', '2016-02-11', '1');
INSERT INTO "public".TB_USER VALUES ('4', '19', 'Lars', '0', 'diam.Pellentesque.habitant@sapienCrasdolor.org', '2017-02-19', '1');
INSERT INTO "public".TB_USER VALUES ('5', '95', 'Russell', '0', 'imperdiet@lacusNullatincidunt.ca', '2016-02-21', '1');
INSERT INTO "public".TB_USER VALUES ('6', '82', 'Carter', '1', 'ipsum@fringillaeuismodenim.ca', '2017-04-08', '1');
INSERT INTO "public".TB_USER VALUES ('7', '98', 'Perry', '0', 'iaculis.aliquet.diam@lacusUtnec.org', '2017-06-03', '1');
INSERT INTO "public".TB_USER VALUES ('8', '12', 'Jermaine', '1', 'euismod.et.commodo@In.ca', '2017-01-01', '1');
INSERT INTO "public".TB_USER VALUES ('9', '92', 'Tucker', '0', 'ipsum.ac@Aeneanmassa.com', '2017-10-12', '0');
INSERT INTO "public".TB_USER VALUES ('10', '20', 'Rafael', '1', 'Donec@metusAliquamerat.net', '2016-10-15', '0');
INSERT INTO "public".TB_USER VALUES ('11', '100', 'Graiden', '0', 'posuere.enim.nisl@sapien.ca', '2017-10-08', '0');
INSERT INTO "public".TB_USER VALUES ('12', '62', 'Christian', '0', 'litora@tinciduntadipiscingMauris.net', '2016-06-10', '1');
INSERT INTO "public".TB_USER VALUES ('13', '72', 'Slade', '0', 'consectetuer@Nullamvelitdui.net', '2016-06-13', '1');
INSERT INTO "public".TB_USER VALUES ('14', '79', 'Mohammad', '1', 'tincidunt.Donec@eleifend.ca', '2016-06-27', '1');
INSERT INTO "public".TB_USER VALUES ('15', '76', 'Brandon', '1', 'Sed.eget@nonquam.com', '2017-07-23', '1');
INSERT INTO "public".TB_USER VALUES ('16', '73', 'William', '1', 'magna.et@nec.edu', '2017-02-26', '0');
INSERT INTO "public".TB_USER VALUES ('17', '97', 'Bruno', '1', 'Donec@ProinultricesDuis.edu', '2017-07-26', '1');
INSERT INTO "public".TB_USER VALUES ('18', '6', 'Elmo', '0', 'eu.tempor.erat@aliquetPhasellus.net', '2017-10-18', '0');
INSERT INTO "public".TB_USER VALUES ('19', '23', 'Hamilton', '1', 'Cras.dictum@erosProin.org', '2016-05-18', '0');
INSERT INTO "public".TB_USER VALUES ('20', '43', 'Keane', '1', 'lacus@sitametorci.org', '2017-07-02', '0');
INSERT INTO "public".TB_USER VALUES ('21', '88', 'Lionel', '0', 'ornare.Fusce@euismodmauriseu.net', '2016-08-02', '1');
INSERT INTO "public".TB_USER VALUES ('22', '51', 'Lane', '1', 'Quisque@duiinsodales.edu', '2017-08-06', '0');
INSERT INTO "public".TB_USER VALUES ('23', '54', 'Jakeem', '0', 'Nunc@ametloremsemper.co.uk', '2017-03-25', '0');
INSERT INTO "public".TB_USER VALUES ('24', '46', 'Levi', '1', 'convallis@vel.co.uk', '2017-09-13', '0');
INSERT INTO "public".TB_USER VALUES ('25', '83', 'Judah', '0', 'luctus.aliquet@Nuncacsem.co.uk', '2016-02-23', '1');
INSERT INTO "public".TB_USER VALUES ('26', '49', 'Hall', '1', 'Suspendisse@malesuadaaugue.edu', '2016-10-18', '0');
INSERT INTO "public".TB_USER VALUES ('27', '5', 'Reece', '0', 'ornare.Fusce@sodales.net', '2016-10-15', '0');
INSERT INTO "public".TB_USER VALUES ('28', '25', 'Shad', '1', 'nec.eleifend@arcu.org', '2017-07-09', '0');
INSERT INTO "public".TB_USER VALUES ('29', '64', 'Bernard', '1', 'per.inceptos@accumsanneque.net', '2016-05-06', '1');
INSERT INTO "public".TB_USER VALUES ('30', '98', 'Ryan', '0', 'Duis.sit@auctorvelit.org', '2017-08-03', '1');
INSERT INTO "public".TB_USER VALUES ('31', '70', 'Cullen', '1', 'dui@ut.co.uk', '2017-01-13', '0');
INSERT INTO "public".TB_USER VALUES ('32', '81', 'Xander', '0', 'Proin@turpisvitae.edu', '2017-08-13', '1');
INSERT INTO "public".TB_USER VALUES ('33', '42', 'Orson', '1', 'massa@fringilla.org', '2016-12-24', '1');
INSERT INTO "public".TB_USER VALUES ('34', '15', 'Elvis', '0', 'dui.Suspendisse@Aliquam.co.uk', '2016-08-04', '0');
INSERT INTO "public".TB_USER VALUES ('35', '77', 'Akeem', '1', 'Nam.consequat@estvitaesodales.net', '2017-12-06', '1');
INSERT INTO "public".TB_USER VALUES ('36', '48', 'Ferdinand', '0', 'dui.nec@urnaconvallis.ca', '2016-02-10', '0');
INSERT INTO "public".TB_USER VALUES ('37', '42', 'Kato', '0', 'Phasellus@variusNam.edu', '2016-03-16', '1');
INSERT INTO "public".TB_USER VALUES ('38', '52', 'Geoffrey', '1', 'ante@nisinibh.net', '2016-03-09', '0');
INSERT INTO "public".TB_USER VALUES ('39', '84', 'Kuame', '1', 'rutrum.magna.Cras@tacitisociosqu.ca', '2016-04-16', '0');
INSERT INTO "public".TB_USER VALUES ('40', '78', 'Camden', '1', 'vitae@tempor.ca', '2016-04-16', '1');
INSERT INTO "public".TB_USER VALUES ('41', '50', 'Kermit', '0', 'nunc.Quisque@consectetuer.org', '2017-06-10', '0');
INSERT INTO "public".TB_USER VALUES ('42', '25', 'Amal', '0', 'metus.facilisis@metusInnec.edu', '2017-08-25', '0');
INSERT INTO "public".TB_USER VALUES ('43', '13', 'Addison', '0', 'mattis.Cras@maurissit.com', '2016-09-08', '1');
INSERT INTO "public".TB_USER VALUES ('44', '42', 'Yardley', '0', 'ridiculus.mus@nuncQuisque.ca', '2016-02-29', '1');
INSERT INTO "public".TB_USER VALUES ('45', '87', 'Michael', '0', 'eu.tellus.eu@ullamcorper.ca', '2017-11-23', '0');
INSERT INTO "public".TB_USER VALUES ('46', '15', 'Giacomo', '1', 'pharetra@magna.net', '2016-08-22', '0');
INSERT INTO "public".TB_USER VALUES ('47', '13', 'Zachary', '1', 'diam.Duis@sagittisNullamvitae.org', '2017-10-26', '1');
INSERT INTO "public".TB_USER VALUES ('48', '11', 'Daniel', '0', 'interdum.libero.dui@commodoipsum.org', '2017-03-10', '0');
INSERT INTO "public".TB_USER VALUES ('49', '29', 'Sean', '0', 'malesuada.fames.ac@Curabiturdictum.com', '2016-02-20', '1');
INSERT INTO "public".TB_USER VALUES ('50', '26', 'Hector', '1', 'pede.Nunc.sed@iaculis.org', '2017-11-04', '0');
INSERT INTO "public".TB_USER VALUES ('51', '72', 'Reuben', '0', 'dictum.eleifend@sagittis.org', '2016-05-04', '0');
INSERT INTO "public".TB_USER VALUES ('52', '41', 'Sebastian', '1', 'dictum.cursus@rutrummagna.org', '2017-03-27', '1');
INSERT INTO "public".TB_USER VALUES ('53', '41', 'Jerry', '0', 'odio.Aliquam@Fuscedolor.com', '2017-10-27', '0');
INSERT INTO "public".TB_USER VALUES ('54', '42', 'Keefe', '1', 'aliquam@metuseu.com', '2017-09-29', '0');
INSERT INTO "public".TB_USER VALUES ('55', '100', 'Edward', '0', 'consectetuer@augue.net', '2016-12-22', '1');
INSERT INTO "public".TB_USER VALUES ('56', '54', 'Addison', '1', 'interdum.Nunc.sollicitudin@dignissimtemporarcu.ca', '2017-11-21', '1');
INSERT INTO "public".TB_USER VALUES ('57', '44', 'Rooney', '0', 'nascetur.ridiculus.mus@egetvenenatisa.ca', '2017-11-30', '1');
INSERT INTO "public".TB_USER VALUES ('58', '44', 'Gareth', '1', 'tincidunt@vel.org', '2017-10-03', '1');
INSERT INTO "public".TB_USER VALUES ('59', '27', 'Kenyon', '1', 'fermentum.fermentum@noncursusnon.com', '2016-03-20', '0');
INSERT INTO "public".TB_USER VALUES ('60', '84', 'Philip', '1', 'vulputate.risus@inmagnaPhasellus.ca', '2016-07-22', '1');
INSERT INTO "public".TB_USER VALUES ('61', '54', 'Drew', '0', 'est@elit.org', '2017-09-07', '0');
INSERT INTO "public".TB_USER VALUES ('62', '88', 'Robert', '0', 'Suspendisse@ullamcorperviverraMaecenas.co.uk', '2016-02-14', '0');
INSERT INTO "public".TB_USER VALUES ('63', '88', 'Melvin', '1', 'mollis.Integer.tincidunt@sollicitudin.edu', '2017-02-04', '1');
INSERT INTO "public".TB_USER VALUES ('64', '79', 'Lance', '1', 'nulla.Integer.urna@ut.edu', '2017-11-29', '1');
INSERT INTO "public".TB_USER VALUES ('65', '44', 'Tiger', '1', 'nibh.sit@purus.edu', '2016-05-19', '1');
INSERT INTO "public".TB_USER VALUES ('66', '31', 'Tate', '1', 'aliquam.iaculis@nibh.com', '2016-08-24', '1');
INSERT INTO "public".TB_USER VALUES ('67', '87', 'Cedric', '0', 'lobortis@luctusvulputate.edu', '2016-11-26', '0');
INSERT INTO "public".TB_USER VALUES ('68', '62', 'Jermaine', '1', 'elit@turpisNullaaliquet.edu', '2017-09-28', '1');
INSERT INTO "public".TB_USER VALUES ('69', '57', 'Brandon', '1', 'Vivamus.euismod@egetlaoreet.net', '2017-04-06', '1');
INSERT INTO "public".TB_USER VALUES ('70', '89', 'Kennan', '1', 'velit.Pellentesque@magnaLorem.com', '2017-10-04', '1');
INSERT INTO "public".TB_USER VALUES ('71', '67', 'Carson', '1', 'Suspendisse@temporaugueac.co.uk', '2018-01-05', '1');
INSERT INTO "public".TB_USER VALUES ('72', '34', 'Lucian', '0', 'faucibus@pedeblanditcongue.org', '2017-02-07', '1');
INSERT INTO "public".TB_USER VALUES ('73', '27', 'Orson', '1', 'Nunc.mauris@laciniamattisInteger.ca', '2016-05-20', '1');
INSERT INTO "public".TB_USER VALUES ('74', '79', 'Basil', '1', 'nec.urna.suscipit@auctorvitaealiquet.org', '2017-03-19', '0');
INSERT INTO "public".TB_USER VALUES ('75', '44', 'Addison', '0', 'viverra@egestashendreritneque.co.uk', '2016-08-30', '0');
INSERT INTO "public".TB_USER VALUES ('76', '85', 'Nero', '0', 'Proin.velit@elitpellentesque.com', '2016-04-02', '1');
INSERT INTO "public".TB_USER VALUES ('77', '81', 'Prescott', '0', 'nisi@semper.net', '2016-07-28', '1');
INSERT INTO "public".TB_USER VALUES ('78', '4', 'Emery', '0', 'nec.eleifend@tinciduntvehicula.org', '2017-10-26', '0');
INSERT INTO "public".TB_USER VALUES ('79', '69', 'Jarrod', '1', 'est@vulputatemauris.org', '2017-10-03', '1');
INSERT INTO "public".TB_USER VALUES ('80', '65', 'Carlos', '0', 'nonummy.ac.feugiat@Nullamvelit.org', '2017-06-19', '1');
INSERT INTO "public".TB_USER VALUES ('81', '26', 'Emery', '1', 'Duis@PhasellusornareFusce.co.uk', '2016-12-14', '1');
INSERT INTO "public".TB_USER VALUES ('82', '25', 'Keane', '0', 'ligula@orcilobortis.com', '2016-05-10', '1');
INSERT INTO "public".TB_USER VALUES ('83', '89', 'Rahim', '1', 'erat.vitae@aliquet.ca', '2016-07-29', '0');
INSERT INTO "public".TB_USER VALUES ('84', '49', 'Plato', '0', 'Praesent.eu.dui@sociisnatoque.ca', '2017-01-26', '0');
INSERT INTO "public".TB_USER VALUES ('85', '10', 'Barrett', '0', 'eros.Proin@semmagnanec.net', '2016-12-24', '1');
INSERT INTO "public".TB_USER VALUES ('86', '27', 'Tarik', '0', 'Sed@neccursus.edu', '2016-11-29', '0');
INSERT INTO "public".TB_USER VALUES ('87', '94', 'Quamar', '0', 'pede.Cras@Pellentesquetincidunt.net', '2017-02-07', '0');
INSERT INTO "public".TB_USER VALUES ('88', '81', 'Jasper', '1', 'risus.Donec@Utsagittis.com', '2016-12-11', '0');
INSERT INTO "public".TB_USER VALUES ('89', '31', 'Magee', '0', 'ipsum@enim.net', '2017-09-07', '0');
INSERT INTO "public".TB_USER VALUES ('90', '15', 'Paki', '0', 'euismod.est@malesuada.org', '2017-01-11', '0');
INSERT INTO "public".TB_USER VALUES ('91', '100', 'Thane', '0', 'magna.et@massa.net', '2016-05-10', '1');
INSERT INTO "public".TB_USER VALUES ('92', '96', 'Todd', '0', 'enim.commodo.hendrerit@arcu.ca', '2016-02-15', '1');
INSERT INTO "public".TB_USER VALUES ('93', '13', 'Kennedy', '0', 'arcu.Vestibulum@porttitorinterdumSed.edu', '2016-04-27', '0');
INSERT INTO "public".TB_USER VALUES ('94', '15', 'Jerome', '1', 'eleifend.egestas@nec.ca', '2016-05-03', '1');
INSERT INTO "public".TB_USER VALUES ('95', '46', 'Christian', '1', 'eget@Maurisblandit.org', '2017-12-27', '0');
INSERT INTO "public".TB_USER VALUES ('96', '88', 'Emerson', '0', 'Aliquam@cursus.edu', '2017-05-08', '0');
INSERT INTO "public".TB_USER VALUES ('97', '28', 'Rogan', '1', 'ac@purusNullam.edu', '2016-08-10', '1');
INSERT INTO "public".TB_USER VALUES ('98', '43', 'Porter', '0', 'euismod@luctus.ca', '2016-10-22', '1');
INSERT INTO "public".TB_USER VALUES ('99', '31', 'Richard', '1', 'Nullam.lobortis@inceptoshymenaeos.co.uk', '2017-09-16', '0');
INSERT INTO "public".TB_USER VALUES ('100', '69', 'Basil', '1', 'imperdiet.ornare.In@semperrutrumFusce.com', '2016-11-13', '1');
