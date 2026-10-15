using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Attendance
{
    public class SectionSearchRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
    }

    public class PeriodLoadRequest
    {
        public string? AcdYr { get; set; }
        public string? Lecturer { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Date { get; set; }
    }

    public class MidAttDatesRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Date { get; set; }
    }

    public class LoadStudentsRequest
    {
        public string? Lecturer { get; set; }
        public string? Period { get; set; }
        public string? Date { get; set; }
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public bool IsPractical { get; set; } = false;
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
    }

    public class FacultyStudentAttendanceItem
    {
        public int SNo { get; set; }
        public string? RegNo { get; set; }
        public string? Status { get; set; } // "P" or "A"
        public string? Remarks { get; set; }
    }

    public class SaveAttendanceSubWiseRequest
    {
        public string? Lecturer { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? AcademicYear { get; set; }
        public string? Day { get; set; }
        public string? Date { get; set; }
        public string? UpdateDate { get; set; } = "NULL";
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
        public string? DayTaught { get; set; }
        public string? AttStat { get; set; }
        public string? Query { get; set; }
        public string? PeriodRange { get; set; }
        public string? TLM { get; set; }
        public List<FacultyStudentAttendanceItem>? Students { get; set; }
    }

    public class AdminDatesCheckRequest
    {
        public string? Lecturer { get; set; }
        public string? Branch { get; set; }
        public string? AcdYr { get; set; }
        public string? Date { get; set; }
    }

    public class EmpPeriodsAttRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Lecturer { get; set; }
    }

    public class SaveAttendanceRequest
    {
        public string? Id { get; set; }
        public string? Date { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? AbsentNumber { get; set; }
        public string? AbsentNumbers { get; set; }
        public string? PeriodRange { get; set; }
    }

    public class DayPeriodsLoadRequest
    {
        public string? UserId { get; set; }
        public string? AcademicYear { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Date { get; set; }
    }

    public class AttStudentsCountRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
    }

    public class AttStudentsAttStatRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? Lecturer { get; set; }
    }

    public class DeleteFacultyAttStatRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? Lecturer { get; set; }
    }

    public class CheckPeriodAttStatusRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? Lecturer { get; set; }
    }

    public class SectionChangeRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class UpdateSectionRequest
    {
        public string? Section { get; set; }
        public string? UpdateSection { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
    }

    public class StatusRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
    }

    public class DelInactiveStudentsRequest
    {
        public string? Section { get; set; }
        public string? DeleteStudents { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
        public string? Status { get; set; }
    }

    public class InactiveStudentsRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
        public string? Status { get; set; }
    }

    public class PostingDatesRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
    }

    public class AttAdmPermissionsRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public string? Date { get; set; }
    }

    public class ValidDatesRequest
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
    }

    public class SaveMidAttendanceRequest
    {
        public string? Id { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Date { get; set; }
        public string? ToDate { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
    }

    public class MidAttDataRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
    }
}
