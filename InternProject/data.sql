CREATE TABLE ES_YDENPYO (
    DENPYONO NUMBER PRIMARY KEY,               -- Số phiếu, khóa chính
    KAIKEIND NUMBER,                           --  Năm tài chính
    UKETUKEDT VARCHAR2(10) NOT NULL,            --  Ngày tiếp nhận (YYYY-MM-DD hoặc tương tự)
    DENPYODT VARCHAR2(10),                     --  Ngày ghi trên phiếu
    BUMONCD_YKANR VARCHAR2(20),                     --  Mã phòng ban lập phiếu
    BIKO VARCHAR2(200),                    --  Ghi chú, mục đích công tác
    SUITOKB VARCHAR2(10),                     --  Phương thức thanh toán 
    SHIHARAIDT VARCHAR2(10),                     --  Ngày dự kiến thanh toán
    KINGAKU NUMBER,                           --  Số tiền (tổng chi phí)
    INSERT_OPE_ID VARCHAR2(30),                     --  ID người tạo
    INSERT_PGM_ID  VARCHAR2(30),                     --  ID chương trình tạo
    INSERT_PGM_PRM  VARCHAR2(30),                     --  Tham số chương trình tạo
    INSERT_DATE VARCHAR2(10),                             --  Ngày giờ tạo
    UPDATE_OPE_ID VARCHAR2(30),                     --  ID người cập nhật
    UPDATE_PGM_ID VARCHAR2(30),                     --  ID chương trình cập nhật
    UPDATE_PGM_PRM  VARCHAR2(30),                     --  Tham số chương trình cập nhật
    UPDATE_DATE VARCHAR2(10),                            --  Ngày giờ cập nhật
    
     CONSTRAINT fk_bumoncd_ykanr 
        FOREIGN KEY (BUMONCD_YKANR) 
            REFERENCES BUMON(BUMONCD) ON DELETE CASCADE
);


CREATE TABLE ES_YDENPYOD (
    GYONO   NUMBER,               -- Số dòng
    DENPYONO  NUMBER NOT NULL,     -- Số phiếu (khóa ngoại từ bảng ES_YDENPYO)
    IDODT VARCHAR2(20),        -- Ngày di chuyển
    SHUPPATSU_PLC VARCHAR2(100),       -- Xuất phát
    MOKUTEKI_PLC  VARCHAR2(100),       -- Đích đến
    KEIRO VARCHAR2(200),       -- Lộ trình
    KINGAKU NUMBER,              -- Số tiền
    INSERT_OPE_ID VARCHAR2(50),        -- Người tạo
    INSERT_PGM_ID VARCHAR2(50),        -- Chương trình tạo
    INSERT_PGM_PRM VARCHAR2(50),        -- Param chương trình tạo
    INSERT_DATE VARCHAR2(10),                -- Ngày giờ tạo
    UPDATE_OPE_ID VARCHAR2(50),        -- Người cập nhật
    UPDATE_PGM_ID VARCHAR2(50),        -- Chương trình cập nhật
    UPDATE_PGM_PRM VARCHAR2(50),        -- Param chương trình cập nhật
    UPDATE_DATE  VARCHAR2(10),                -- Ngày giờ cập nhật
   
    CONSTRAINT  fk_ES_YDENPYOD FOREIGN KEY (DENPYONO)
    REFERENCES ES_YDENPYO( DENPYONO ) 
      ON DELETE CASCADE
    
);

CREATE TABLE BUMON(
    BUMONCD	VARCHAR2(20) PRIMARY KEY,    --ID phòng ban
    BUMONNM VARCHAR2(100)				 --Tên phòng ban
)

--Insert data
INSERT INTO BUMON (BUMONCD, BUMONNM) VALUES ('HR', '人事部');
INSERT INTO BUMON (BUMONCD, BUMONNM) VALUES ('ACC', '経理部');
INSERT INTO BUMON (BUMONCD, BUMONNM) VALUES ('IT', '情報システム部');
INSERT INTO BUMON (BUMONCD, BUMONNM) VALUES ('MKT', 'マーケティング部');
INSERT INTO BUMON (BUMONCD, BUMONNM) VALUES ('SAL', '営業部');

INSERT INTO ES_YDENPYO VALUES (1001, 2024, '2024-07-01', '2024-07-01', 'HR', 'ハノイ出張', '振込', '2024-07-10', 5000000, 'user01', 'pgm01', 'prm01', '2024-07-01', 'user01', 'pgm01', 'prm01', '2024-07-01');
INSERT INTO ES_YDENPYO VALUES (1002, 2024, '2024-07-02', '2024-07-02', 'ACC', 'セミナー参加', '現金', '2024-07-12', 4200000, 'user02', 'pgm02', 'prm02', '2024-07-02', 'user02', 'pgm02', 'prm02', '2024-07-02');
INSERT INTO ES_YDENPYO VALUES (1003, 2024, '2024-07-03', '2024-07-03', 'IT', '支店点検', '振込', '2024-07-13', 3800000, 'user03', 'pgm03', 'prm03', '2024-07-03', 'user03', 'pgm03', 'prm03', '2024-07-03');
INSERT INTO ES_YDENPYO VALUES (1004, 2024, '2024-07-04', '2024-07-04', 'MKT', '取引先訪問', '現金', '2024-07-14', 6000000, 'user04', 'pgm04', 'prm04', '2024-07-04', 'user04', 'pgm04', 'prm04', '2024-07-04');
INSERT INTO ES_YDENPYO VALUES (1005, 2024, '2024-07-05', '2024-07-05', 'SAL', '市場調査', '振込', '2024-07-15', 4500000, 'user05', 'pgm05', 'prm05', '2024-07-05', 'user05', 'pgm05', 'prm05', '2024-07-05');

INSERT INTO ES_YDENPYOD VALUES (1, 1001, '2024-07-02', 'Ho Chi Minh', 'Hanoi', 'Airplane', 2500000, 'user01', 'pgm01', 'prm01', '2024-07-01', 'user01', 'pgm01', 'prm01', '2024-07-01');
INSERT INTO ES_YDENPYOD VALUES (2, 1001, '2024-07-03', 'Hanoi', 'Ho Chi Minh', 'Airplane', 2500000, 'user01', 'pgm01', 'prm01', '2024-07-01', 'user01', 'pgm01', 'prm01', '2024-07-01');

INSERT INTO ES_YDENPYOD VALUES (1, 1002, '2024-07-05', 'Ho Chi Minh', 'Da Nang', 'Train', 2100000, 'user02', 'pgm02', 'prm02', '2024-07-02', 'user02', 'pgm02', 'prm02', '2024-07-02');
INSERT INTO ES_YDENPYOD VALUES (2, 1002, '2024-07-07', 'Da Nang', 'Ho Chi Minh', 'Train', 2100000, 'user02', 'pgm02', 'prm02', '2024-07-02', 'user02', 'pgm02', 'prm02', '2024-07-02');

INSERT INTO ES_YDENPYOD VALUES (1, 1003, '2024-07-05', 'Ho Chi Minh', 'Binh Duong', 'Car', 1900000, 'user03', 'pgm03', 'prm03', '2024-07-03', 'user03', 'pgm03', 'prm03', '2024-07-03');
INSERT INTO ES_YDENPYOD VALUES (2, 1003, '2024-07-06', 'Binh Duong', 'Ho Chi Minh', 'Car', 1900000, 'user03', 'pgm03', 'prm03', '2024-07-03', 'user03', 'pgm03', 'prm03', '2024-07-03');

INSERT INTO ES_YDENPYOD VALUES (1, 1004, '2024-07-10', 'Ho Chi Minh', 'Nha Trang', 'Bus', 3000000, 'user04', 'pgm04', 'prm04', '2024-07-04', 'user04', 'pgm04', 'prm04', '2024-07-04');
INSERT INTO ES_YDENPYOD VALUES (2, 1004, '2024-07-12', 'Nha Trang', 'Ho Chi Minh', 'Bus', 3000000, 'user04', 'pgm04', 'prm04', '2024-07-04', 'user04', 'pgm04', 'prm04', '2024-07-04');

INSERT INTO ES_YDENPYOD VALUES (1, 1005, '2024-07-15', 'Ho Chi Minh', 'Can Tho', 'Bus', 2250000, 'user05', 'pgm05', 'prm05', '2024-07-05', 'user05', 'pgm05', 'prm05', '2024-07-05');
INSERT INTO ES_YDENPYOD VALUES (2, 1005, '2024-07-16', 'Can Tho', 'Ho Chi Minh', 'Bus', 2250000, 'user05', 'pgm05', 'prm05', '2024-07-05', 'user05', 'pgm05', 'prm05', '2024-07-05');

DELETE FROM ES_YDENPYOD;
DELETE FROM ES_YDENPYO;
DELETE FROM BUMON;

SELECT * FROM bumon;
SELECT * FROM es_ydenpyo WHERE es_ydenpyo.bumoncd_ykanr = 'HR';

SELECT * FROM es_ydenpyo 
JOIN es_ydenpyod ON es_ydenpyo.DENPYONO = es_ydenpyod.DENPYONO 
ORDER BY es_ydenpyo.denpyono;

SELECT * FROM es_ydenpyo 
JOIN es_ydenpyod USING (DENPYONO) 
ORDER BY DENPYONO;

ALTER TABLE ES_YDENPYOD ADD CONSTRAINT PK_ES_YDENPYOD PRIMARY KEY (DENPYONO, GYONO);

--------------------------------
CREATE USER BAI1 IDENTIFIED BY BAI1;

GRANT CONNECT, RESOURCE, DBA TO BAI1;

SELECT SYS_CONTEXT('USERENV', 'CON_NAME') from DUAL

SELECT NAME, OPEN_MODE FROM V$PDBS;
SELECT CON_ID, NAME, DBID, OPEN_MODE, RESTRICTED FROM V$PDBS;

ALTER SESSION SET CONTAINER = XEPDB1;


