namespace IcampusBoatBackend.Models.Attendance
{
    public class AttendanceDeletionBranchesRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
    }

    public class AttendanceDeletionYearsRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
    }

    public class AttendanceDeletionSectionsRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
    }

    public class AttendanceDeletionPeriodsRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
    }

    public class AttendanceDeletionLecturersRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
    }

    public class AttendanceDeletionSubjectsRequest
    {
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Lecturer { get; set; }
    }

    public class AttendanceDeletionGridRequest
    {
        public string? EmpId { get; set; }
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Lecturer { get; set; }
        public string? Subject { get; set; }
    }

    public class AttendanceDeletionDeleteRequest
    {
        public string? EmpId { get; set; }
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Lecturer { get; set; }
        public string? Subject { get; set; }
    }
}
