define ILC_SCHEMA=ILC;

CREATE TABLE &ILC_SCHEMA .ILC_SCENARIO ( 
	ILC_SCENARIO_ID NUMBER(38), 
	NAME VARCHAR2(50) NOT NULL ,
	ASSEMBLIES  VARCHAR2(2000),
	SCRIPTING_EXPRESSION  VARCHAR2(4000) NOT NULL,
	PRIMARY KEY (ILC_SCENARIO_ID) VALIDATE )
TABLESPACE &ILC_SCHEMA PCTFREE 10 INITRANS 1 MAXTRANS 255 
STORAGE ( INITIAL 64K BUFFER_POOL DEFAULT) LOGGING
/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_ILC_SCENARIO_ID BEFORE
INSERT
OR UPDATE OF ILC_SCENARIO_ID ON &ILC_SCHEMA .ILC_SCENARIO REFERENCING OLD AS OLD NEW AS NEW FOR EACH ROW
begin
if :NEW.ILC_SCENARIO_ID is not null then
raise_application_error(-20501, 'ILC_SCENARIO_ID Column read-only');
end if;
if inserting then
select &ILC_SCHEMA .SQ_DATA.nextval into :NEW.ILC_SCENARIO_ID from DUAL;
end if;
end;
/

ALTER TABLE &ILC_SCHEMA .ILC_INSTANCE
ADD (ILC_SCENARIO_ID NUMBER(38))
/
ALTER TABLE &ILC_SCHEMA .ILC_INSTANCE
ADD 
FOREIGN KEY
  (ILC_SCENARIO_ID)
REFERENCES
  &ILC_SCHEMA .ILC_SCENARIO
  (ILC_SCENARIO_ID)
ENABLE
VALIDATE
/

ALTER TABLE &ILC_SCHEMA .IVR_SERVER
ADD (ILC_SCENARIO_ID NUMBER(38))
/
ALTER TABLE &ILC_SCHEMA .IVR_SERVER
ADD 
FOREIGN KEY
  (ILC_SCENARIO_ID)
REFERENCES
  &ILC_SCHEMA .ILC_SCENARIO
  (ILC_SCENARIO_ID)
ENABLE
VALIDATE
/
ALTER TABLE &ILC_SCHEMA .IVR_PROJECT
ADD (ILC_SCENARIO_ID NUMBER(38))
/
ALTER TABLE &ILC_SCHEMA .IVR_PROJECT
ADD 
FOREIGN KEY
  (ILC_SCENARIO_ID)
REFERENCES
  &ILC_SCHEMA .ILC_SCENARIO
  (ILC_SCENARIO_ID)
ENABLE
VALIDATE
/
ALTER TABLE &ILC_SCHEMA .SERVICE_INFO
ADD (ILC_SCENARIO_ID NUMBER(38))
/
ALTER TABLE &ILC_SCHEMA .SERVICE_INFO
ADD 
FOREIGN KEY
  (ILC_SCENARIO_ID)
REFERENCES
  &ILC_SCHEMA .ILC_SCENARIO
  (ILC_SCENARIO_ID)
ENABLE
VALIDATE
/

ALTER TABLE &ILC_SCHEMA .ILC_INSTANCE
ADD (SCRIPTS_VERSION NUMBER(38) DEFAULT 1 NOT NULL)
/

CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_ILC_SCENARIO_VERSION
AFTER INSERT OR DELETE OR UPDATE ON &ILC_SCHEMA .ILC_SCENARIO
BEGIN
    UPDATE &ILC_SCHEMA .ILC_INSTANCE SET SCRIPTS_VERSION = SCRIPTS_VERSION + 1;
END;
/

-- Obsolete
DROP PROCEDURE &ILC_SCHEMA .SET_WORKER_STATUS
/

-- Note: this function performs a DML operation!
CREATE OR REPLACE FUNCTION &ILC_SCHEMA .GET_WORKER_ID(
		WORKER_NAME IN VARCHAR2, A_REPORT_ID IN NUMBER, MAX_ATTEMPTS IN NUMBER, TIME_SPAN IN NUMBER) 
		RETURN VARCHAR2 AS
	PRAGMA AUTONOMOUS_TRANSACTION;
	A_ROW_ID ROWID;
BEGIN
	LOCK TABLE &ILC_SCHEMA .SERVICE_VERIFICATION IN EXCLUSIVE MODE;
	
	SELECT ROWID INTO A_ROW_ID FROM
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
			
		ORDER BY TIME, ATTEMPTS) WHERE ROWNUM=1;
	
	UPDATE &ILC_SCHEMA .SERVICE_VERIFICATION SET STATUS=WORKER_NAME, TIME=SYSDATE WHERE ROWID=A_ROW_ID;
	COMMIT;
	RETURN A_ROW_ID;
EXCEPTION
	WHEN NO_DATA_FOUND THEN 
		--A_ROW_ID := NULL;
		ROLLBACK;
		RETURN NULL;
END GET_WORKER_ID;
/

/* Only one row is allowed in the SERVICE_VERIFICATION_SESSION*/
CREATE OR REPLACE
TRIGGER &ILC_SCHEMA .TR_SERVICE_VER_SESSION_DEL_INS
before Insert or Delete
on &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION
for each row
begin
    raise_application_error(-20502, 'Can not delete from or insert into SERVICE_VERIFICATION_SESSION!');
end;
/


-- Audit functionality update


/*ILC_INSTANCE UPDATE TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_ILC_INSTANCE_UPD AFTER
        UPDATE
        ON &ILC_SCHEMA .ILC_INSTANCE FOR EACH row BEGIN IF :NEW.SCHEDULE_CRON <>
               :OLD.SCHEDULE_CRON
            OR :NEW.SCHEDULE_CRON IS NOT NULL
           AND :OLD.SCHEDULE_CRON IS NULL
            OR :OLD.SCHEDULE_CRON IS NOT NULL
           AND :NEW.SCHEDULE_CRON IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                          ,
                      'ILC_INSTANCE'             ,
                      'SCHEDULE_CRON'            ,
                      TO_CHAR(:OLD.SCHEDULE_CRON),
                      TO_CHAR(:NEW.SCHEDULE_CRON),
                      'U'
               );

END IF;
IF :NEW.OUT_CHANNEL_NUM <> :OLD.OUT_CHANNEL_NUM OR :NEW.OUT_CHANNEL_NUM IS NOT NULL AND
        :OLD.OUT_CHANNEL_NUM IS NULL OR :OLD.OUT_CHANNEL_NUM IS NOT NULL AND
        :NEW.OUT_CHANNEL_NUM IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                            ,
                      'ILC_INSTANCE'               ,
                      'OUT_CHANNEL_NUM'            ,
                      TO_CHAR(:OLD.OUT_CHANNEL_NUM),
                      TO_CHAR(:NEW.OUT_CHANNEL_NUM),
                      'U'
               );

END IF;
IF :NEW.TERMINATOR_KEY <> :OLD.TERMINATOR_KEY OR :NEW.TERMINATOR_KEY IS NOT NULL AND
        :OLD.TERMINATOR_KEY IS NULL OR :OLD.TERMINATOR_KEY IS NOT NULL AND
        :NEW.TERMINATOR_KEY IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                           ,
                      'ILC_INSTANCE'              ,
                      'TERMINATOR_KEY'            ,
                      TO_CHAR(:OLD.TERMINATOR_KEY),
                      TO_CHAR(:NEW.TERMINATOR_KEY),
                      'U'
               );

END IF;
IF :NEW.TIMESPAN <> :OLD.TIMESPAN OR :NEW.TIMESPAN IS NOT NULL AND :OLD.TIMESPAN IS NULL
        OR :OLD.TIMESPAN IS NOT NULL AND :NEW.TIMESPAN IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                     ,
                      'ILC_INSTANCE'        ,
                      'TIMESPAN'            ,
                      TO_CHAR(:OLD.TIMESPAN),
                      TO_CHAR(:NEW.TIMESPAN),
                      'U'
               );

END IF;
IF :NEW.COMMON_RECIPIENT_EMAIL <> :OLD.COMMON_RECIPIENT_EMAIL OR
        :NEW.COMMON_RECIPIENT_EMAIL IS NOT NULL AND :OLD.COMMON_RECIPIENT_EMAIL IS NULL
        OR :OLD.COMMON_RECIPIENT_EMAIL IS NOT NULL AND :NEW.COMMON_RECIPIENT_EMAIL
        IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                   ,
                      'ILC_INSTANCE'                      ,
                      'COMMON_RECIPIENT_EMAIL'            ,
                      TO_CHAR(:OLD.COMMON_RECIPIENT_EMAIL),
                      TO_CHAR(:NEW.COMMON_RECIPIENT_EMAIL),
                      'U'
               );

END IF;
IF :NEW.TIME_BETWEEN_VERIFICATION <> :OLD.TIME_BETWEEN_VERIFICATION OR
        :NEW.TIME_BETWEEN_VERIFICATION IS NOT NULL AND :OLD.TIME_BETWEEN_VERIFICATION
        IS NULL OR :OLD.TIME_BETWEEN_VERIFICATION IS NOT NULL AND
        :NEW.TIME_BETWEEN_VERIFICATION IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                      ,
                      'ILC_INSTANCE'                         ,
                      'TIME_BETWEEN_VERIFICATION'            ,
                      TO_CHAR(:OLD.TIME_BETWEEN_VERIFICATION),
                      TO_CHAR(:NEW.TIME_BETWEEN_VERIFICATION),
                      'U'
               );

END IF;
IF :NEW.MAX_ATTEMPTS <> :OLD.MAX_ATTEMPTS OR :NEW.MAX_ATTEMPTS IS NOT NULL AND
        :OLD.MAX_ATTEMPTS IS NULL OR :OLD.MAX_ATTEMPTS IS NOT NULL AND :NEW.MAX_ATTEMPTS
        IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                         ,
                      'ILC_INSTANCE'            ,
                      'MAX_ATTEMPTS'            ,
                      TO_CHAR(:OLD.MAX_ATTEMPTS),
                      TO_CHAR(:NEW.MAX_ATTEMPTS),
                      'U'
               );

END IF;
IF :NEW.FROM_EMAIL <> :OLD.FROM_EMAIL OR :NEW.FROM_EMAIL IS NOT NULL AND :OLD.FROM_EMAIL
        IS NULL OR :OLD.FROM_EMAIL IS NOT NULL AND :NEW.FROM_EMAIL IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                       ,
                      'ILC_INSTANCE'          ,
                      'FROM_EMAIL'            ,
                      TO_CHAR(:OLD.FROM_EMAIL),
                      TO_CHAR(:NEW.FROM_EMAIL),
                      'U'
               );

END IF;
IF :NEW.SILENCE_TIMEOUT <> :OLD.SILENCE_TIMEOUT OR :NEW.SILENCE_TIMEOUT IS NOT NULL AND
        :OLD.SILENCE_TIMEOUT IS NULL OR :OLD.SILENCE_TIMEOUT IS NOT NULL AND
        :NEW.SILENCE_TIMEOUT IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                            ,
                      'ILC_INSTANCE'               ,
                      'SILENCE_TIMEOUT'            ,
                      TO_CHAR(:OLD.SILENCE_TIMEOUT),
                      TO_CHAR(:NEW.SILENCE_TIMEOUT),
                      'U'
               );

END IF;
IF :NEW.RECORD_DURATION <> :OLD.RECORD_DURATION OR :NEW.RECORD_DURATION IS NOT NULL AND
        :OLD.RECORD_DURATION IS NULL OR :OLD.RECORD_DURATION IS NOT NULL AND
        :NEW.RECORD_DURATION IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                            ,
                      'ILC_INSTANCE'               ,
                      'RECORD_DURATION'            ,
                      TO_CHAR(:OLD.RECORD_DURATION),
                      TO_CHAR(:NEW.RECORD_DURATION),
                      'U'
               );

END IF;
IF :NEW.LOGIN_SILENCE_TIMEOUT <> :OLD.LOGIN_SILENCE_TIMEOUT OR :NEW.LOGIN_SILENCE_TIMEOUT
        IS NOT NULL AND :OLD.LOGIN_SILENCE_TIMEOUT IS NULL OR :OLD.LOGIN_SILENCE_TIMEOUT
        IS NOT NULL AND :NEW.LOGIN_SILENCE_TIMEOUT IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                  ,
                      'ILC_INSTANCE'                     ,
                      'LOGIN_SILENCE_TIMEOUT'            ,
                      TO_CHAR(:OLD.LOGIN_SILENCE_TIMEOUT),
                      TO_CHAR(:NEW.LOGIN_SILENCE_TIMEOUT),
                      'U'
               );

END IF;
IF :NEW.SUMMARY_REPORT_CRON <> :OLD.SUMMARY_REPORT_CRON OR :NEW.SUMMARY_REPORT_CRON
        IS NOT NULL AND :OLD.SUMMARY_REPORT_CRON IS NULL OR :OLD.SUMMARY_REPORT_CRON
        IS NOT NULL AND :NEW.SUMMARY_REPORT_CRON IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                ,
                      'ILC_INSTANCE'                   ,
                      'SUMMARY_REPORT_CRON'            ,
                      TO_CHAR(:OLD.SUMMARY_REPORT_CRON),
                      TO_CHAR(:NEW.SUMMARY_REPORT_CRON),
                      'U'
               );

END IF;
IF :NEW.SUMMARY_RECIPIENT_EMAIL <> :OLD.SUMMARY_RECIPIENT_EMAIL OR
        :NEW.SUMMARY_RECIPIENT_EMAIL IS NOT NULL AND :OLD.SUMMARY_RECIPIENT_EMAIL IS NULL
        OR :OLD.SUMMARY_RECIPIENT_EMAIL IS NOT NULL AND :NEW.SUMMARY_RECIPIENT_EMAIL
        IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                    ,
                      'ILC_INSTANCE'                       ,
                      'SUMMARY_RECIPIENT_EMAIL'            ,
                      TO_CHAR(:OLD.SUMMARY_RECIPIENT_EMAIL),
                      TO_CHAR(:NEW.SUMMARY_RECIPIENT_EMAIL),
                      'U'
               );

END IF;
IF :NEW.JOB_MISFIRE_THRESHOLD <> :OLD.JOB_MISFIRE_THRESHOLD OR :NEW.JOB_MISFIRE_THRESHOLD
        IS NOT NULL AND :OLD.JOB_MISFIRE_THRESHOLD IS NULL OR :OLD.JOB_MISFIRE_THRESHOLD
        IS NOT NULL AND :NEW.JOB_MISFIRE_THRESHOLD IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                  ,
                      'ILC_INSTANCE'                     ,
                      'JOB_MISFIRE_THRESHOLD'            ,
                      TO_CHAR(:OLD.JOB_MISFIRE_THRESHOLD),
                      TO_CHAR(:NEW.JOB_MISFIRE_THRESHOLD),
                      'U'
               );

END IF;
IF :NEW.ILC_SCENARIO_ID <> :OLD.ILC_SCENARIO_ID OR :NEW.ILC_SCENARIO_ID
        IS NOT NULL AND :OLD.ILC_SCENARIO_ID IS NULL OR :OLD.ILC_SCENARIO_ID
        IS NOT NULL AND :NEW.ILC_SCENARIO_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                                  ,
                      'ILC_INSTANCE'                     ,
                      'ILC_SCENARIO_ID'            ,
                      TO_CHAR(:OLD.ILC_SCENARIO_ID),
                      TO_CHAR(:NEW.ILC_SCENARIO_ID),
                      'U'
               );

END IF;
END;
/
/*IVR_SERVER UPDATE TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_IVR_SERVER_UPD AFTER
        UPDATE
        ON &ILC_SCHEMA .IVR_SERVER FOR EACH row BEGIN IF :NEW.NAME <> :OLD.NAME
            OR :NEW.NAME IS NOT NULL
           AND :OLD.NAME IS NULL
            OR :OLD.NAME IS NOT NULL
           AND :NEW.NAME IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID,
                      'IVR_SERVER'      ,
                      'NAME'            ,
                      TO_CHAR(:OLD.NAME),
                      TO_CHAR(:NEW.NAME),
                      'U'
               );

END IF;
IF :NEW.NUM_CHANNELS <> :OLD.NUM_CHANNELS OR :NEW.NUM_CHANNELS IS NOT NULL AND
        :OLD.NUM_CHANNELS IS NULL OR :OLD.NUM_CHANNELS IS NOT NULL AND :NEW.NUM_CHANNELS
        IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID        ,
                      'IVR_SERVER'              ,
                      'NUM_CHANNELS'            ,
                      TO_CHAR(:OLD.NUM_CHANNELS),
                      TO_CHAR(:NEW.NUM_CHANNELS),
                      'U'
               );

END IF;
IF :NEW.DB_CONN <> :OLD.DB_CONN OR :NEW.DB_CONN IS NOT NULL AND :OLD.DB_CONN IS NULL OR
        :OLD.DB_CONN IS NOT NULL AND :NEW.DB_CONN IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID   ,
                      'IVR_SERVER'         ,
                      'DB_CONN'            ,
                      TO_CHAR(:OLD.DB_CONN),
                      TO_CHAR(:NEW.DB_CONN),
                      'U'
               );

END IF;
IF :NEW.ENABLED <> :OLD.ENABLED OR :NEW.ENABLED IS NOT NULL AND :OLD.ENABLED IS NULL OR
        :OLD.ENABLED IS NOT NULL AND :NEW.ENABLED IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID   ,
                      'IVR_SERVER'         ,
                      'ENABLED'            ,
                      TO_CHAR(:OLD.ENABLED),
                      TO_CHAR(:NEW.ENABLED),
                      'U'
               );

END IF;
IF :NEW.AUTH_CHECKER <> :OLD.AUTH_CHECKER OR :NEW.AUTH_CHECKER IS NOT NULL AND
        :OLD.AUTH_CHECKER IS NULL OR :OLD.AUTH_CHECKER IS NOT NULL AND :NEW.AUTH_CHECKER
        IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID        ,
                      'IVR_SERVER'              ,
                      'AUTH_CHECKER'            ,
                      TO_CHAR(:OLD.AUTH_CHECKER),
                      TO_CHAR(:NEW.AUTH_CHECKER),
                      'U'
               );

END IF;
IF :NEW.SCHEDULE_CRON <> :OLD.SCHEDULE_CRON OR :NEW.SCHEDULE_CRON IS NOT NULL AND
        :OLD.SCHEDULE_CRON IS NULL OR :OLD.SCHEDULE_CRON IS NOT NULL AND
        :NEW.SCHEDULE_CRON IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID         ,
                      'IVR_SERVER'               ,
                      'SCHEDULE_CRON'            ,
                      TO_CHAR(:OLD.SCHEDULE_CRON),
                      TO_CHAR(:NEW.SCHEDULE_CRON),
                      'U'
               );

END IF;
IF :NEW.ILC_SCENARIO_ID <> :OLD.ILC_SCENARIO_ID OR :NEW.ILC_SCENARIO_ID IS NOT NULL AND
        :OLD.ILC_SCENARIO_ID IS NULL OR :OLD.ILC_SCENARIO_ID IS NOT NULL AND
        :NEW.ILC_SCENARIO_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID         ,
                      'IVR_SERVER'               ,
                      'ILC_SCENARIO_ID'            ,
                      TO_CHAR(:OLD.ILC_SCENARIO_ID),
                      TO_CHAR(:NEW.ILC_SCENARIO_ID),
                      'U'
               );

END IF;
END;
/
/*IVR_PROJECT UPDATE TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_IVR_PROJECT_UPD AFTER
        UPDATE
        ON &ILC_SCHEMA .IVR_PROJECT FOR EACH row BEGIN IF :NEW.IVR_SERVER_ID <> :OLD.IVR_SERVER_ID
            OR :NEW.IVR_SERVER_ID IS NOT NULL
           AND :OLD.IVR_SERVER_ID IS NULL
            OR :OLD.IVR_SERVER_ID IS NOT NULL
           AND :NEW.IVR_SERVER_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID        ,
                      'IVR_PROJECT'              ,
                      'IVR_SERVER_ID'            ,
                      TO_CHAR(:OLD.IVR_SERVER_ID),
                      TO_CHAR(:NEW.IVR_SERVER_ID),
                      'U'
               );

END IF;
IF :NEW.NAME <> :OLD.NAME OR :NEW.NAME IS NOT NULL AND :OLD.NAME IS NULL OR :OLD.NAME
        IS NOT NULL AND :NEW.NAME IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID,
                      'IVR_PROJECT'      ,
                      'NAME'             ,
                      TO_CHAR(:OLD.NAME) ,
                      TO_CHAR(:NEW.NAME) ,
                      'U'
               );

END IF;
IF :NEW.EMAIL_ADDRESSES <> :OLD.EMAIL_ADDRESSES OR :NEW.EMAIL_ADDRESSES IS NOT NULL AND
        :OLD.EMAIL_ADDRESSES IS NULL OR :OLD.EMAIL_ADDRESSES IS NOT NULL AND
        :NEW.EMAIL_ADDRESSES IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID          ,
                      'IVR_PROJECT'                ,
                      'EMAIL_ADDRESSES'            ,
                      TO_CHAR(:OLD.EMAIL_ADDRESSES),
                      TO_CHAR(:NEW.EMAIL_ADDRESSES),
                      'U'
               );

END IF;
IF :NEW.ENABLED <> :OLD.ENABLED OR :NEW.ENABLED IS NOT NULL AND :OLD.ENABLED IS NULL OR
        :OLD.ENABLED IS NOT NULL AND :NEW.ENABLED IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID  ,
                      'IVR_PROJECT'        ,
                      'ENABLED'            ,
                      TO_CHAR(:OLD.ENABLED),
                      TO_CHAR(:NEW.ENABLED),
                      'U'
               );

END IF;
IF :NEW.SCHEDULE_CRON <> :OLD.SCHEDULE_CRON OR :NEW.SCHEDULE_CRON IS NOT NULL AND
        :OLD.SCHEDULE_CRON IS NULL OR :OLD.SCHEDULE_CRON IS NOT NULL AND
        :NEW.SCHEDULE_CRON IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID        ,
                      'IVR_PROJECT'              ,
                      'SCHEDULE_CRON'            ,
                      TO_CHAR(:OLD.SCHEDULE_CRON),
                      TO_CHAR(:NEW.SCHEDULE_CRON),
                      'U'
               );

END IF;
IF :NEW.ILC_SCENARIO_ID <> :OLD.ILC_SCENARIO_ID OR :NEW.ILC_SCENARIO_ID IS NOT NULL AND
        :OLD.ILC_SCENARIO_ID IS NULL OR :OLD.ILC_SCENARIO_ID IS NOT NULL AND
        :NEW.ILC_SCENARIO_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID        ,
                      'IVR_PROJECT'              ,
                      'ILC_SCENARIO_ID'            ,
                      TO_CHAR(:OLD.ILC_SCENARIO_ID),
                      TO_CHAR(:NEW.ILC_SCENARIO_ID),
                      'U'
               );

END IF;
END;
/
/*SERVICE_INFO UPDATE TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_SERVICE_INFO_UPD AFTER
        UPDATE
        ON &ILC_SCHEMA .SERVICE_INFO FOR EACH row BEGIN IF :NEW.TNUMBER <> :OLD.TNUMBER
            OR :NEW.TNUMBER IS NOT NULL
           AND :OLD.TNUMBER IS NULL
            OR :OLD.TNUMBER IS NOT NULL
           AND :NEW.TNUMBER IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID         ,
                      'SERVICE_INFO'       ,
                      'TNUMBER'            ,
                      TO_CHAR(:OLD.TNUMBER),
                      TO_CHAR(:NEW.TNUMBER),
                      'U'
               );

END IF;
IF :NEW.IVR_PROJECT_ID <> :OLD.IVR_PROJECT_ID OR :NEW.IVR_PROJECT_ID IS NOT NULL AND
        :OLD.IVR_PROJECT_ID IS NULL OR :OLD.IVR_PROJECT_ID IS NOT NULL AND
        :NEW.IVR_PROJECT_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID                ,
                      'SERVICE_INFO'              ,
                      'IVR_PROJECT_ID'            ,
                      TO_CHAR(:OLD.IVR_PROJECT_ID),
                      TO_CHAR(:NEW.IVR_PROJECT_ID),
                      'U'
               );

END IF;
IF :NEW.USERID <> :OLD.USERID OR :NEW.USERID IS NOT NULL AND :OLD.USERID IS NULL OR
        :OLD.USERID IS NOT NULL AND :NEW.USERID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID        ,
                      'SERVICE_INFO'      ,
                      'USERID'            ,
                      TO_CHAR(:OLD.USERID),
                      TO_CHAR(:NEW.USERID),
                      'U'
               );

END IF;
IF :NEW.PWD <> :OLD.PWD OR :NEW.PWD IS NOT NULL AND :OLD.PWD IS NULL OR :OLD.PWD
        IS NOT NULL AND :NEW.PWD IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID     ,
                      'SERVICE_INFO'   ,
                      'PWD'            ,
                      TO_CHAR(:OLD.PWD),
                      TO_CHAR(:NEW.PWD),
                      'U'
               );

END IF;
IF :NEW.ENABLED <> :OLD.ENABLED OR :NEW.ENABLED IS NOT NULL AND :OLD.ENABLED IS NULL OR
        :OLD.ENABLED IS NOT NULL AND :NEW.ENABLED IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID         ,
                      'SERVICE_INFO'       ,
                      'ENABLED'            ,
                      TO_CHAR(:OLD.ENABLED),
                      TO_CHAR(:NEW.ENABLED),
                      'U'
               );

END IF;
IF :NEW.ILC_SCENARIO_ID <> :OLD.ILC_SCENARIO_ID OR :NEW.ILC_SCENARIO_ID IS NOT NULL AND :OLD.ILC_SCENARIO_ID IS NULL OR
        :OLD.ILC_SCENARIO_ID IS NOT NULL AND :NEW.ILC_SCENARIO_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID         ,
                      'SERVICE_INFO'       ,
                      'ILC_SCENARIO_ID'            ,
                      TO_CHAR(:OLD.ILC_SCENARIO_ID),
                      TO_CHAR(:NEW.ILC_SCENARIO_ID),
                      'U'
               );

END IF;
END;
/

/*ILC_SCENARIO UPDATE TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .ILC_SCENARIO_UPD AFTER 
	UPDATE ON &ILC_SCHEMA .ILC_SCENARIO FOR EACH row BEGIN IF :NEW.NAME <> :OLD.NAME 
		OR :NEW.NAME IS NOT NULL 
		AND :OLD.NAME IS NULL 
		OR :OLD.NAME IS NOT NULL 
		AND :NEW.NAME IS NULL THEN 
	INSERT 
	INTO &ILC_SCHEMA .CA_AUDIT 
		(
			RECID, 
			TABLENAME, 
			FIELDNAME, 
			OLDVALUE, 
			NEWVALUE, 
			ACTION
		) 
	VALUES 
		(
			:NEW.ILC_SCENARIO_ID, 
			'ILC_SCENARIO', 
			'NAME', 
			to_char(:OLD.NAME), 
			to_char(:NEW.NAME), 
			'U'
		);	
END IF;
IF :NEW.ASSEMBLIES <> :OLD.ASSEMBLIES OR :NEW.ASSEMBLIES IS NOT NULL AND 
	:OLD.ASSEMBLIES IS NULL OR :OLD.ASSEMBLIES IS NOT NULL AND 
	:NEW.ASSEMBLIES IS NULL THEN 
	INSERT 
	INTO &ILC_SCHEMA .CA_AUDIT 
		(
			RECID, 
			TABLENAME, 
			FIELDNAME, 
			OLDVALUE, 
			NEWVALUE, 
			ACTION
		) 
		VALUES 
		(
			:NEW.ILC_SCENARIO_ID, 
			'ILC_SCENARIO', 
			'ASSEMBLIES', 
			to_char(:OLD.ASSEMBLIES), 
			to_char(:NEW.ASSEMBLIES), 
			'U'
		);	
END IF;
IF :NEW.SCRIPTING_EXPRESSION <> :OLD.SCRIPTING_EXPRESSION OR :NEW.SCRIPTING_EXPRESSION IS NOT NULL AND 
	:OLD.SCRIPTING_EXPRESSION IS NULL OR :OLD.SCRIPTING_EXPRESSION IS NOT NULL AND 
	:NEW.SCRIPTING_EXPRESSION IS NULL THEN 
	INSERT 
	INTO &ILC_SCHEMA .CA_AUDIT 
		(
			RECID, 
			TABLENAME, 
			FIELDNAME, 
			OLDVALUE, 
			NEWVALUE, 
			ACTION
		) 
		VALUES 
		(
			:NEW.ILC_SCENARIO_ID, 
			'ILC_SCENARIO', 
			'SCRIPTING_EXPRESSION', 
			to_char(:OLD.SCRIPTING_EXPRESSION), 
			to_char(:NEW.SCRIPTING_EXPRESSION), 
			'U'
		);	
END IF;
END;
/

/*IVR_SERVER AFTER INSERT TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_IVR_SERVER_INS AFTER
        INSERT
        ON &ILC_SCHEMA .IVR_SERVER FOR EACH row BEGIN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_SERVER_ID,
                      'IVR_SERVER'      ,
                      'I'
               );
        
        IF :NEW.NAME IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID,
                              'IVR_SERVER'      ,
                              'NAME'            ,
                              TO_CHAR(:NEW.NAME),
                              'I'
                       );
        
        END IF;
        IF :NEW.NUM_CHANNELS IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID        ,
                              'IVR_SERVER'              ,
                              'NUM_CHANNELS'            ,
                              TO_CHAR(:NEW.NUM_CHANNELS),
                              'I'
                       );
        
        END IF;
        IF :NEW.DB_CONN IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID   ,
                              'IVR_SERVER'         ,
                              'DB_CONN'            ,
                              TO_CHAR(:NEW.DB_CONN),
                              'I'
                       );
        
        END IF;
        IF :NEW.ENABLED IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID   ,
                              'IVR_SERVER'         ,
                              'ENABLED'            ,
                              TO_CHAR(:NEW.ENABLED),
                              'I'
                       );
        
        END IF;
        IF :NEW.AUTH_CHECKER IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID        ,
                              'IVR_SERVER'              ,
                              'AUTH_CHECKER'            ,
                              TO_CHAR(:NEW.AUTH_CHECKER),
                              'I'
                       );
        
        END IF;
        IF :NEW.SCHEDULE_CRON IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID         ,
                              'IVR_SERVER'               ,
                              'SCHEDULE_CRON'            ,
                              TO_CHAR(:NEW.SCHEDULE_CRON),
                              'I'
                       );
        
        END IF;
	IF :NEW.ILC_SCENARIO_ID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_SERVER_ID         ,
                              'IVR_SERVER'               ,
                              'ILC_SCENARIO_ID'            ,
                              TO_CHAR(:NEW.ILC_SCENARIO_ID),
                              'I'
                       );
        
        END IF;
END;
/
/*IVR_PROJECT AFTER INSERT TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_IVR_PROJECT_INS AFTER
        INSERT
        ON &ILC_SCHEMA .IVR_PROJECT FOR EACH row BEGIN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      ACTION
               )
               VALUES
               (
                      :NEW.IVR_PROJECT_ID,
                      'IVR_PROJECT'      ,
                      'I'
               );
        
        IF :NEW.IVR_SERVER_ID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID        ,
                              'IVR_PROJECT'              ,
                              'IVR_SERVER_ID'            ,
                              TO_CHAR(:NEW.IVR_SERVER_ID),
                              'I'
                       );
        
        END IF;
        IF :NEW.NAME IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID,
                              'IVR_PROJECT'      ,
                              'NAME'             ,
                              TO_CHAR(:NEW.NAME) ,
                              'I'
                       );
        
        END IF;
        IF :NEW.EMAIL_ADDRESSES IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID          ,
                              'IVR_PROJECT'                ,
                              'EMAIL_ADDRESSES'            ,
                              TO_CHAR(:NEW.EMAIL_ADDRESSES),
                              'I'
                       );
        
        END IF;
        IF :NEW.ENABLED IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID  ,
                              'IVR_PROJECT'        ,
                              'ENABLED'            ,
                              TO_CHAR(:NEW.ENABLED),
                              'I'
                       );
        
        END IF;
        IF :NEW.SCHEDULE_CRON IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID        ,
                              'IVR_PROJECT'              ,
                              'SCHEDULE_CRON'            ,
                              TO_CHAR(:NEW.SCHEDULE_CRON),
                              'I'
                       );
        
        END IF;
	IF :NEW.ILC_SCENARIO_ID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.IVR_PROJECT_ID        ,
                              'IVR_PROJECT'              ,
                              'ILC_SCENARIO_ID'            ,
                              TO_CHAR(:NEW.ILC_SCENARIO_ID),
                              'I'
                       );
        
        END IF;
END;
/
/*SERVICE_INFO AFTER INSERT TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_SERVICE_INFO_INS AFTER
        INSERT
        ON &ILC_SCHEMA .SERVICE_INFO FOR EACH row BEGIN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      ACTION
               )
               VALUES
               (
                      :NEW.ITEM_ID  ,
                      'SERVICE_INFO',
                      'I'
               );
        
        IF :NEW.TNUMBER IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID         ,
                              'SERVICE_INFO'       ,
                              'TNUMBER'            ,
                              TO_CHAR(:NEW.TNUMBER),
                              'I'
                       );
        
        END IF;
        IF :NEW.IVR_PROJECT_ID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID                ,
                              'SERVICE_INFO'              ,
                              'IVR_PROJECT_ID'            ,
                              TO_CHAR(:NEW.IVR_PROJECT_ID),
                              'I'
                       );
        
        END IF;
        IF :NEW.USERID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID        ,
                              'SERVICE_INFO'      ,
                              'USERID'            ,
                              TO_CHAR(:NEW.USERID),
                              'I'
                       );
        
        END IF;
        IF :NEW.PWD IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID     ,
                              'SERVICE_INFO'   ,
                              'PWD'            ,
                              TO_CHAR(:NEW.PWD),
                              'I'
                       );
        
        END IF;
        IF :NEW.ENABLED IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID         ,
                              'SERVICE_INFO'       ,
                              'ENABLED'            ,
                              TO_CHAR(:NEW.ENABLED),
                              'I'
                       );
        
        END IF;
	IF :NEW.ILC_SCENARIO_ID IS NOT NULL THEN
                INSERT
                INTO   &ILC_SCHEMA .CA_AUDIT
                       (
                              RECID    ,
                              TABLENAME,
                              FIELDNAME,
                              NEWVALUE ,
                              ACTION
                       )
                       VALUES
                       (
                              :NEW.ITEM_ID         ,
                              'SERVICE_INFO'       ,
                              'ILC_SCENARIO_ID'            ,
                              TO_CHAR(:NEW.ILC_SCENARIO_ID),
                              'I'
                       );
        
        END IF;
END;
/

/*ILC_SCENARIO AFTER INSERT TRIGGER*/
CREATE OR REPLACE TRIGGER &ILC_SCHEMA .TR_ILC_SCENARIO_INS AFTER 
    INSERT
    ON &ILC_SCHEMA .ILC_SCENARIO
    FOR EACH row BEGIN 
    INSERT
    INTO &ILC_SCHEMA .CA_AUDIT
        (
            RECID,
            TABLENAME,
            ACTION
        ) 
        VALUES
        (
            :NEW.ILC_SCENARIO_ID,
            'ILC_SCENARIO',
             'I'
         );
    IF :NEW.NAME IS NOT NULL THEN 
        INSERT INTO &ILC_SCHEMA .CA_AUDIT 
        (
            RECID,
            TABLENAME, 
            FIELDNAME, 
            NEWVALUE, 
            ACTION
        ) 
        VALUES 
        (
            :NEW.ILC_SCENARIO_ID, 
            'ILC_SCENARIO', 
            'NAME', 
            to_char(:NEW.NAME), 
            'I'
        );    
    END IF;
    IF :NEW.ASSEMBLIES IS NOT NULL THEN 
        INSERT INTO &ILC_SCHEMA .CA_AUDIT 
        (
            RECID, 
            TABLENAME, 
            FIELDNAME, 
            NEWVALUE, 
            ACTION
        ) 
        VALUES 
        (
            :NEW.ILC_SCENARIO_ID, 
            'ILC_SCENARIO',
             'ASSEMBLIES', 
             to_char(:NEW.ASSEMBLIES), 
             'I'
         );   
    END IF;
    IF :NEW.SCRIPTING_EXPRESSION IS NOT NULL THEN
    INSERT INTO &ILC_SCHEMA .CA_AUDIT 
    (
        RECID, 
        TABLENAME, 
        FIELDNAME, 
        NEWVALUE, ACTION
    ) 
    VALUES 
    (
        :NEW.ILC_SCENARIO_ID, 
        'ILC_SCENARIO', 
        'SCRIPTING_EXPRESSION', 
        to_char(:NEW.SCRIPTING_EXPRESSION), 
        'I'
    );    
    END IF;
END;
/

/*SERVICE_VERIFICATION_SESSION AFTER UPDATE TRIGGER*/
CREATE OR REPLACE 
TRIGGER &ILC_SCHEMA .TR_SERVICE_VER_SESSION_UPD AFTER
        UPDATE
        ON &ILC_SCHEMA .SERVICE_VERIFICATION_SESSION FOR EACH row 
BEGIN IF :NEW.REPORT_ID <> :OLD.REPORT_ID
			OR :NEW.REPORT_ID IS NOT NULL
           AND :OLD.REPORT_ID IS NULL
            OR :OLD.REPORT_ID IS NOT NULL
           AND :NEW.REPORT_ID IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0         ,
                      'SERVICE_VERIFICATION_SESSION'       ,
                      'REPORT_ID'            ,
                      TO_CHAR(:OLD.REPORT_ID),
                      TO_CHAR(:NEW.REPORT_ID),
                      'U'
               );

END IF;
IF :NEW.START_TIME <> :OLD.START_TIME OR :NEW.START_TIME IS NOT NULL AND
        :OLD.START_TIME IS NULL OR :OLD.START_TIME IS NOT NULL AND
        :NEW.START_TIME IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0                ,
                      'SERVICE_VERIFICATION_SESSION'              ,
                      'START_TIME'            ,
                      TO_CHAR(:OLD.START_TIME),
                      TO_CHAR(:NEW.START_TIME),
                      'U'
               );

END IF;
IF :NEW.IS_REPORTED <> :OLD.IS_REPORTED OR :NEW.IS_REPORTED IS NOT NULL AND :OLD.IS_REPORTED IS NULL OR
        :OLD.IS_REPORTED IS NOT NULL AND :NEW.IS_REPORTED IS NULL THEN
        INSERT
        INTO   &ILC_SCHEMA .CA_AUDIT
               (
                      RECID    ,
                      TABLENAME,
                      FIELDNAME,
                      OLDVALUE ,
                      NEWVALUE ,
                      ACTION
               )
               VALUES
               (
                      0        ,
                      'SERVICE_VERIFICATION_SESSION'      ,
                      'IS_REPORTED'            ,
                      TO_CHAR(:OLD.IS_REPORTED),
                      TO_CHAR(:NEW.IS_REPORTED),
                      'U'
               );
END IF;
END;
/
