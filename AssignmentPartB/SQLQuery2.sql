﻿USE [PRIVATE SCHOOL];
CREATE TABLE TRAINER(
TRAINER_ID INTEGER NOT NULL,
FIRSTNAME VARCHAR(50) NOT NULL,
LASTNAME VARCHAR(50) NOT NULL,
SUBJECT VARCHAR(50) NOT NULL,
CONSTRAINT PK_TRAINER PRIMARY KEY(TRAINER_ID)
);