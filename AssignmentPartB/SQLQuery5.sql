﻿USE [PRIVATE SCHOOL];
CREATE TABLE STUDENTPERCOURSE(
STUD_ID INTEGER NOT NULL,
C_ID INTEGER NOT NULL,
CONSTRAINT PK_SPC PRIMARY KEY(STUD_ID,C_ID),
CONSTRAINT FK_STUD_ID FOREIGN KEY(STUD_ID) REFERENCES STUDENT(STUDENT_ID),
CONSTRAINT FK_C_ID FOREIGN KEY(C_ID) REFERENCES COURSE(COURSE_ID)
);