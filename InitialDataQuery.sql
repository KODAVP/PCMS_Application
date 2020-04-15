DELETE FROM SETTINGS;
INSERT INTO SETTINGS (type, name, value) VALUES (0, '승인 발생시 알림 이메일', 'CHOIY28');
INSERT INTO SETTINGS (type, name, value) VALUES (1, '동의 유효기간(년)', 1);
INSERT INTO SETTINGS (type, name, value) VALUES (2, '동의 만료 알림(일)', 14);
INSERT INTO SETTINGS (type, name, value) VALUES (3, '화이자링크 알림', 'CHOIY28');


INSERT INTO CHANNEL (name, bound, type, athour, usage, Instantrun) VALUES ('PforceRX',0,0,1,false,false);
INSERT INTO CHANNEL (name, bound, type, athour, usage, Instantrun) VALUES ('MMS',0,0,1,false,false);
INSERT INTO CHANNEL (name, bound, type, athour, usage, Instantrun) VALUES ('N360',1,1,2,false,false);
INSERT INTO CHANNEL (name, bound, type, athour, usage, Instantrun) VALUES ('ODSM',0,1,3,false,false);
INSERT INTO CHANNEL (name, bound, type, athour, usage, Instantrun) VALUES ('PforceRX',1,0,4,false,false);


ALTER TABLE dbo.pcms_d ADD 
	        IND_SP_SUB VARCHAR(256) NULL
	        , KeyOpinionLeader VARCHAR(1) NULL 
	        , DoctorStatus VARCHAR(1) NULL 
	        , HospitalType VARCHAR(50) NULL 
	        , HospitalBeds VARCHAR(30) NULL 
	        , WKP_FAX VARCHAR(50) NULL 
	        , HOMEPAGE VARCHAR(256) NULL 
	        , POSN_NAME VARCHAR(256) NULL 
	        , GEO_NAME VARCHAR(256) NULL 
	        , PARENT_GEO_NAME VARCHAR(256) NULL 
	        , OWNERNAME VARCHAR(256) NULL 
        ; 