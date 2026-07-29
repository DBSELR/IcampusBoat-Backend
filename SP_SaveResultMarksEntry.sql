USE [db_ERP_Lbrce_react]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SP_SaveResultMarksEntry]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SP_SaveResultMarksEntry]
GO

CREATE PROCEDURE [dbo].[SP_SaveResultMarksEntry]
    @id INT = 0,
    @RegistrationNo VARCHAR(50) = NULL,
    @Date VARCHAR(50) = NULL,
    @Programme VARCHAR(50) = NULL,
    @Branch VARCHAR(50) = NULL,
    @Year VARCHAR(50) = NULL,
    @Semester VARCHAR(50) = NULL,
    @Section VARCHAR(50) = NULL,
    @Stream VARCHAR(50) = '1',
    @SubjectName VARCHAR(100) = NULL,
    @Sub_Max_MRK VARCHAR(50) = NULL,
    @Max_MRK VARCHAR(50) = NULL,
    @Marks VARCHAR(50) = NULL,
    @Grade VARCHAR(50) = NULL,
    @SGPA VARCHAR(50) = NULL,
    @CGPA VARCHAR(50) = NULL,
    @AcademicYear VARCHAR(50) = NULL,
    @Lecturer VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM tbl_Result_Marks WHERE RegistrationNo = @RegistrationNo AND SubjectName = @SubjectName AND Semester = @Semester AND AcademicYear = @AcademicYear)
    BEGIN
        UPDATE tbl_Result_Marks
        SET 
            Date = @Date,
            Programme = @Programme,
            Branch = @Branch,
            Year = @Year,
            Section = @Section,
            Stream = @Stream,
            Sub_Max_MRK = @Sub_Max_MRK,
            Max_MRK = @Max_MRK,
            Marks = @Marks,
            Grade = @Grade,
            SGPA = @SGPA,
            CGPA = @CGPA,
            Lecturer = @Lecturer
        WHERE RegistrationNo = @RegistrationNo 
          AND SubjectName = @SubjectName 
          AND Semester = @Semester 
          AND AcademicYear = @AcademicYear;
    END
    ELSE
    BEGIN
        INSERT INTO tbl_Result_Marks 
        (
            RegistrationNo, Date, Programme, Branch, Year, Semester, 
            Section, Stream, SubjectName, Sub_Max_MRK, Max_MRK, 
            Marks, Grade, SGPA, CGPA, AcademicYear, Lecturer
        )
        VALUES 
        (
            @RegistrationNo, @Date, @Programme, @Branch, @Year, @Semester, 
            @Section, @Stream, @SubjectName, @Sub_Max_MRK, @Max_MRK, 
            @Marks, @Grade, @SGPA, @CGPA, @AcademicYear, @Lecturer
        );
    END
END
GO
