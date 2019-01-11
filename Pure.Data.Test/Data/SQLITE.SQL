/*
Navicat SQLite Data Transfer

Source Server         : testsqlite
Source Server Version : 30802
Source Host           : :0

Target Server Type    : SQLite
Target Server Version : 30802
File Encoding         : 65001

Date: 2017-01-20 15:31:26
*/

PRAGMA foreign_keys = OFF;

-- ----------------------------
-- Table structure for TB_USER
-- ----------------------------
DROP TABLE IF EXISTS "main"."TB_USER";
CREATE TABLE "TB_USER" (
"id"  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Age"  INTEGER DEFAULT NULL,
"Name"  TEXT DEFAULT NULL,
"Sex"  INTEGER DEFAULT NULL,
"Email"  TEXT DEFAULT NULL,
"DTCreate"  TEXT,
"HasDelete"  INTEGER DEFAULT NULL
);

-- ----------------------------
-- Records of TB_USER
-- ----------------------------
INSERT INTO "main"."TB_USER" VALUES (101, 68, 'Wyatt', 0, 'nibh@velitPellentesque.ca', '2017-10-20 16:04:37', 1);
INSERT INTO "main"."TB_USER" VALUES (102, 32, 'Oren', 1, 'lectus.quis@loremfringilla.net', '2017-02-23 23:19:33', 0);
INSERT INTO "main"."TB_USER" VALUES (103, 94, 'Acton', 0, 'posuere@nonloremvitae.ca', '2017-08-08 14:31:06', 0);
INSERT INTO "main"."TB_USER" VALUES (104, 86, 'Vance', 1, 'mollis.Integer@tempusscelerisque.co.uk', '2017-09-23 00:39:10', 0);
INSERT INTO "main"."TB_USER" VALUES (105, 24, 'Chandler', 0, 'tellus.faucibus.leo@rutrummagna.com', '2017-07-01 14:15:49', 0);
INSERT INTO "main"."TB_USER" VALUES (106, 58, 'Callum', 0, 'massa.lobortis@placerat.edu', '2018-01-01 01:44:56', 0);
INSERT INTO "main"."TB_USER" VALUES (107, 11, 'Elliott', 0, 'pharetra.sed@eget.co.uk', '2017-01-23 04:43:11', 0);
INSERT INTO "main"."TB_USER" VALUES (108, 19, 'Jin', 0, 'arcu@Donec.org', '2017-07-17 13:42:24', 1);
INSERT INTO "main"."TB_USER" VALUES (109, 73, 'Rashad', 0, 'ac.mi.eleifend@sagittisfelisDonec.co.uk', '2016-02-05 02:20:39', 0);
INSERT INTO "main"."TB_USER" VALUES (110, 98, 'Rahim', 1, 'justo@ornare.ca', '2016-06-09 18:46:58', 0);
INSERT INTO "main"."TB_USER" VALUES (111, 55, 'Joel', 1, 'mus.Donec@dolor.co.uk', '2017-06-16 01:39:52', 0);
INSERT INTO "main"."TB_USER" VALUES (112, 73, 'Robert', 0, 'aliquet.nec.imperdiet@dolorDonecfringilla.net', '2016-11-29 22:05:56', 1);
INSERT INTO "main"."TB_USER" VALUES (113, 25, 'Roth', 1, 'velit.Pellentesque@eulacusQuisque.co.uk', '2017-07-30 20:24:37', 0);
INSERT INTO "main"."TB_USER" VALUES (114, 17, 'Emery', 1, 'dapibus.gravida@Sedetlibero.com', '2016-12-01 17:27:55', 0);
INSERT INTO "main"."TB_USER" VALUES (115, 25, 'Giacomo', 0, 'nunc.id@adlitora.net', '2017-06-03 22:02:51', 1);
INSERT INTO "main"."TB_USER" VALUES (116, 63, 'Emerson', 1, 'interdum@temporarcuVestibulum.edu', '2016-01-28 09:02:32', 0);
INSERT INTO "main"."TB_USER" VALUES (117, 49, 'Jermaine', 0, 'Donec@quisarcuvel.edu', '2016-08-18 16:48:47', 1);
INSERT INTO "main"."TB_USER" VALUES (118, 86, 'Arsenio', 0, 'nec.enim@urnasuscipitnonummy.ca', '2016-12-06 02:30:34', 1);
INSERT INTO "main"."TB_USER" VALUES (119, 75, 'Dexter', 0, 'nec.quam@tincidunt.edu', '2017-11-25 01:18:38', 1);
INSERT INTO "main"."TB_USER" VALUES (120, 100, 'Oleg', 1, 'Vivamus.non.lorem@id.co.uk', '2017-02-21 07:53:21', 0);
INSERT INTO "main"."TB_USER" VALUES (121, 15, 'Wade', 1, 'ut.nisi.a@elitEtiamlaoreet.com', '2017-09-03 23:13:33', 0);
INSERT INTO "main"."TB_USER" VALUES (122, 36, 'Price', 1, 'Proin@arcu.net', '2016-04-02 06:30:30', 1);
INSERT INTO "main"."TB_USER" VALUES (123, 10, 'Jordan', 1, 'vehicula.et.rutrum@enim.ca', '2016-04-21 06:59:58', 1);
INSERT INTO "main"."TB_USER" VALUES (124, 59, 'Dante', 1, 'enim.commodo@pedenecante.net', '2017-04-14 13:55:54', 1);
INSERT INTO "main"."TB_USER" VALUES (125, 31, 'Sawyer', 0, 'nascetur.ridiculus@Proin.com', '2017-11-09 06:37:24', 0);
INSERT INTO "main"."TB_USER" VALUES (126, 60, 'Vincent', 1, 'Fusce.diam@mollis.edu', '2017-01-13 04:46:06', 0);
INSERT INTO "main"."TB_USER" VALUES (127, 45, 'Knox', 0, 'mollis@auctor.co.uk', '2016-06-05 10:39:23', 0);
INSERT INTO "main"."TB_USER" VALUES (128, 33, 'Owen', 0, 'bibendum.sed@Aliquamrutrumlorem.net', '2017-12-11 13:55:55', 0);
INSERT INTO "main"."TB_USER" VALUES (129, 27, 'Ferdinand', 1, 'iaculis.enim@ridiculusmusAenean.org', '2017-07-06 04:45:43', 1);
INSERT INTO "main"."TB_USER" VALUES (130, 98, 'Merritt', 0, 'elit.fermentum@mifringillami.org', '2017-04-25 08:00:53', 1);
INSERT INTO "main"."TB_USER" VALUES (131, 74, 'Eaton', 0, 'cursus.luctus@lectusNullamsuscipit.com', '2018-01-02 00:37:59', 1);
INSERT INTO "main"."TB_USER" VALUES (132, 41, 'Noah', 0, 'sem@lobortisquam.ca', '2016-03-17 18:41:39', 0);
INSERT INTO "main"."TB_USER" VALUES (133, 81, 'Plato', 0, 'ipsum@lorem.edu', '2016-10-06 16:48:44', 0);
INSERT INTO "main"."TB_USER" VALUES (134, 61, 'Dale', 1, 'Donec.nibh.Quisque@velit.net', '2016-08-09 13:05:56', 1);
INSERT INTO "main"."TB_USER" VALUES (135, 14, 'Boris', 0, 'iaculis.enim@Morbi.org', '2016-01-16 03:40:31', 1);
INSERT INTO "main"."TB_USER" VALUES (136, 3, 'Gavin', 1, 'ut@ligulaAenean.net', '2016-08-02 03:25:20', 0);
INSERT INTO "main"."TB_USER" VALUES (137, 62, 'Oleg', 0, 'blandit.at@aarcuSed.com', '2016-10-09 19:07:22', 1);
INSERT INTO "main"."TB_USER" VALUES (138, 31, 'Joel', 1, 'nunc.Quisque@anteiaculisnec.net', '2016-05-21 00:53:20', 1);
INSERT INTO "main"."TB_USER" VALUES (139, 91, 'Hashim', 0, 'imperdiet.dictum.magna@augueac.net', '2016-07-31 03:21:01', 0);
INSERT INTO "main"."TB_USER" VALUES (140, 68, 'Salvador', 0, 'Aliquam@parturient.net', '2017-06-25 20:17:32', 1);
INSERT INTO "main"."TB_USER" VALUES (141, 99, 'Rafael', 0, 'fermentum@odioAliquamvulputate.ca', '2016-08-29 15:02:23', 1);
INSERT INTO "main"."TB_USER" VALUES (142, 86, 'Jelani', 0, 'consectetuer@venenatisa.org', '2016-02-04 07:15:09', 1);
INSERT INTO "main"."TB_USER" VALUES (143, 1, 'Raja', 1, 'nibh@ligulaAliquam.ca', '2017-12-07 02:53:25', 0);
INSERT INTO "main"."TB_USER" VALUES (144, 46, 'Omar', 1, 'Donec.at.arcu@etlibero.net', '2016-05-31 23:31:21', 1);
INSERT INTO "main"."TB_USER" VALUES (145, 76, 'Wing', 0, 'pharetra.felis.eget@euismod.net', '2017-07-13 02:05:01', 1);
INSERT INTO "main"."TB_USER" VALUES (146, 10, 'Sebastian', 1, 'non.quam.Pellentesque@temporloremeget.com', '2017-03-11 04:15:32', 1);
INSERT INTO "main"."TB_USER" VALUES (147, 48, 'Guy', 0, 'lacus.varius.et@acurnaUt.edu', '2016-03-06 12:40:54', 0);
INSERT INTO "main"."TB_USER" VALUES (148, 7, 'Carl', 0, 'eu.sem@vestibulum.co.uk', '2017-08-21 12:19:52', 0);
INSERT INTO "main"."TB_USER" VALUES (149, 48, 'Eaton', 1, 'orci.Phasellus.dapibus@blanditatnisi.com', '2017-09-09 23:34:51', 0);
INSERT INTO "main"."TB_USER" VALUES (150, 38, 'Baker', 0, 'odio@egestas.edu', '2017-10-04 02:30:00', 0);
INSERT INTO "main"."TB_USER" VALUES (151, 86, 'Clayton', 1, 'diam@commodoipsum.edu', '2017-11-05 07:31:30', 1);
INSERT INTO "main"."TB_USER" VALUES (152, 38, 'Tyler', 1, 'purus.ac@auctorvelit.com', '2016-04-28 10:14:17', 1);
INSERT INTO "main"."TB_USER" VALUES (153, 100, 'Leo', 1, 'sagittis@odioAliquam.ca', '2016-02-01 13:46:24', 1);
INSERT INTO "main"."TB_USER" VALUES (154, 74, 'Nissim', 0, 'In@tincidunt.ca', '2017-01-22 04:14:13', 0);
INSERT INTO "main"."TB_USER" VALUES (155, 80, 'Hunter', 1, 'vestibulum.nec@incursus.org', '2017-10-22 12:42:13', 1);
INSERT INTO "main"."TB_USER" VALUES (156, 97, 'Axel', 1, 'id.magna@lobortis.com', '2016-11-05 15:53:17', 1);
INSERT INTO "main"."TB_USER" VALUES (157, 55, 'Flynn', 0, 'Morbi.accumsan@Phaselluselitpede.edu', '2017-06-11 22:14:48', 1);
INSERT INTO "main"."TB_USER" VALUES (158, 5, 'Myles', 0, 'rutrum.urna@interdumlibero.edu', '2017-07-19 13:04:02', 0);
INSERT INTO "main"."TB_USER" VALUES (159, 38, 'Deacon', 0, 'cursus@porttitortellusnon.org', '2016-07-13 08:01:31', 1);
INSERT INTO "main"."TB_USER" VALUES (160, 20, 'Joel', 1, 'congue.In@Nunclectus.org', '2016-08-05 08:27:39', 0);
INSERT INTO "main"."TB_USER" VALUES (161, 8, 'Rogan', 0, 'pharetra.Nam.ac@velitSedmalesuada.net', '2016-05-21 14:07:08', 0);
INSERT INTO "main"."TB_USER" VALUES (162, 97, 'Paul', 1, 'auctor@sedleoCras.org', '2016-12-20 11:14:40', 0);
INSERT INTO "main"."TB_USER" VALUES (163, 47, 'Akeem', 1, 'ac.turpis.egestas@auctorvitae.ca', '2016-06-27 10:17:47', 0);
INSERT INTO "main"."TB_USER" VALUES (164, 73, 'Christian', 1, 'rutrum@Maecenasmi.net', '2016-08-03 22:28:58', 1);
INSERT INTO "main"."TB_USER" VALUES (165, 96, 'George', 0, 'tempor@musAenean.ca', '2017-01-07 06:33:59', 1);
INSERT INTO "main"."TB_USER" VALUES (166, 74, 'Hilel', 0, 'scelerisque.neque@sapien.ca', '2017-07-11 07:07:57', 1);
INSERT INTO "main"."TB_USER" VALUES (167, 40, 'Zane', 1, 'malesuada@vehicula.co.uk', '2016-04-04 06:44:30', 0);
INSERT INTO "main"."TB_USER" VALUES (168, 33, 'Brody', 1, 'risus.Nunc@DonecfringillaDonec.co.uk', '2016-12-19 22:26:23', 1);
INSERT INTO "main"."TB_USER" VALUES (169, 81, 'Blake', 1, 'Cras.vulputate@diamProin.co.uk', '2017-06-01 22:39:12', 0);
INSERT INTO "main"."TB_USER" VALUES (170, 22, 'Barry', 1, 'ultrices.iaculis.odio@mauriserat.org', '2017-05-10 08:10:21', 1);
INSERT INTO "main"."TB_USER" VALUES (171, 34, 'Allistair', 1, 'elit@vestibulumnec.co.uk', '2017-07-12 21:55:03', 1);
INSERT INTO "main"."TB_USER" VALUES (172, 28, 'Adam', 1, 'Cras.eget.nisi@neccursusa.ca', '2016-08-20 02:28:14', 0);
INSERT INTO "main"."TB_USER" VALUES (173, 92, 'Macaulay', 1, 'amet@Morbiaccumsanlaoreet.net', '2017-07-11 15:33:37', 1);
INSERT INTO "main"."TB_USER" VALUES (174, 77, 'Bruno', 1, 'et@metus.org', '2017-06-05 12:32:49', 1);
INSERT INTO "main"."TB_USER" VALUES (175, 44, 'Geoffrey', 0, 'commodo.auctor@Pellentesquetincidunt.net', '2016-10-16 07:03:34', 1);
INSERT INTO "main"."TB_USER" VALUES (176, 76, 'Abel', 1, 'tincidunt.congue.turpis@uteratSed.com', '2017-04-18 04:02:35', 0);
INSERT INTO "main"."TB_USER" VALUES (177, 61, 'Elijah', 1, 'odio@augueSed.com', '2017-12-12 06:56:57', 0);
INSERT INTO "main"."TB_USER" VALUES (178, 79, 'Clark', 0, 'ac.mi.eleifend@non.org', '2016-11-21 05:45:05', 0);
INSERT INTO "main"."TB_USER" VALUES (179, 84, 'Mark', 1, 'ac@etnetus.org', '2017-12-06 19:21:06', 1);
INSERT INTO "main"."TB_USER" VALUES (180, 5, 'Dalton', 1, 'arcu.Nunc.mauris@sit.ca', '2017-12-18 05:39:35', 1);
INSERT INTO "main"."TB_USER" VALUES (181, 74, 'Jeremy', 0, 'posuere.vulputate@senectus.ca', '2017-05-19 14:40:36', 0);
INSERT INTO "main"."TB_USER" VALUES (182, 90, 'Ethan', 1, 'leo.Cras@nonduinec.com', '2016-02-22 08:46:53', 0);
INSERT INTO "main"."TB_USER" VALUES (183, 13, 'Keith', 1, 'Vivamus.rhoncus.Donec@velarcu.com', '2016-09-03 04:12:09', 1);
INSERT INTO "main"."TB_USER" VALUES (184, 77, 'Dylan', 0, 'pharetra.Quisque.ac@uteros.net', '2017-11-08 16:10:56', 0);
INSERT INTO "main"."TB_USER" VALUES (185, 10, 'Lucius', 0, 'neque.et@tinciduntadipiscingMauris.edu', '2017-02-24 21:17:25', 0);
INSERT INTO "main"."TB_USER" VALUES (186, 29, 'Rashad', 1, 'Nullam.scelerisque@Nullaeget.ca', '2017-07-07 05:07:43', 0);
INSERT INTO "main"."TB_USER" VALUES (187, 24, 'Lucas', 0, 'lobortis@cursusluctusipsum.ca', '2017-01-11 15:29:35', 1);
INSERT INTO "main"."TB_USER" VALUES (188, 86, 'Felix', 1, 'egestas@Sednullaante.com', '2016-05-11 01:05:56', 1);
INSERT INTO "main"."TB_USER" VALUES (189, 60, 'Fletcher', 0, 'eu.turpis.Nulla@lobortis.org', '2016-08-20 20:33:33', 0);
INSERT INTO "main"."TB_USER" VALUES (190, 96, 'Dane', 0, 'Cras.pellentesque.Sed@mauris.org', '2017-05-21 07:04:55', 1);
INSERT INTO "main"."TB_USER" VALUES (191, 35, 'Brian', 0, 'Donec.non@erat.net', '2017-02-10 03:01:52', 1);
INSERT INTO "main"."TB_USER" VALUES (192, 47, 'Baker', 0, 'blandit@dictummagna.net', '2017-05-13 23:49:07', 1);
INSERT INTO "main"."TB_USER" VALUES (193, 96, 'Paul', 1, 'ullamcorper.Duis@magnaa.co.uk', '2017-09-27 03:15:52', 1);
INSERT INTO "main"."TB_USER" VALUES (194, 67, 'Ahmed', 0, 'aliquet.molestie@ipsumSuspendissesagittis.com', '2017-05-03 07:15:25', 0);
INSERT INTO "main"."TB_USER" VALUES (195, 86, 'Preston', 1, 'dis.parturient.montes@dui.net', '2017-04-16 06:18:01', 0);
INSERT INTO "main"."TB_USER" VALUES (196, 100, 'Derek', 1, 'vehicula@tellus.net', '2016-06-16 22:22:44', 1);
INSERT INTO "main"."TB_USER" VALUES (197, 67, 'Ahmed', 0, 'Nullam.nisl@consequat.co.uk', '2016-08-17 10:57:17', 0);
INSERT INTO "main"."TB_USER" VALUES (198, 10, 'Xenos', 1, 'mauris.a.nunc@Morbi.com', '2016-01-26 05:13:19', 1);
INSERT INTO "main"."TB_USER" VALUES (199, 49, 'Rudyard', 1, 'at.auctor.ullamcorper@feugiatSed.net', '2016-12-20 07:23:01', 1);
INSERT INTO "main"."TB_USER" VALUES (200, 94, 'Abbot', 0, 'auctor.quis.tristique@nislMaecenas.ca', '2017-04-27 16:33:01', 0);
INSERT INTO "main"."TB_USER" VALUES (201, null, '李四', 1, '123456@qq.com', null, null);
