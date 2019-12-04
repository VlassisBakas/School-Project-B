using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace AssignmentPartB
{
    class Program
    {

        
        static void Main(string[] args)
        {
            string ConnectionString = @"Data Source=DESKTOP-P2KVBMV\SQLEXPRESS; Initial Catalog=PRIVATE SCHOOL; Integrated Security = true;";

            string sqlGetStudents = "SELECT * FROM STUDENT";
            string sqlGetTrainers = "SELECT * FROM TRAINER";
            string sqlGetCourses = "SELECT * FROM COURSE";
            string sqlGetAssignments = "SELECT * FROM ASSIGNMENT";
            string sqlGetStudentsPreCourse = "SELECT TITLE,STREAM,TYPE,FIRSTNAME,LASTNAME FROM (COURSE INNER JOIN STUDENTPERCOURSE ON COURSE_ID=C_ID) INNER JOIN STUDENT ON STUDENT_ID=STUD_ID ORDER BY COURSE_ID";
            string sqlGetTrainersPerCourse = "SELECT TITLE,STREAM,TYPE,FIRSTNAME,LASTNAME,SUBJECT FROM (COURSE INNER JOIN TRAINERPERCOURSE ON COURSE_ID=C1_ID) INNER JOIN TRAINER ON TRAINER_ID = TR_ID ORDER BY COURSE_ID";
            string sqlGetAssignmentsPerCourse = "SELECT e.TITLE COURSE_TITLE,e.STREAM,e.TYPE,a.TITLE ASSIGNMENT_TITLE,a.DESCRIPTION,a.SUBDATETIME FROM (COURSE e INNER JOIN ASSIGNMENTPERCOURSE ON COURSE_ID=C2_ID) INNER JOIN ASSIGNMENT A ON ASSIGNMENT_ID = AS_ID ORDER BY COURSE_ID";
            string sqlGetAssignmentsPerCoursePerStudent= "SELECT s.FIRSTNAME,s.LASTNAME,c.TITLE COURSE_TITLE,c.STREAM,c.TYPE,a.TITLE ASSIGNMENT_TITLE,a.DESCRIPTION,a.SUBDATETIME FROM(ASSIGNMENT a INNER JOIN ASSIGNMENTPERCOURSE ON ASSIGNMENT_ID = AS_ID)INNER JOIN COURSE c ON C2_ID = COURSE_ID INNER JOIN STUDENTPERCOURSE ON COURSE_ID = C_ID INNER JOIN STUDENT s ON STUD_ID = STUDENT_ID ORDER BY STUDENT_ID ";
            string sqlGetStudsWithMoreCourses = "select * from student A inner join (SELECT STUD_ID,COUNT(*) as NoOfCourses FROM STUDENTPERCOURSE GROUP BY STUD_ID HAVING COUNT(*)> 1 )  B ON A.STUDENT_ID = B.STUD_ID ";

            string sqlInsertStudent = "INSERT INTO STUDENT(STUDENT_ID,FIRSTNAME,LASTNAME,DATEOFBIRTH,TUITIONFEES) VALUES(@id,@fn,@ln,@dob,@tf)";
            string sqlInsertTrainer = "INSERT INTO TRAINER(TRAINER_ID,FIRSTNAME,LASTNAME, SUBJECT) VALUES(@id,@fn,@ln,@sub)";
            string sqlInsertAssignment = "INSERT INTO ASSIGNMENT(ASSIGNMENT_ID,TITLE,DESCRIPTION,SUBDATETIME,ORALMARK,TOTALMARK) VALUES(@id,@title,@desc,@subdate,@om,@tm)";
            string sqlInsertCourse = "INSERT INTO COURSE(COURSE_ID,TITLE,STREAM,TYPE,STARTDATE,ENDDATE) VALUES(@id,@title,@stream,@type,@sdate,@edate)";
            string sqlInserStudentsPerCourse = "INSERT INTO STUDENTPERCOURSE(C_ID,STUD_ID) VALUES(@cid,@sid)";
            string sqlInsertTrainerPerCourse = "INSERT INTO TRAINERPERCOURSE(C1_ID,TR_ID) VALUES(@cid,@trid)";
            string sqlCreateAsPerStPerCourse = "If not exists (select name from sysobjects where name = 'ASSIGNMENTPERSTUDENTPERCOURSE') CREATE TABLE ASSIGNMENTPERSTUDENTPERCOURSE(AS1_ID INTEGER NOT NULL,C3_ID INTEGER NOT NULL,STUD1_ID INTEGER NOT NULL CONSTRAINT PK_APSPC PRIMARY KEY(AS1_ID, C3_ID, STUD1_ID), CONSTRAINT FK_AS1_ID FOREIGN KEY(AS1_ID) REFERENCES ASSIGNMENT(ASSIGNMENT_ID), CONSTRAINT FK_C3_ID FOREIGN KEY(C3_ID) REFERENCES COURSE(COURSE_ID), CONSTRAINT FK_STUD1_ID FOREIGN KEY(STUD1_ID) REFERENCES STUDENT(STUDENT_ID));";
            string sqlInsertAsPerStPerCourse = "INSERT INTO ASSIGNMENTPERSTUDENTPERCOURSE(AS1_ID,C3_ID,STUD1_ID) VALUES(@as1id,@c3id,@stud1id) ";
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();

                    #region List of Students 
                    Console.WriteLine(" ");
                    Console.WriteLine("STUDENTS ");
                    using (SqlCommand cmGetStudents = new SqlCommand(sqlGetStudents, cn))
                    {
                        using (SqlDataReader drStudents=cmGetStudents.ExecuteReader())
                        {
                            Console.WriteLine($"{"STUDENT_ID",-20}{"FIRSTNAME",-20}{"LASTNAME",-20}{"DATE OF BIRTH",-30}{"TUITION FEES",-10}");
                            while (drStudents.Read())
                            {
                                string StId = drStudents["STUDENT_ID"].ToString();
                                string StFN = drStudents["FIRSTNAME"].ToString();
                                string StLN = drStudents["LASTNAME"].ToString();
                                string StDoB = drStudents["DATEOFBIRTH"].ToString();
                                string StTF = drStudents["TUITIONFEES"].ToString();
                               
                                Console.WriteLine($"{StId,-20}{StFN,-20}{StLN,-20}{StDoB,-30}{StTF,-10}");
                            }
                        }
                    };
                    #endregion
                    #region List of Trainers
                    Console.WriteLine(" ");
                    Console.WriteLine("TRAINERS ");
                    using (SqlCommand cmGetTrainers = new SqlCommand(sqlGetTrainers, cn))
                    {
                        using(SqlDataReader drTrainers = cmGetTrainers.ExecuteReader())
                        {
                            Console.WriteLine($"{"TRAINER_ID",-20}{"FIRSTNAME",-20}{"LASTNAME",-20}{"SUBJECT",-20}");
                            while (drTrainers.Read())
                            {
                                string TrId = drTrainers["TRAINER_ID"].ToString();
                                string TrFN = drTrainers["FIRSTNAME"].ToString();
                                string TrLN = drTrainers["LASTNAME"].ToString();
                                string TrSubj = drTrainers["SUBJECT"].ToString();
                                Console.WriteLine($"{TrId,-20}{TrFN,-20}{TrLN,-20}{TrSubj,-20}");
                            }
                        }
                    };
                    #endregion
                    #region List of courses
                    Console.WriteLine(" ");
                    Console.WriteLine("COURSES ");
                    using (SqlCommand cmGetCourses = new SqlCommand(sqlGetCourses, cn))
                    {
                        using (SqlDataReader drCourses=cmGetCourses.ExecuteReader())
                        {
                            Console.WriteLine($"{"COURSE_ID",-10}{"TITLE",-10}{"STREAM",-10}{"TYPE",-15}{"STARTDATE",-25}{"ENDDATE",-20}");
                            while (drCourses.Read())
                            {
                                string CId = drCourses["COURSE_ID"].ToString();
                                string CTitle = drCourses["TITLE"].ToString();
                                string CStream = drCourses["STREAM"].ToString();
                                string CType = drCourses["TYPE"].ToString();
                                string CStart = drCourses["STARTDATE"].ToString();
                                string CEnd = drCourses["ENDDATE"].ToString();
                                Console.WriteLine($"{CId,-10}{CTitle,-10}{CStream,-10}{CType,-15}{CStart,-25}{CEnd,-20}");
                            }
                        }
                    };
                    #endregion
                    #region List of Assignments
                    Console.WriteLine(" ");
                    Console.WriteLine("ASSIGNMENTS");
                    using (SqlCommand cmGetAssignments = new SqlCommand(sqlGetAssignments, cn))
                    {
                        using (SqlDataReader drAssignments=cmGetAssignments.ExecuteReader())
                        {
                            Console.WriteLine($"{"ASSIGNMENT_ID",-15}{"TITLE",-15}{"DESCRIPTION",-25}{"SUBDATETIME",-25}{"ORALMARK",-10}{"TOTALMARK",-10}");
                            while (drAssignments.Read())
                            {
                                string ASId = drAssignments["ASSIGNMENT_ID"].ToString();
                                string ASTitle = drAssignments["TITLE"].ToString();
                                string ASDesc = drAssignments["DESCRIPTION"].ToString();
                                string ASSub = drAssignments["SUBDATETIME"].ToString();
                                string ASOM = drAssignments["ORALMARK"].ToString();
                                string ASTM = drAssignments["TOTALMARK"].ToString();
                                Console.WriteLine($"{ASId,-15}{ASTitle,-15}{ASDesc,-25}{ASSub,-25}{ASOM,-10}{ASTM,-10}");
                            }
                        }
                    };
                    #endregion
                    #region All students per course
                    Console.WriteLine(" ");
                    Console.WriteLine("Students Per Course");
                    using (SqlCommand cmGetStudentsPerCourse = new SqlCommand(sqlGetStudentsPreCourse,cn))
                    {
                        using (SqlDataReader drSpC=cmGetStudentsPerCourse.ExecuteReader())
                        {
                            Console.WriteLine($"{"TITLE",-10}P{"STREAM",-15}{"TYPE",-15}{"FIRSTNAME",-20}{"LASTNAME",-20}");
                            while (drSpC.Read())
                            {
                                string CTitle = drSpC["TITLE"].ToString();
                                string CStream = drSpC["STREAM"].ToString();
                                string CType = drSpC["TYPE"].ToString();
                                string StFName = drSpC["FIRSTNAME"].ToString();
                                string StLName = drSpC["LASTNAME"].ToString();
                                Console.WriteLine($"{CTitle,-10}{CStream,-15}{CType,-15}{StFName,-20}{StLName,-20}");
                            }
                        }
                    };
                    #endregion
                    #region All trainers per course
                    Console.WriteLine(" ");
                    Console.WriteLine("Trainers Per Course");
                    using (SqlCommand cmGetTrainerPerCourse = new SqlCommand(sqlGetTrainersPerCourse, cn))
                    {
                        using (SqlDataReader drTpC = cmGetTrainerPerCourse.ExecuteReader())
                        {
                            Console.WriteLine($"{"TITLE",-10}{"STREAM",-10}{"TYPE",-10}{"FIRSTNAME",-15}{"LASTNAME",-15}{"SUBJECT",15}");
                            while (drTpC.Read())
                            {
                                string CTitle = drTpC["TITLE"].ToString();
                                string CStream = drTpC["STREAM"].ToString();
                                string CType = drTpC["TYPE"].ToString();
                                string TrFN = drTpC["FIRSTNAME"].ToString();
                                string TrLN = drTpC["LASTNAME"].ToString();
                                string TrS = drTpC["SUBJECT"].ToString();
                                Console.WriteLine($"{CTitle,-10}{CStream,-10}{CType,-10}{TrFN,-15}{TrLN,-15}{TrS,15}");




                            }
                        }
                    };
                    #endregion
                    #region All assignments per course
                    Console.WriteLine(" ");
                    Console.WriteLine("Assignments Per Course");
                    using(SqlCommand cmGetAssignmentsPerCourse=new SqlCommand(sqlGetAssignmentsPerCourse, cn))
                    {
                        Console.WriteLine($"{"COURSE TITLE",-15}{"STREAM",-8}{"TYPE",-10}{"ASSIGNMENT TITLE",-20}{"DESCRIPTION",-20}{"SUBDATETIME",-15}");
                        using (SqlDataReader drApC = cmGetAssignmentsPerCourse.ExecuteReader())
                        {
                            while (drApC.Read())
                            {
                                string CTitle = drApC["COURSE_TITLE"].ToString();
                                string CStream = drApC["STREAM"].ToString();
                                string CType = drApC["TYPE"].ToString();
                                string ASTitle = drApC["ASSIGNMENT_TITLE"].ToString();
                                string ASDesc = drApC["DESCRIPTION"].ToString();
                                string ASSub = drApC["SUBDATETIME"].ToString();
                                Console.WriteLine($"{CTitle,-15}{CStream,-8}{CType,-10}{ASTitle,-20}{ASDesc,-20}{ASSub,-15}");
                            }
                        }
                    };
                    #endregion
                    #region All Assignments per course per student
                    Console.WriteLine(" ");
                    Console.WriteLine("Assignments Per Course Per Student");
                    using (SqlCommand cmGetAssignmentsPerCoursepPerStudent = new SqlCommand(sqlGetAssignmentsPerCoursePerStudent, cn))
                    {
                        Console.WriteLine($"{"FIRSTNAME",-12}{"LASTNAME",-15}{"COURSE TITLE",-15}{"STREAM",-8}{"TYPE",-10}{"ASSIGNMENT TITLE",-20}{"DESCRIPTION",-15}{"SUBDATETIME",-15}");
                        using(SqlDataReader drApS = cmGetAssignmentsPerCoursepPerStudent.ExecuteReader())
                        {
                            while (drApS.Read())
                            {
                                string CTitle = drApS["COURSE_TITLE"].ToString();
                                string CStream = drApS["STREAM"].ToString();
                                string CType = drApS["TYPE"].ToString();
                                string ASTitle = drApS["ASSIGNMENT_TITLE"].ToString();
                                string ASDesc = drApS["DESCRIPTION"].ToString();
                                string ASSub = drApS["SUBDATETIME"].ToString();
                                string SFN = drApS["FIRSTNAME"].ToString();
                                string SLN = drApS["LASTNAME"].ToString();
                                Console.WriteLine($"{SFN,-12}{SLN,-15}{CTitle,-15}{CStream,-8}{CType,-10}{ASTitle,-20}{ASDesc,-15}{ASSub,-15}");
                            }
                        }
                    };
                    #endregion
                    #region  List of students that belong to more than one courses
                    Console.WriteLine(" ");
                    Console.WriteLine("Students that belong to more than one courses");
                    using (SqlCommand cmGetStudsWithMoreCourses = new SqlCommand(sqlGetStudsWithMoreCourses, cn))
                    {
                        Console.WriteLine($"{"FIRSTNAME",-15}{"LASTNAME",-15}");
                        using (SqlDataReader drSMC=cmGetStudsWithMoreCourses.ExecuteReader())
                        {
                            while (drSMC.Read())
                            {
                                string StFN = drSMC["FIRSTNAME"].ToString();
                                string StLN = drSMC["LASTNAME"].ToString();
                               //string No = drSMC[" NoOfCourses"].ToString();
                                Console.WriteLine($"{StFN,-15}{StLN,-15}");
                            }
                        }
                    };
                    #endregion

                    Console.WriteLine("Do you want to insert data to the database? (Press 'Y' for yes or 'N' for no)" );
                    string answer = Console.ReadLine();
                    if (answer=="Y")
                    {
                        int i = 0;
                        #region Insert students
                        Console.WriteLine("How many students do you want to insert? (Please enter a number)");
                        int x =Convert.ToInt32(Console.ReadLine());
                        while (i < x)
                        {
                            SqlCommand cmInsertStudent = new SqlCommand(sqlInsertStudent, cn);
                            Console.WriteLine("Enter student id:");
                            int ID = Convert.ToInt32(Console.ReadLine());
                           

                            Console.WriteLine("Enter Firstname:");
                            string fn = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter Lastname:");
                            string ln = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter date of birth:");
                            DateTime dob = Convert.ToDateTime(Console.ReadLine());
                            Console.WriteLine("Enter tuition fees: ");
                            int tf = Convert.ToInt32(Console.ReadLine());

                            cmInsertStudent.Parameters.Add("id", ID);
                            cmInsertStudent.Parameters.Add("fn", fn);
                            cmInsertStudent.Parameters.Add("ln", ln);
                            cmInsertStudent.Parameters.Add("dob", dob);
                            cmInsertStudent.Parameters.Add("tf", tf);
                            int rowsAffected = cmInsertStudent.ExecuteNonQuery();
                            Console.WriteLine("Student Inserted Succesfully: {0}", rowsAffected);
                            i++;
                        }
                        #endregion
                        #region Insert trainers
                        Console.WriteLine("How many trainers do you want to insert? (Please enter a number)");
                        int y = Convert.ToInt32(Console.ReadLine());
                        while (i < y)
                        {
                            SqlCommand cmInsertTrainer = new SqlCommand(sqlInsertTrainer, cn);
                            Console.WriteLine("Enter trainer id:");
                            int ID = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter Firstname:");
                            string fn = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter Lastname:");
                            string ln = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter subject:");
                            string sub = Convert.ToString(Console.ReadLine());
                            
                            cmInsertTrainer.Parameters.Add("id", ID);
                            cmInsertTrainer.Parameters.Add("fn", fn);
                            cmInsertTrainer.Parameters.Add("ln", ln);
                            cmInsertTrainer.Parameters.Add("sub", sub);
                            int rowsAffected = cmInsertTrainer.ExecuteNonQuery();
                            Console.WriteLine("Trainer Inserted Succesfully: {0}", rowsAffected);
                            i++;
                        }
                        #endregion
                        #region Insert Assignment
                        Console.WriteLine("How many assignments do you want to insert? (Please enter a number)");
                        int z = Convert.ToInt32(Console.ReadLine());
                        while (i < z)
                        {
                            SqlCommand cmInsertAssignment = new SqlCommand(sqlInsertAssignment, cn);
                            Console.WriteLine("Enter assignment id:");
                            int id = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter title:");
                            string title = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter description:");
                            string desc = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter submission date");
                            DateTime subdate = Convert.ToDateTime(Console.ReadLine());
                            Console.WriteLine("Enter oral mark: ");
                            int om = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter total mark:");
                            int tm = Convert.ToInt32(Console.ReadLine());
                            cmInsertAssignment.Parameters.Add("id", id);
                            cmInsertAssignment.Parameters.Add("title", title);
                            cmInsertAssignment.Parameters.Add("desc", desc);
                            cmInsertAssignment.Parameters.Add("subdate", subdate);
                            cmInsertAssignment.Parameters.Add("om", om);
                            cmInsertAssignment.Parameters.Add("tm", tm);
                            int rowsAffected = cmInsertAssignment.ExecuteNonQuery();
                            Console.WriteLine("assignment Inserted Succesfully: {0}", rowsAffected);
                            i++;
                        }
                        #endregion
                        #region Insert Courses
                        Console.WriteLine("How many courses do you want to insert? (Please enter a number)");
                        int l = Convert.ToInt32(Console.ReadLine());
                        while (i < l)
                        {
                            SqlCommand cmInsertCourse = new SqlCommand(sqlInsertCourse, cn);
                            Console.WriteLine("Enter course id:");
                            int id = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("Enter title:");
                            string title = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter stream:");
                            string stream = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter type:");
                            string type = Convert.ToString(Console.ReadLine());
                            Console.WriteLine("Enter start date");
                            DateTime sdate = Convert.ToDateTime(Console.ReadLine());
                            Console.WriteLine("Enter end date");
                            DateTime edate = Convert.ToDateTime(Console.ReadLine());
                           
                            cmInsertCourse.Parameters.Add("id", id);
                            cmInsertCourse.Parameters.Add("title", title);
                            cmInsertCourse.Parameters.Add("stream", stream);
                            cmInsertCourse.Parameters.Add("type", type);
                            cmInsertCourse.Parameters.Add("sdate", sdate);
                            cmInsertCourse.Parameters.Add("edate", edate);
                            int rowsAffected = cmInsertCourse.ExecuteNonQuery();
                            Console.WriteLine("Course Inserted Succesfully: {0}", rowsAffected);
                            i++;
                        }
                        #endregion
                        #region Insert Student per course
                        Console.WriteLine("Do you want to insert the students in the courses (Press 'Y' for yes)");
                        Console.WriteLine("When you finish enter 'DONE' ");
                        string answer1 = Console.ReadLine();
                        if (answer1 == "Y")
                        {
                            do
                            {
                                SqlCommand cmInsertSpC = new SqlCommand(sqlInserStudentsPerCourse, cn);
                                Console.WriteLine("Enter student id:");
                                int sid = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Enter course id:");
                                int cid = Convert.ToInt32(Console.ReadLine());
                                cmInsertSpC.Parameters.Add("cid", cid);
                                cmInsertSpC.Parameters.Add("sid", sid);
                                int rowsAffected = cmInsertSpC.ExecuteNonQuery();
                                Console.WriteLine("StudentPerCourse Inserted Succesfully: {0}", rowsAffected);
                                Console.WriteLine("if you finished press 'DONE', else press enter");
                                string d = Console.ReadLine();
                                if (d == "DONE") break;

                            } while (true);
                            
                        }


                        #endregion
                        #region Insert Trainer per course
                        Console.WriteLine("Do you want to insert the trainers in the courses (Press 'Y' for yes)");
                        Console.WriteLine("When you finish enter 'DONE' ");
                        string answer2 = Console.ReadLine();
                        if (answer1 == "Y")
                        {
                            do
                            {
                                SqlCommand cmInsertTpC = new SqlCommand(sqlInsertTrainerPerCourse, cn);
                                Console.WriteLine("Enter trainer id:");
                                int trid = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Enter course id:");
                                int cid = Convert.ToInt32(Console.ReadLine());
                                cmInsertTpC.Parameters.Add("cid", cid);
                                cmInsertTpC.Parameters.Add("trid", trid);
                                int rowsAffected = cmInsertTpC.ExecuteNonQuery();
                                Console.WriteLine("TrainerPerCourse Inserted Succesfully: {0}", rowsAffected);
                                Console.WriteLine("if you finished press 'DONE', else press enter");
                                string d = Console.ReadLine();
                                if (d == "DONE") break;

                            } while (true);

                        }
                        #endregion

                        #region Create and Insert Assignment per student per course
                        using (SqlCommand cmInsertApC = new SqlCommand(sqlCreateAsPerStPerCourse, cn))
                        {
                            cmInsertApC.ExecuteNonQuery();
                        }

                        Console.WriteLine("Do you want to insert the assignments per students per courses (Press 'Y' for yes)");
                        Console.WriteLine("When you finish enter 'DONE' ");
                        string answer3 = Console.ReadLine();
                        if (answer3 == "Y")
                        {
                            do
                            {
                                SqlCommand cmInsertApSpC = new SqlCommand(sqlInsertAsPerStPerCourse, cn);
                                Console.WriteLine("Enter assignment id:");
                                int as1id = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Enter course id:");
                                int c3id = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Enter student id:");
                                int stud1id = Convert.ToInt32(Console.ReadLine());
                                cmInsertApSpC.Parameters.Add("as1id", as1id);
                                cmInsertApSpC.Parameters.Add("c3id", c3id);
                                cmInsertApSpC.Parameters.Add("stud1id", stud1id);
                                int rowsAffected = cmInsertApSpC.ExecuteNonQuery();
                                Console.WriteLine("AssignmentPerStudentPerCourse Inserted Succesfully: {0}", rowsAffected);
                                Console.WriteLine("if you finished press 'DONE', else press enter");
                                string d = Console.ReadLine();
                                if (d == "DONE") break;

                            } while (true);

                        }
                        #endregion
                    }

                }
            }
            catch (SqlException es)
            {
                for (int i = 0; i < es.Errors.Count; i++)
                {
                    Console.WriteLine("Error: {0}", es.Errors[i].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           


           
           
        }
    }
}
