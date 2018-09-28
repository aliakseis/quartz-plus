--connect DEV/PASSWORD


-- Database Setup

define ILC_SCHEMA=ILC;

--CREATE TABLESPACE &ILC_SCHEMA
--DATAFILE 'ILC_SCHEMA.ORA' 
--SIZE 10312K 
--REUSE AUTOEXTEND ON NEXT 25600K MAXSIZE 1008M EXTENT MANAGEMENT LOCAL
--;

--CREATE USER &ILC_SCHEMA IDENTIFIED BY &ILC_SCHEMA DEFAULT TABLESPACE &ILC_SCHEMA 
--TEMPORARY TABLESPACE TEMP QUOTA UNLIMITED ON &ILC_SCHEMA QUOTA UNLIMITED ON IDX_AUDIT;

--grant create session to &ILC_SCHEMA;

--grant create sequence to &ILC_SCHEMA;



-- Create schema objects


create sequence &ILC_SCHEMA .SQ_DATA minvalue 1 increment by 1 nocache;

CREATE SEQUENCE &ILC_SCHEMA .SQ_SERVICE_VERIFICATION 
NOCYCLE NOORDER NOMAXVALUE MINVALUE 1 INCREMENT BY 1 START WITH 1 nocache
/

CREATE SEQUENCE &ILC_SCHEMA .SQ_WORKER_ID 
NOCYCLE NOORDER NOMAXVALUE MINVALUE 1 INCREMENT BY 1 START WITH 1 nocache
/

-- tables
CREATE TABLE &ILC_SCHEMA .IVR_SERVER ( 
	IVR_SERVER_ID NUMBER(38), 
	NAME VARCHAR2(50) NOT NULL ,
	NUM_CHANNELS NUMBER(10) DEFAULT 2 NOT NULL,
	DB_CONN  VARCHAR2(2000),
	ENABLED  NUMBER(1) DEFAULT 1 NOT NULL,
	PRIMARY KEY (IVR_SERVER_ID) VALIDATE )
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING
/
CREATE TABLE &ILC_SCHEMA .IVR_PROJECT ( 
	IVR_PROJECT_ID NUMBER(38), 
	IVR_SERVER_ID NUMBER(38) NOT NULL , 
	NAME VARCHAR2(50) NOT NULL ,
	EMAIL_ADDRESSES VARCHAR2(2000) NULL ,
	ENABLED  NUMBER(1) DEFAULT 1 NOT NULL,
	PRIMARY KEY (IVR_PROJECT_ID) VALIDATE )
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING
/
CREATE TABLE &ILC_SCHEMA .SERVICE_INFO ( 
	ITEM_ID NUMBER(38), 
	TNUMBER VARCHAR2(50) NOT NULL , 
	IVR_PROJECT_ID NUMBER(38) NOT NULL , 
	USERID VARCHAR2(29) , 
	PWD VARCHAR2(100) , 
	ENABLED  NUMBER(1) DEFAULT 1 NOT NULL,
	PRIMARY KEY (ITEM_ID) VALIDATE ) 
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING
/
CREATE TABLE &ILC_SCHEMA .SERVICE_VERIFICATION ( 
	"ITEM_ID" NUMBER(38), 
	"REPORT_ID" NUMBER(38), 
	"ATTEMPTS" NUMBER(38), 
	"STATUS" VARCHAR2(50), 
	REASON     VARCHAR2(254),
	"TIME" DATE, 
	PRIMARY KEY ("REPORT_ID", "ITEM_ID") VALIDATE ) 
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING 
/

CREATE TABLE &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION ( 
"REPORT_ID" NUMBER(38) NOT NULL , "START_TIME" DATE NOT NULL , "IS_REPORTED" NUMBER(1) NOT NULL ) 
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING  
/
DELETE FROM &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION
/
INSERT INTO &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION ("REPORT_ID", "START_TIME", "IS_REPORTED") 
VALUES (0, SYSDATE-1, 1)
/

CREATE TABLE &ILC_SCHEMA .RUNTIME_CONFIGURATION ( 
	NAME VARCHAR2(50) NOT NULL ,
	VALUE VARCHAR2(2000)  ,
	PRIMARY KEY (NAME) VALIDATE )
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING
/

CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_SERVICE_INFO_ID BEFORE
INSERT
OR UPDATE OF "ITEM_ID" ON &ILC_SCHEMA .SERVICE_INFO REFERENCING OLD AS OLD NEW AS NEW FOR EACH ROW begin 
if :NEW.ITEM_ID is not null then 
raise_application_error(-20501, 'ITEMID Column read-only'); 
end if; 
if inserting then 
select &ILC_SCHEMA .SQ_DATA.nextval into :NEW.ITEM_ID from DUAL; 
end if; 
end;
/

-- constraints
ALTER TABLE &ILC_SCHEMA .IVR_PROJECT ADD FOREIGN KEY (IVR_SERVER_ID)
 REFERENCES &ILC_SCHEMA .IVR_SERVER (IVR_SERVER_ID) ENABLE VALIDATE
/
ALTER TABLE &ILC_SCHEMA .SERVICE_INFO ADD FOREIGN KEY (IVR_PROJECT_ID)
 REFERENCES &ILC_SCHEMA .IVR_PROJECT (IVR_PROJECT_ID) ENABLE VALIDATE
/
ALTER TABLE &ILC_SCHEMA .SERVICE_VERIFICATION ADD FOREIGN KEY (ITEM_ID)
 REFERENCES &ILC_SCHEMA .SERVICE_INFO (ITEM_ID) ENABLE VALIDATE
/


-- procedures
CREATE OR REPLACE PROCEDURE &ILC_SCHEMA .INITIALIZE_CHECKLIST(
		TIME_SPAN IN NUMBER, 
		WAS_MISFIRED IN NUMBER, 
		A_REPORT_ID OUT NUMBER,
		A_START_TIME OUT DATE,
		PREV_START_TIME OUT DATE,
		IS_NEW_SESSION OUT NUMBER) AS
BEGIN
	IS_NEW_SESSION := 0;
	SELECT REPORT_ID, START_TIME INTO A_REPORT_ID, A_START_TIME 
		FROM &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION FOR UPDATE;
	IF A_START_TIME < SYSDATE - TIME_SPAN THEN
		IS_NEW_SESSION := 1;
		PREV_START_TIME := A_START_TIME;
		SELECT &ILC_SCHEMA .SQ_SERVICE_VERIFICATION.NEXTVAL INTO A_REPORT_ID FROM DUAL;
		SELECT SYSDATE INTO A_START_TIME FROM DUAL;
		IF WAS_MISFIRED = 0 THEN
			INSERT INTO &ILC_SCHEMA .SERVICE_VERIFICATION(ITEM_ID, REPORT_ID, ATTEMPTS, STATUS, TIME)
				(SELECT ITEM_ID, &ILC_SCHEMA .SQ_SERVICE_VERIFICATION.CURRVAL, 0, 'AWAITING', A_START_TIME FROM &ILC_SCHEMA .SERVICE_INFO
					INNER JOIN &ILC_SCHEMA .IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID AND IVR_PROJECT.ENABLED != 0
					INNER JOIN &ILC_SCHEMA .IVR_SERVER ON IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID AND IVR_SERVER.ENABLED != 0
					WHERE SERVICE_INFO.ENABLED != 0
				);
			UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION 
				SET REPORT_ID = &ILC_SCHEMA .SQ_SERVICE_VERIFICATION.CURRVAL, START_TIME = A_START_TIME, IS_REPORTED=0;
		ELSE
			UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION 
				SET REPORT_ID = &ILC_SCHEMA .SQ_SERVICE_VERIFICATION.CURRVAL, START_TIME = A_START_TIME, IS_REPORTED=1;
		END IF;
	END IF;
	COMMIT;
END INITIALIZE_CHECKLIST;
/

CREATE OR REPLACE PROCEDURE &ILC_SCHEMA .SET_WORKER_STATUS(
WORKER_NAME IN VARCHAR2, A_REPORT_ID IN NUMBER, MAX_ATTEMPTS IN NUMBER, TIME_SPAN IN NUMBER) AS
BEGIN
	LOCK TABLE &ILC_SCHEMA .SERVICE_VERIFICATION IN EXCLUSIVE MODE;
	UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION SET STATUS=WORKER_NAME, TIME=SYSDATE
	WHERE ROWID=(SELECT ROWID FROM
		(SELECT SERVICE_VERIFICATION.ROWID 
		FROM &ILC_SCHEMA .SERVICE_VERIFICATION 
			INNER JOIN &ILC_SCHEMA .SERVICE_INFO ON SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID
			INNER JOIN &ILC_SCHEMA .IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID
			INNER JOIN &ILC_SCHEMA .IVR_SERVER THE_SERVER ON IVR_PROJECT.IVR_SERVER_ID = THE_SERVER.IVR_SERVER_ID
		WHERE REPORT_ID=A_REPORT_ID
			AND STATUS='AWAITING' AND (ATTEMPTS=0 OR (TIME<SYSDATE-TIME_SPAN AND ATTEMPTS < MAX_ATTEMPTS))
			-- check if we reach a maximum number of channels on a targeted IVR server
            AND (SELECT COUNT(1)
				FROM &ILC_SCHEMA .SERVICE_VERIFICATION 
					INNER JOIN &ILC_SCHEMA .SERVICE_INFO ON SERVICE_VERIFICATION.ITEM_ID = SERVICE_INFO.ITEM_ID
					INNER JOIN &ILC_SCHEMA .IVR_PROJECT ON SERVICE_INFO.IVR_PROJECT_ID = IVR_PROJECT.IVR_PROJECT_ID
					INNER JOIN &ILC_SCHEMA .IVR_SERVER ON IVR_PROJECT.IVR_SERVER_ID = IVR_SERVER.IVR_SERVER_ID
					INNER JOIN &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION ON SERVICE_VERIFICATION.REPORT_ID = SERVICE_VERIFICATION_SESSION.REPORT_ID
				WHERE SERVICE_VERIFICATION.STATUS LIKE 'WORKER%' 
					AND IVR_SERVER.IVR_SERVER_ID = THE_SERVER.IVR_SERVER_ID)
            < THE_SERVER.NUM_CHANNELS                           
			
		ORDER BY TIME, ATTEMPTS) WHERE ROWNUM=1);
	COMMIT;
END SET_WORKER_STATUS;
/

CREATE OR REPLACE PROCEDURE &ILC_SCHEMA .SET_RESULT_STATUS(RESULT_STATUS IN VARCHAR2, RESULT_REASON IN VARCHAR2, A_ROW_ID IN VARCHAR2) AS
BEGIN
	UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION SET STATUS=RESULT_STATUS, REASON=RESULT_REASON, ATTEMPTS=ATTEMPTS+1, TIME=SYSDATE 
	WHERE ROWID=A_ROW_ID;
END SET_RESULT_STATUS;
/

CREATE OR REPLACE PROCEDURE &ILC_SCHEMA .GET_GENERATE_REPORT_FLAG(
		A_REPORT_ID IN NUMBER, GENERATE_REPORT_FLAG OUT NUMBER) AS
	CURRENT_REPORT_ID NUMBER;
	CURRENT_IS_REPORTED NUMBER;
BEGIN
	GENERATE_REPORT_FLAG := 0;
	SELECT REPORT_ID, IS_REPORTED INTO CURRENT_REPORT_ID, CURRENT_IS_REPORTED 
		FROM &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION FOR UPDATE;
	IF CURRENT_REPORT_ID = A_REPORT_ID AND CURRENT_IS_REPORTED = 0 THEN
		GENERATE_REPORT_FLAG := 1;
		UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION SET IS_REPORTED=1;
	END IF;
	COMMIT;
END GET_GENERATE_REPORT_FLAG;
/

